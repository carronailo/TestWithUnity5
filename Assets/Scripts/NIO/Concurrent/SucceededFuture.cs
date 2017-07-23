
namespace NIO
{
	using System;

	public class SucceededFuture<V> : CompleteFuture<V>
	{
		private readonly V result;

		public SucceededFuture(IEventExecutor executor, V result) : base(executor)
		{
			this.result = result;
		}

		public override Exception Cause { get { return null; } }

		public override bool IsSuccess { get { return true; } }

		public override V GetNow()
		{
			return result;
		}
	}
}
