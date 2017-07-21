
namespace NIO
{
	using System;
	using System.Threading;

	public interface IEventExecutor : IEventExecutorGroup
	{
		IEventExecutorGroup Parent { get; }

		bool InEventLoop();
		bool InEventLoop(Thread thread);

		IPromise<V> NewPromise<V>();

		IFuture<V> NewSucceededFuture<V>(V result);
		IFuture<V> NewFailedFuture<V>(Exception cause);
	}
}
