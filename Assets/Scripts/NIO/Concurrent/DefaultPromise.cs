
namespace NIO
{
	using System;
	using System.Threading;

	public class DefaultPromise<V> : AbstractFuture<V>, IPromise<V>
	{
		// @TODO: 把 SUCCESS 转成常量
		private static readonly object SUCCESS = new object();
		// @TODO: 把 UNCANCELLABLE 转成常量
		private static readonly object UNCANCELLABLE = new object();

		private static Exception CANCELLATION_CAUSE = new OperationCanceledException(" at NIO.DefaultPromise`V.Cancel()");

		private volatile object result;
		private readonly IEventExecutor executor;

		private object listeners;
		private short waiters;
		private bool notifyingListeners;
		private readonly object syncLock = new object();

		public DefaultPromise() : this(null)
		{
		}

		public DefaultPromise(IEventExecutor executor)
		{
			this.executor = ObjectUtil.CheckNotNull(executor, "executor");
		}

		public override bool IsDone { get { return result != null && result != UNCANCELLABLE; } }

		public override bool IsSuccess { get { return result != null && result != UNCANCELLABLE && !(result is Exception); } }

		public override bool IsCancelled { get { return result is OperationCanceledException; } }

		public override bool IsCancellable { get { return result == null; } }

		public override Exception Cause { get { return (result is Exception) ? (Exception)result : null; } }

		public override V GetNow()
		{
			if (result is Exception || result == SUCCESS)
				return default(V);
			return (V)result;
		}

		public override bool Cancel(bool mayInterruptIfRunning)
		{
			Interlocked.CompareExchange(ref result, CANCELLATION_CAUSE, null);
			if (result == CANCELLATION_CAUSE)
			{
				_CheckNotifyWaiters();
				_NotifyListeners();
				return true;
			}
			return false;
		}

		new public IPromise<V> AddListener(IFutureListener<V> listener)
		{
			ObjectUtil.CheckNotNull(listener, "listener");
			lock (syncLock)
			{
				_AddListener(listener);
			}
			if (IsDone)
			{
				_NotifyListeners();
			}
			return this;
		}

		new public IPromise<V> AddListeners(params IFutureListener<V>[] listeners)
		{
			ObjectUtil.CheckNotNull(listeners, "listeners");
			lock (syncLock)
			{
				IFutureListener<V> listener;
				for (int i = 0; i < listeners.Length; ++i)
				{
					listener = listeners[i];
					if (listener == null)
						continue;
					_AddListener(listener);
				}
			}
			if (IsDone)
			{
				_NotifyListeners();
			}
			return this;
		}

		new public IPromise<V> RemoveListener(IFutureListener<V> listener)
		{
			ObjectUtil.CheckNotNull(listener, "listener");
			lock (syncLock)
			{
				_RemoveListener(listener);
			}
			return this;
		}

		new public IPromise<V> RemoveListeners(params IFutureListener<V>[] listeners)
		{
			ObjectUtil.CheckNotNull(listeners, "listeners");
			lock (syncLock)
			{
				IFutureListener<V> listener;
				for (int i = 0; i < listeners.Length; ++i)
				{
					listener = listeners[i];
					if (listener == null)
						continue;
					_RemoveListener(listener);
				}
			}
			return this;
		}

		new public IPromise<V> Sync()
		{
			Await();
			_RethrowIfFailed();
			return this;
		}

		new public IPromise<V> SyncUninterruptibly()
		{
			AwaitUninterruptibly();
			_RethrowIfFailed();
			return this;
		}

		new public IPromise<V> Await()
		{
			if (IsDone)
				return this;
			CheckDeadLock();
			lock (syncLock)
			{
				while (!IsDone)
				{
					_IncWaiters();
					try
					{
						Monitor.Wait(syncLock);
					}
					finally
					{
						_DecWaiters();
					}
				}
			}
			return this;
		}

		new public IPromise<V> AwaitUninterruptibly()
		{
			if (IsDone)
				return this;
			CheckDeadLock();
			bool interrupted = false;
			lock (syncLock)
			{
				while (!IsDone)
				{
					_IncWaiters();
					try
					{
						Monitor.Wait(syncLock);
					}
					catch (ThreadInterruptedException)
					{
						interrupted = true;
					}
					finally
					{
						_DecWaiters();
					}
				}
			}
			if (interrupted)
				Thread.CurrentThread.Interrupt();
			return this;
		}

		public override bool Await(long milliseconds)
		{
			return _Await(milliseconds, true);
		}

		public override bool AwaitUninterruptibly(long milliseconds)
		{
			try
			{
				return _Await(milliseconds, false);
			}
			catch (ThreadInterruptedException e)
			{
				throw new Exception("should not be raised at all.", e);
			}
		}

		public IPromise<V> SetSuccess(V result)
		{
			if (_SetSuccess(result))
			{
				_NotifyListeners();
				return this;
			}
			throw new Exception("complete already: " + this);
		}

		public bool TrySuccess(V result)
		{
			if (_SetSuccess(result))
			{
				_NotifyListeners();
				return true;
			}
			return false;
		}

		public IPromise<V> SetFailure(Exception cause)
		{
			if (_SetFailure(cause))
			{
				_NotifyListeners();
				return this;
			}
			throw new Exception("complete already: " + this, cause);
		}

		public bool TryFailure(Exception cause)
		{
			if (_SetFailure(cause))
			{
				_NotifyListeners();
				return true;
			}
			return false;
		}

		public bool SetUncancelable()
		{
			Interlocked.CompareExchange(ref result, UNCANCELLABLE, null);
			if (result == UNCANCELLABLE)
				return true;
			return !IsDone || !IsCancelled;
		}

