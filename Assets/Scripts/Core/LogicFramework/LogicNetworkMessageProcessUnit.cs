using Networks;

namespace LogicFramework
{
	public abstract class LogicNetworkMessageProcessUnit : NetworkMessageProcessUnitBase
	{
		protected LogicModule host = null;

		protected abstract void OnNetworkMessageProcessUnitInitialized();
	}
}
