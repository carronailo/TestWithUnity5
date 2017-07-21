
using System;

namespace NIO
{
	public interface IPromise<V> : IFuture<V>
	{
		IPromise<V> SetSuccess(V result);
		bool TrySuccess(V result);
		IPromise<V> SetFailure(Exception cause);
		bool TryFailure(Exception cause);
		bool SetUncancelable();

		new IPromise<V> AddListener(IFutureListener<V> listener);
		new IPromise<V> AddListeners(params IFutureListener<V>[] listeners);
		new IPromise<V> RemoveListener(IFutureListener<V> listener);
		new IPromise<V> RemoveListeners(params IFutureListener<V>[] listeners);
		new IPromise<V> Sync();
		new IPromise<V> SyncUninterruptibly();
		new IPromise<V> Await();
		new IPromise<V> AwaitUninterruptibly();
	}
}
