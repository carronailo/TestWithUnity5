using MessageSystem;

namespace LogicFramework
{
	public abstract class LogicMessageProcessUnit : MessageProcessUnitBase
	{
		protected LogicModule host = null;

		protected abstract void OnMessageProcessUnitInitialized();
	}
}
