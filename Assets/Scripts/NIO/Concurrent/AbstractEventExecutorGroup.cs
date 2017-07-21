
namespace NIO
{
	using System;

	public abstract class AbstractEventExecutorGroup : IEventExecutorGroup
	{
		public abstract bool IsShutdown { get; }
		public abstract bool IsShuttingDown { get; }
		public abstract bool IsTerminated { get; }

		public abstract void Execute(Action task);
		public abstract void Execute(IRunnable task);
		public abstract IEventExecutor Next();
		public abstract IFuture<V> ShutdownGracefully<V>();
		public abstract IFuture<V> ShutdownGracefully<V>(long quietPeriod, long timeoutMilli);
		public abstract IFuture<V> Submit<V>(IRunnable task);
		public abstract IFuture<V> Submit<V>(ICallable<V> task);
		public abstract IFuture<V> Submit<V>(IRunnable task, V result);
		public abstract IFuture<V> TerminationFuture<V>();
	}
}
