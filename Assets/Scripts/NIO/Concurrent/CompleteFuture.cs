
namespace NIO
{
	using System;

	public abstract class CompleteFuture<V> : AbstractFuture<V>
	{
		private readonly IEventExecutor executor;

		public CompleteFuture(IEventExecutor executor)
		{
			this.executor = ObjectUtil.CheckNotNull(executor, "executor");
		}

		public override bool IsDone { get { return true; } }

		public override bool IsCancelled { get { return false; } }

		public override bool IsCancellable { get { return false; } }

		public override bool Cancel(bool mayInterruptIfRunning)
		{
			return false;
		}

		public override IFuture<V> AddListener(IFutureListener<V> listener)
		{
			if (listener == null)
				throw new NullReferenceException("listener");
			DefaultPromise<V>.NotifyListener(Executor, this, listener);
			return this;
		}

		public override IFuture<V> AddListeners(params IFutureListener<V>[] listeners)
		{
			if (listeners == null)
				throw new NullReferenceException("listeners");
			for (int i = 0; i < listeners.Length; ++i)
			{
				IFutureListener<V> listener = listeners[i];
				if (listener == null)
					continue;
				DefaultPromise<V>.NotifyListener(Executor, this, listener);
			}
			return this;
		}

		public override IFuture<V> RemoveListener(IFutureListener<V> listener)
		{
			return this;
		}

		public override IFuture<V> RemoveListeners(params IFutureListener<V>[] listeners)
		{
			return this;
		}

		public override IFuture<V> Await()
		{
			return this;
		}

		public override IFuture<V> AwaitUninterruptibly()
		{
			return this;
		}

		public override bool Await(long milliseconds)
		{
			return true;
		}

		public override bool AwaitUninterruptibly(long milliseconds)
		{
			return true;
		}

		public override IFuture<V> Sync()
		{
			return this;
		}

		public override IFuture<V> SyncUninterruptibly()
		{
			return this;
		}

		protected IEventExecutor Executor { get { return executor; } }

	}
}
