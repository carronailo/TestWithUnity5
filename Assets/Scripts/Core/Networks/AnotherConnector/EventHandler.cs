
using System;

namespace Networks.Temp
{
	public interface IEventHandler
	{
		void OnConnected(Connector ctx);
		void OnConnectFailed(Connector ctx, string error);
		void OnDisconnected(Connector ctx);
		void OnDisconnectFailed(Connector ctx, string error);
		void OnConnectionLost(Connector ctx);
		void OnReconnected(Connector ctx);
		void OnReconnectFailed(Connector ctx, string error);
		void OnDataSent(Connector ctx, int sendBytes);
		void OnDataSendFailed(Connector ctx, string error);
		void OnDataReceived(Connector ctx, int receiveBytes);
		void OnDataReceiveFailed(Connector ctx, string error);
	}

	public class EventHandlerBase : IEventHandler
	{
		#region 对外可见，可重载的接口

		public virtual void OnConnected(Connector ctx)
		{
			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("连接上了");
		}

		public virtual void OnConnectFailed(Connector ctx, string error)
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("连接失败：" + error);
		}

		public virtual void OnConnectionLost(Connector ctx)
		{
			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("连接丢失");
		}

		public virtual void OnDataSendFailed(Connector ctx, string error)
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("发送数据失败：" + error);
		}

		public virtual void OnDataSent(Connector ctx, int sendBytes)
		{
			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("发送数据成功，发送" + sendBytes + "字节");
		}

		public virtual void OnDisconnected(Connector ctx)
		{
			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("连接断开了");
		}

		public virtual void OnDisconnectFailed(Connector ctx, string error)
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("断开连接失败" + error);
		}

		public virtual void OnReconnected(Connector ctx)
		{
			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("重新连接上了");
		}

		public virtual void OnReconnectFailed(Connector ctx, string error)
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("重连失败：" + error);
		}

		public virtual void OnDataReceived(Connector ctx, int receiveBytes)
		{
			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("接收数据成功，接收" + receiveBytes + "字节");
		}

		public virtual void OnDataReceiveFailed(Connector ctx, string error)
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("接收数据失败：" + error);
		}

		#endregion
	}

}
