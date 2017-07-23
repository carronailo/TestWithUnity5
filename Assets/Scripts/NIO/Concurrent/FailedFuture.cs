
namespace NIO
{
	using System;

	public class FailedFuture<V> : CompleteFuture<V>
	{
		private readonly Exception cause;

		public FailedFuture(IEventExecutor executor, Exception cause) : base(executor)
		{
			if (cause == null)
				throw new NullReferenceException("cause");
			this.cause = cause;
		}

		public override Exception Cause { get { return cause; } }

		public override bool IsSuccess { get { return false; } }

		public override IFuture<V> Sync()
		{
			throw cause;
		}

		public override IFuture<V> SyncUninterruptibly()
		{
			throw cause;
		}

		public override V GetNow()
		{
			return default(V);
		}
	}
}
