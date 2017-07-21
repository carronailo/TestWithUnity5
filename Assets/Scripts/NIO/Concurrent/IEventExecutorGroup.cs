
namespace NIO
{
	using System;

	public interface IEventExecutorGroup
	{
		bool IsShuttingDown { get; }
		bool IsShutdown { get; }
		bool IsTerminated { get; }

		IFuture<V> ShutdownGracefully<V>();
		IFuture<V> ShutdownGracefully<V>(long quietPeriod, long timeoutMilli);
		IFuture<V> TerminationFuture<V>();

		IEventExecutor Next();

		IFuture<V> Submit<V>(ICallable<V> task);
		IFuture<V> Submit<V>(IRunnable task);
		IFuture<V> Submit<V>(IRunnable task, V result);

		void Execute(IRunnable task);
		void Execute(Action task);
	}
}
