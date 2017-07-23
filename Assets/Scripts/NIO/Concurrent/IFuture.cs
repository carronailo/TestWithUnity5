
using System;

namespace NIO
{
	public interface IFuture<V>
	{
		bool IsDone { get; }
		bool IsSuccess { get; }
		bool IsCancelled { get; }
		bool IsCancellable { get; }
		Exception Cause { get; }

		V Get();
		V Get(long miliseconds);
		V GetNow();
		bool Cancel(bool mayInterruptIfRunning);
		IFuture<V> AddListener(IFutureListener<V> listener);
		IFuture<V> AddListeners(params IFutureListener<V>[] listeners);
		IFuture<V> RemoveListener(IFutureListener<V> listener);
		IFuture<V> RemoveListeners(params IFutureListener<V>[] listeners);
		IFuture<V> Sync();
		IFuture<V> SyncUninterruptibly();
		IFuture<V> Await();
		IFuture<V> AwaitUninterruptibly();
		bool Await(long milliseconds);
		bool AwaitUninterruptibly(long milliseconds);
 	}
}
