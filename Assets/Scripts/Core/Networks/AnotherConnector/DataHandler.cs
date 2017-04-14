
namespace Networks.Temp
{
	public interface IDataHandler
	{
		void OnDataReceived(Connector ctx, byte[] data);
	}

	public class DataHandlerBase : IDataHandler
	{
		public virtual void OnDataReceived(Connector ctx, byte[] data)
		{
		}
	}

}
