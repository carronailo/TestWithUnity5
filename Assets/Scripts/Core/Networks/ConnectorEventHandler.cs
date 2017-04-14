
namespace Networks
{
	public abstract class ConnectorEventHandler
	{
		public abstract void OnConnected();
		public abstract void OnDisconnected();
		public abstract void OnDisconnectedUnexpected();
		public abstract void OnReconnectedByDaemon();
		public abstract void OnReconnectFailed();

		public virtual void OnReceive(byte[] data)
		{
			OnReceive(0, 0, data);
		}
		public abstract void OnReceive(byte moduleID, byte functionID, byte[] data);
		public abstract void OnSend();
	}
}

