
namespace NIO
{

	public class DefaultEventExecutor : SingleThreadEventExecutor
	{
		public DefaultEventExecutor(IEventExecutorGroup parent) : base(parent)
		{
		}
	}
}
