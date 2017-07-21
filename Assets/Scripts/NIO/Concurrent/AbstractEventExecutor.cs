
namespace NIO
{
	using System;
	using System.Threading;

	public abstract class AbstractEventExecutor : IEventExecutor
	{
		private static readonly long DEFAULT_SHUTDOWN_QUIET_PERIOD = 2000;
		private static readonly long DEFAULT_SHUTDOWN_TIMEOUT = 15000;

		private readonly IEventExecutorGroup parent;

		protected AbstractEventExecutor() : this(null)
		{
		}

		protected AbstractEventExecutor(IEventExecutorGroup parent)
		{
			this.parent = parent;
		}

		public abstract bool IsShuttingDown { get; }
		public abstract bool IsShutdown { get; }
		public abstract bool IsTerminated { get; }

		public IEventExecutorGroup Parent { get { return parent; } }

		public IEventExecutor Next()
		{
			return this;
		}

		public bool InEventLoop()
		{
			return InEventLoop(Thread.CurrentThread);
		}

		public abstract bool InEventLoop(Thread thread);

		public IFuture<V> ShutdownGracefully<V>()
		{
			return ShutdownGracefully<V>(DEFAULT_SHUTDOWN_QUIET_PERIOD, DEFAULT_SHUTDOWN_TIMEOUT);
		}

		public abstract IFuture<V> ShutdownGracefully<V>(long quietPeriod, long timeoutMilli);

		public abstract IFuture<V> TerminationFuture<V>();

		public IPromise<V> NewPromise<V>()
		{
			return new DefaultPromise<V>(this);
		}

		public IFuture<V> NewSucceededFuture<V>(V result)
		{
			return new SucceededFuture<V>(this, result);
		}

		public IFuture<V> NewFailedFuture<V>(Exception cause)
		{
			return new FailedFuture<V>(this, cause);
		}

		public IFuture<V> Submit<V>(ICallable<V> task)
		{
			if (task == null)
				throw  new NullReferenceException();
			return null;
		}

		public IFuture<V> Submit<V>(IRunnable task)
		{
			if (task == null)
				throw new NullReferenceException();
			return null;
		}

		public IFuture<V> Submit<V>(IRunnable task, V result)
		{
			if (task == null)
				throw new NullReferenceException();
			return null;
		}

		public abstract void Execute(IRunnable task);
		public abstract void Execute(Action task);
	}
}
