//#define USE_GENERIC

using System;

namespace MessageSystem
{
	public abstract class MessageProcessUnitBase
	{
		protected MessageProcessUnitBase()
		{
			OnRegisteringMessageHandler();
		}

		/// <summary>
		/// 需要显示调用Release来释放MessageProcessUnitBase占用的资源
		/// </summary>
		public void Release()
		{
			MessageCentralHub.Instance.UnregisterAllHandlersBelongToModule(this);
			NotificationCentralHub.Instance.UnregisterAllHandlersBelongToModule(this);
		}

		abstract protected void OnRegisteringMessageHandler();

#if USE_GENERIC
		protected void TryRegisterMessageHandler<T>(MessageHandlerDelegate<T> handler) where T : IMessage
#else
		protected void TryRegisterMessageHandler(MessageHandlerDelegate handler, Type msgType)
#endif
		{
			if (handler != null)
			{
				// Register to message central hub
#if USE_GENERIC
				MessageCentralHub.Instance.RegisterHandler(this, handler);
#else
				MessageCentralHub.Instance.RegisterHandler(this, handler, msgType);
#endif
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
#if USE_GENERIC
					LogUtil.ShowError(string.Format("模块[{0}]注册消息的调用中有无效参数：消息类型[{1}] 监视器委托[{2}]",
						GetType().Name, typeof(T).Name, StringUtil.ToString(handler)));
#else
					LogUtil.ShowError(string.Format("模块[{0}]注册消息的调用中有无效参数：消息类型[{1}] 监视器委托[{2}]",
						GetType().Name, msgType.Name, StringUtility.ToString(handler)));
#endif
			}
#endif
		}

		protected void TryRegisterNotificationHandler(int noticeID, NotificationHandlerDelegate handler)
		{
			if (handler != null)
			{
				// Register to notification central hub
				NotificationCentralHub.Instance.RegisterHandler(this, noticeID, handler);
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("模块[{0}]注册通知的调用中有无效参数：通知ID[{1}] 监视器委托[{2}]",
						GetType().Name, noticeID, StringUtility.ToString(handler)));
			}
#endif
		}
	}
}
