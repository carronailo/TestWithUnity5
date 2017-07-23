
using System;

namespace NIO
{
	public abstract class AbstractFuture<V> : IFuture<V>
	{
		public abstract bool IsDone { get; }
		public abstract bool IsSuccess { get; }
		public abstract bool IsCancelled { get; }
		public abstract bool IsCancellable { get; }
		public abstract Exception Cause { get; }

		public abstract V GetNow();
		public abstract bool Cancel(bool mayInterruptIfRunning);
		public abstract bool Await(long milliseconds);
		public abstract bool AwaitUninterruptibly(long milliseconds);

		public virtual V Get()
		{
			Await();
			Exception cause = Cause;
			if (cause == null)
				return GetNow();
			if(cause is OperationCanceledException)
				throw (OperationCanceledException)cause;
			throw cause;
		}

		public virtual V Get(long miliseconds)
		{
			if (Await(miliseconds))
			{
				Exception cause = Cause;
				if (cause == null)
					return GetNow();
				if (cause is OperationCanceledException)
					throw (OperationCanceledException)cause;
				throw cause;
			}
			throw new TimeoutException();
		}

		public virtual IFuture<V> AddListener(IFutureListener<V> listener)
		{
			throw new NotImplementedException();
		}

		public virtual IFuture<V> AddListeners(params IFutureListener<V>[] listeners)
		{
			throw new NotImplementedException();
		}

		public virtual IFuture<V> RemoveListener(IFutureListener<V> listener)
		{
			throw new NotImplementedException();
		}

		public virtual IFuture<V> RemoveListeners(params IFutureListener<V>[] listeners)
		{
			throw new NotImplementedException();
		}

		public virtual IFuture<V> Sync()
		{
			throw new NotImplementedException();
		}

		public virtual IFuture<V> SyncUninterruptibly()
		{
			throw new NotImplementedException();
		}

		public virtual IFuture<V> Await()
		{
			throw new NotImplementedException();
		}

		public virtual IFuture<V> AwaitUninterruptibly()
		{
			throw new NotImplementedException();
		}

	}
}