		protected IEventExecutor Executor { get { return executor; } }

		protected void CheckDeadLock()
		{
			IEventExecutor e = Executor;
			if (e != null && e.InEventLoop())
				throw new ThreadStateException("deadlock");
		}

		protected internal static void NotifyListener(IEventExecutor eventExecutor, IFuture<V> future, IFutureListener<V> listener)
		{
			ObjectUtil.CheckNotNull(eventExecutor, "eventExecutor");
			ObjectUtil.CheckNotNull(future, "future");
			ObjectUtil.CheckNotNull(listener, "listener");
			_NotifyListenerWithStackOverFlowProtection(eventExecutor, future, listener);
		}

		private void _NotifyListeners()
		{
			IEventExecutor executor = Executor;
			if(executor.InEventLoop())
			{
				// @TODO: 增加对调用堆栈深度的保护（避免出现堆栈溢出）
				try
				{
					_NotifyListenersNow();
				}
				catch { }
				return;
			}
			_SafeExecute(executor, () => { _NotifyListenersNow(); });
		}

		private static void _NotifyListenerWithStackOverFlowProtection(IEventExecutor eventExecutor, IFuture<V> future, IFutureListener<V> listener)
		{
			if (eventExecutor.InEventLoop())
			{
				// @TODO: 增加对调用堆栈深度的保护（避免出现堆栈溢出）
				try
				{
					_NotifyListener(future, listener);
				}
				catch { }
				return;
			}
			_SafeExecute(eventExecutor, () => { _NotifyListener(future, listener); });
		}

		private void _NotifyListenersNow()
		{
			object listeners;
			lock(syncLock)
			{
				if (notifyingListeners || this.listeners == null)
					return;
				notifyingListeners = true;
				listeners = this.listeners;
				this.listeners = null;
			}
			for(;;)
			{
				if (listeners is DefaultFutureListeners<V>)
					_NotifyListeners((DefaultFutureListeners<V>)listeners);
				else
					_NotifyListener(this, (IFutureListener<V>)listeners);
				lock(syncLock)
				{
					// double check，再检查一遍看有没有没处理完的listener
					if(this.listeners == null)
					{
						notifyingListeners = false;
						return;
					}
					listeners = this.listeners;
					this.listeners = null;
				}
			}
		}

		private void _NotifyListeners(DefaultFutureListeners<V> listeners)
		{
			IFutureListener<V>[] array = listeners.Listeners;
			int size = listeners.Size;
			for(int i = 0; i < size; ++i)
			{
				_NotifyListener(this, array[i]);
			}
		}

		private static void _NotifyListener(IFuture<V> future, IFutureListener<V> listener)
		{
			try
			{
				listener.OperationComplete(future);
			}
			catch (Exception ex)
			{
				// @TODO: 添加对ex的Log
			}
		}

		private static void _SafeExecute(IEventExecutor eventExecutor, Action task)
		{
			try
			{
				eventExecutor.Execute(task);
			}
			catch (Exception ex)
			{
				// @TODO: 添加对ex的Log
			}
		}

		private void _AddListener(IFutureListener<V> listener)
		{
			if (this.listeners == null)
				this.listeners = listener;
			else if (this.listeners is DefaultFutureListeners<V>)
				((DefaultFutureListeners<V>)this.listeners).Add(listener);
			else
				this.listeners = new DefaultFutureListeners<V>((IFutureListener<V>)this.listeners, listener);
		}

		private void _RemoveListener(IFutureListener<V> listener)
		{
			if (this.listeners is DefaultFutureListeners<V>)
				((DefaultFutureListeners<V>)this.listeners).Remove(listener);
			else if (listeners == listener)
				listeners = null;
		}

		private void _IncWaiters()
		{
			if (waiters == short.MaxValue)
				throw new Exception("too many waiters: " + this);
			++waiters;
		}

		private void _DecWaiters()
		{
			--waiters;
		}

		private bool _Await(long timeoutMilli, bool interruptable)
		{
			if (IsDone)
				return true;
			if (timeoutMilli <= 0)
				return IsDone;
			CheckDeadLock();
			long startTime = DateTime.Now.Millisecond;
			long waitTime = timeoutMilli;
			bool interrupted = false;
			try
			{
				for (;;)
				{
					lock (syncLock)
					{
						if (IsDone)
							return true;
						_IncWaiters();
						try
						{
							Monitor.Wait(syncLock, (int)waitTime);
						}
						catch (ThreadInterruptedException e)
						{
							if (interruptable)
								throw e;
							else
								interrupted = true;
						}
						finally
						{
							_DecWaiters();
						}
					}
					if (IsDone)
						return true;
					else
					{
						waitTime = timeoutMilli - (DateTime.Now.Millisecond - startTime);
						if (waitTime <= 0)
							return IsDone;
					}
				}
			}
			finally
			{
				if (interrupted)
					Thread.CurrentThread.Interrupt();
			}
		}

		private bool _SetSuccess(V result)
		{
			return _SetValue(result == null ? SUCCESS : result);
		}

		private bool _SetFailure(Exception cause)
		{
			return _SetValue(cause);
		}

		private bool _SetValue(object objResult)
		{
			Interlocked.CompareExchange(ref result, objResult, null);
			Interlocked.CompareExchange(ref result, objResult, UNCANCELLABLE);
			if (result == objResult)
			{
				_CheckNotifyWaiters();
				return true;
			}
			return false;
		}

		private void _CheckNotifyWaiters()
		{
			lock (syncLock)
			{
				if (waiters > 0)
					Monitor.PulseAll(syncLock);
			}
		}

		private void _RethrowIfFailed()
		{
			Exception ex = Cause;
			if (ex != null)
				throw ex;
		}
	}
}
