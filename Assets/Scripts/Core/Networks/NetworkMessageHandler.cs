using System;
using System.Reflection;

namespace Networks
{
	public delegate void NetworkMessageHandlerDelegate<in T>(T remoteMessage) where T : INetworkMessage;

	internal class NetworkMessageHandler
	{
		internal long messageIdentity;
		internal Type messageType;
		internal bool isActivated;
		internal Delegate Handler { get { return handler; } }
		private Delegate handler;

		internal NetworkMessageHandler(long messageIdentity, Type messageType, Delegate handler)
		{
			this.messageIdentity = messageIdentity;
			this.messageType = messageType;
			this.handler = handler;
			isActivated = true;
		}

		internal void Activate()
		{
			isActivated = true;
		}

		internal void Deactivate()
		{
			isActivated = false;
		}

		internal void Handle(INetworkMessage msg)
		{
			if (isActivated && msg != null && msg.GetType() == messageType)
			{
				object[] invokeParams = new object[1];
				invokeParams[0] = msg;
				handler.Method.Invoke(handler.Target, invokeParams);
			}
		}
	}
}

