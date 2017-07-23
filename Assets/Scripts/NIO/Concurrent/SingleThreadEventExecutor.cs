
namespace NIO
{
	using System;
	using System.Diagnostics;
	using System.Threading;

	public class SingleThreadEventExecutor : AbstractEventExecutor
	{
		private static readonly int ST_NOT_STARTED	= 1;
		private static readonly int ST_STARTED = 2;
		private static readonly int ST_SHUTTING_DOWN = 3;
		private static readonly int ST_SHUTDOWN = 4;
		private static readonly int ST_TERMINATED = 5;

		private readonly Thread thread = null;

		private readonly SynchronizedQueue<IRunnable> taskQueue;

		private volatile int state = ST_NOT_STARTED;

		// 一个信号量，用来标识taskQueue是否为空，信号量为signaled（true）表明taskQueue中有待处理的任务，否则表明没有任务可以挂起工作线程
		// @TODO: .Net4以后有更轻量级的ManualResetEventSlim
		readonly ManualResetEvent hasAnyTaskEvent = new ManualResetEvent(false);

		public override bool IsShuttingDown
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool IsShutdown
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

		public SingleThreadEventExecutor(IEventExecutorGroup parent) : base(parent)
		{
			taskQueue = new SynchronizedQueue<IRunnable>();
			thread = new Thread(Loop);
			thread.Start();
		}

		public override bool InEventLoop(Thread thread)
		{
			return thread == this.thread;
		}

		public override void Execute(IRunnable task)
		{
			if (task == null)
				throw new NullReferenceException("task");

			taskQueue.TryEnqueue(task);

			// 调用是在本线程内的话，不需要设置信号量，因为如果是本线程调用，说明线程没有挂起，既然没有挂起那么信号量也就不起作用
			if (!InEventLoop())
				hasAnyTaskEvent.Set();
		}

		public override void Execute(Action task)
		{
			Execute(new ActionTaskQueueNode(task));
		}

		private void StartThread()
		{
			if(state == ST_NOT_STARTED)
			{
				Interlocked.CompareExchange(ref state, ST_STARTED, ST_NOT_STARTED);
			}
		}

		void Loop()
		{
			while(!CheckShutdown())
			{
				RunAllTasks();
			}
		}

		bool RunAllTasks()
		{
			// 断言：必定是在本线程内调用
			Debug.Assert(InEventLoop());

			bool res = false;
			IRunnable task = PollTask();
			if (task != null)
			{
				while(true)
				{
					try
					{
						task.Run();
					}
					catch(Exception ex)
					{
						// @TODO: 接入日志输出接口
						//LogConsole.LogException(ex);
					}
					task = this.PollTask();
					if(task == null)
						break;
				}
				res = true;
			}
			return res;
		}

		bool CheckShutdown()
		{
			return false;
		}

		IRunnable PollTask()
		{
			// 断言：必定是在本线程内调用
			Debug.Assert(InEventLoop());

			IRunnable task;
			if(!taskQueue.TryDequeue(out task))
			{
				hasAnyTaskEvent.Reset();
				// double check，对线程安全关键资源检查的标准步骤
				if(!taskQueue.TryDequeue(out task))
				{
					// 没有待执行任务，线程挂起
					hasAnyTaskEvent.WaitOne();
					taskQueue.TryDequeue(out task);
				}
			}
			return task;
		}

		public override IFuture<V> ShutdownGracefully<V>(long quietPeriod, long timeoutMilli)
		{
			throw new NotImplementedException();
		}

		public override IFuture<V> TerminationFuture<V>()
		{
			throw new NotImplementedException();
		}

		sealed class ActionTaskQueueNode : IRunnable
		{
			readonly Action action;

			public ActionTaskQueueNode(Action action)
			{
				this.action = action;
			}

			public void Run()
			{
				this.action();
			}
		}
	}
}
