//#define USE_GENERIC

using System;

namespace MessageSystem
{
#if USE_GENERIC
	public delegate void MessageHandlerDelegate<in T>(T message) where T : IMessage;
#else
	public delegate void MessageHandlerDelegate(IMessage message);
#endif

	internal class MessageHandler
	{
		internal Type messageType;
		internal bool isActivated;
#if USE_GENERIC
		internal Delegate Handler { get { return handler; } }
		internal Delegate handler;
#else
		internal MessageHandlerDelegate Handler { get { return handler; } }
		internal MessageHandlerDelegate handler;
#endif

#if USE_GENERIC
		internal MessageHandler(Type messageType, Delegate handler)
#else
		internal MessageHandler(Type messageType, MessageHandlerDelegate handler)
#endif
		{
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

#if USE_GENERIC
		internal void Handle<T>(T msg) where T : IMessage
		{
			if (isActivated && msg != null)
			{
				MessageHandlerDelegate<T> handler = (MessageHandlerDelegate<T>)this.handler;
				handler(msg);
			}
		}
#else
		internal void Handle(IMessage msg)
		{
			if (isActivated && msg != null)
				handler(msg);
		}
#endif
	}

}
