
namespace NIO
{
	using System;
	using System.Threading;

	public class MultithreadEventExecutorGroup : AbstractEventExecutorGroup
	{
		static readonly int DefaultEventExecutorCount = Environment.ProcessorCount * 2;

		readonly IEventExecutor[] eventExecutors;
		int requestID = -1;

		public MultithreadEventExecutorGroup() : this(DefaultEventExecutorCount)
		{

		}

		public MultithreadEventExecutorGroup(int eventExecutorCount)
		{
			eventExecutors = new IEventExecutor[eventExecutorCount];
			for (int i = 0; i < eventExecutorCount; ++i)
			{
				//eventExecutors[i] = new IEventExecutor();
			}
		}

		public int ExecutorCount { get { return eventExecutors.Length; } }

		public override bool IsShutdown
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool IsShuttingDown
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool IsTerminated
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override void Execute(IRunnable task)
		{
			throw new NotImplementedException();
		}

		public override void Execute(Action task)
		{
			throw new NotImplementedException();
		}

		public IEventExecutor GetNext()
		{
			int id = Interlocked.Increment(ref requestID);
			return eventExecutors[Math.Abs(id % eventExecutors.Length)];
		}

		public override IEventExecutor Next()
		{
			throw new NotImplementedException();
		}

		public override IFuture<V> ShutdownGracefully<V>()
		{
			throw new NotImplementedException();
		}

		public override IFuture<V> ShutdownGracefully<V>(long quietPeriod, long timeoutMilli)
		{
			throw new NotImplementedException();
		}

		public override IFuture<V> Submit<V>(ICallable<V> task)
		{
			throw new NotImplementedException();
		}

		public override IFuture<V> Submit<V>(IRunnable task)
		{
			throw new NotImplementedException();
		}

		public override IFuture<V> Submit<V>(IRunnable task, V result)
		{
			throw new NotImplementedException();
		}

		public override IFuture<V> TerminationFuture<V>()
		{
			throw new NotImplementedException();
		}
	}
}
