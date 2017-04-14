using System.Collections.Generic;

namespace MessageSystem
{
	public class NotificationCentralHub
	{
		public static NotificationCentralHub Instance
		{
			get
			{
				if (_instance == null)
					_instance = new NotificationCentralHub();
				return _instance;
			}
		}
		private static NotificationCentralHub _instance = null;

		private Dictionary<int, List<NotificationHandler>> notificationHandlerTable = new Dictionary<int, List<NotificationHandler>>();
		private Dictionary<MessageProcessUnitBase, Dictionary<int, NotificationHandler>> moduleNotificationRegisterTable = new Dictionary<MessageProcessUnitBase, Dictionary<int, NotificationHandler>>();

		#region Register & Unregister

		public void RegisterHandler(MessageProcessUnitBase module, int noticeID, NotificationHandlerDelegate handler)
		{
			if (module != null && handler != null)
			{
				NotificationHandler notificationHandler = new NotificationHandler(noticeID, handler);
				CollectionUtil.AddIntoTable(noticeID, notificationHandler, notificationHandlerTable);
				CollectionUtil.AddIntoTable(module, noticeID, notificationHandler, moduleNotificationRegisterTable);
#if UNITY_EDITOR
				if (LogUtil.ShowDebug != null)
					LogUtil.ShowDebug(string.Format("[本地通知中心]1.注册通知处理器：来自[{0}] 通知ID[{1}] 处理回调[{2}]", module.GetType().Name, noticeID, StringUtility.ToString(handler)));
#endif
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("注册通知处理器时传递的参数有[null]值：通知ID[{0}] 模块[{1}] 处理回调[{2}]",
						noticeID, StringUtility.ToString(module), StringUtility.ToString(handler)));
			}
#endif
		}

		public void PauseAllHandlersBelongToModule(MessageProcessUnitBase module)
		{
			if (module != null)
			{
				Dictionary<int, NotificationHandler> handlerTable;
				if (moduleNotificationRegisterTable.TryGetValue(module, out handlerTable))
				{
					foreach (NotificationHandler handler in handlerTable.Values)
						handler.Deactivate();
				}
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("暂停通知处理器时传递的参数有[null]值：模块[{0}]",
						StringUtility.ToString(module)));
			}
#endif
		}

		public void ResumeAllHandlersBelongToModule(MessageProcessUnitBase module)
		{
			if (module != null)
			{
				Dictionary<int, NotificationHandler> handlerTable;
				if (moduleNotificationRegisterTable.TryGetValue(module, out handlerTable))
				{
					foreach (NotificationHandler handler in handlerTable.Values)
						handler.Activate();
				}
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("恢复通知处理器时传递的参数有[null]值：模块[{0}]",
						StringUtility.ToString(module)));
			}
#endif
		}

		public void UnregisterAllHandlersBelongToModule(MessageProcessUnitBase module)
		{
			if (module != null)
			{
				Dictionary<int, NotificationHandler> handlerTable;
				if (moduleNotificationRegisterTable.TryGetValue(module, out handlerTable))
				{
					foreach (NotificationHandler handler in handlerTable.Values)
					{
						List<NotificationHandler> tmpList;
						if (notificationHandlerTable.TryGetValue(handler.noticeID, out tmpList))
						{
							tmpList.Remove(handler);
#if UNITY_EDITOR
							if (LogUtil.ShowDebug != null)
								LogUtil.ShowDebug(string.Format("[本地通知中心]2.注销通知处理器：来自[{0}] 通知ID[{1}] 处理回调[{2}]", module.GetType().Name, handler.noticeID, StringUtility.ToString(handler.Handler)));
#endif
						}
					}
					moduleNotificationRegisterTable.Remove(module);
				}
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("注销通知处理器时传递的参数有[null]值：模块[{0}]",
						StringUtility.ToString(module)));
			}
#endif
		}

		#endregion

		#region Messaging 

		public void DeliverNotification(int noticeID)
		{
			List<NotificationHandler> handlers;
			if (notificationHandlerTable.TryGetValue(noticeID, out handlers))
			{
				for(int i = 0; i < handlers.Count; ++i)
					Invoke(handlers[i], noticeID);
			}
		}

		public void DeliverNotification(int noticeID, MessageProcessUnitBase terminal)
		{
			if (terminal != null)
			{
				InternalDeliverNotification(noticeID, terminal);
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("发送通知时传递的参数有[null]值：通知ID[{0}] 接收端[{1}]",
						noticeID, StringUtility.ToString(terminal)));
			}
#endif
		}

		public void DeliverNotification(int noticeID, IEnumerable<MessageProcessUnitBase> terminals)
		{
			if (terminals != null)
			{
				foreach (MessageProcessUnitBase terminal in terminals)
					InternalDeliverNotification(noticeID, terminal);
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("发送通知时传递的参数有[null]值：通知ID[{0}] 接收端[{1}]",
						noticeID, StringUtility.ToString(terminals)));
			}
#endif
		}

		public void DeliverNotification<T>(int noticeID, IEnumerable<T> terminals)
			where T : MessageProcessUnitBase
		{
			if (terminals != null)
			{
				foreach (MessageProcessUnitBase terminal in terminals)
					InternalDeliverNotification(noticeID, terminal);
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("发送通知时传递的参数有[null]值：通知ID[{0}] 接收端[{1}]",
						noticeID, StringUtility.ToString(terminals)));
			}
#endif
		}

		public void DeliverNotificationExclude(int noticeID, MessageProcessUnitBase excludeTerminal)
		{
			if (excludeTerminal != null)
			{
				List<NotificationHandler> handlers;
				if (notificationHandlerTable.TryGetValue(noticeID, out handlers))
				{
					NotificationHandler excludeHandler = CollectionUtil.GetFromTable(excludeTerminal, noticeID, moduleNotificationRegisterTable);
					for(int i = 0; i < handlers.Count; ++i)
					{
						if (handlers[i] != excludeHandler)
							Invoke(handlers[i], noticeID);
					}
				}
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("发送通知时传递的参数有[null]值：消息[{0}] 接收端[{1}]",
						noticeID, StringUtility.ToString(excludeTerminal)));
			}
#endif
		}

		public void DeliverNotification(int noticeID, MessageProcessUnitBase terminal, 投递选项 option)
		{
			switch (option)
			{
				case 投递选项.指定目标为投递对象:
					DeliverNotification(noticeID, terminal);
					break;
				case 投递选项.指定目标为排除对象:
					DeliverNotificationExclude(noticeID, terminal);
					break;
			}
		}

		private void InternalDeliverNotification(int noticeID, MessageProcessUnitBase terminal)
		{
			Dictionary<int, NotificationHandler> handlers;
			if (moduleNotificationRegisterTable.TryGetValue(terminal, out handlers))
			{
				NotificationHandler handler;
				if (handlers.TryGetValue(noticeID, out handler))
					Invoke(handler, noticeID);
			}
		}

		private void Invoke(NotificationHandler handler, int noticeID)
		{
			if (handler.isActivated)
				handler.Handle(noticeID);
		}

		#endregion

	}
}

