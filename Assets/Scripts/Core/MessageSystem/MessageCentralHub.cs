//#define USE_GENERIC
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MessageSystem
{
	internal class MessageCentralHub
	{
		private class RegisterModifier
		{
			public bool isUnregister = false;
			public Type messageType;
			public MessageHandler messageHandler;
			public MessageProcessUnitBase module;
		}

		private class RegisterModifierPool
		{
			private Queue<RegisterModifier> queue = null;

			public RegisterModifier Aquire()
			{
				if (queue == null)
					queue = new Queue<RegisterModifier>();
				if (queue.Count <= 0)
					return new RegisterModifier();
				else
					return queue.Dequeue();
			}

			public void Return(RegisterModifier item)
			{
				if (item == null)
					return;
				if (queue == null)
					queue = new Queue<RegisterModifier>();
				item.isUnregister = false;
				item.messageType = null;
				item.messageHandler = null;
				item.module = null;
				queue.Enqueue(item);
			}
		}


		public static MessageCentralHub Instance
		{
			get
			{
				if (_instance == null)
					_instance = new MessageCentralHub();
				return _instance;
			}
		}
		private static MessageCentralHub _instance = null;

		private Dictionary<Type, List<MessageHandler>> messageHandlerTable = null;
		private bool messageHandlerTableLocked = false;
		private List<RegisterModifier> messageHandlerRegisterChangeCache = null;

		private Dictionary<MessageProcessUnitBase, Dictionary<Type, MessageHandler>> moduleMessageRegisterTable = null;

		private RegisterModifierPool pool = null;

		private MessageCentralHub()
		{
			pool = new RegisterModifierPool();
		}

		#region Register & Unregister

#if USE_GENERIC
		public void RegisterHandler<T>(MessageProcessUnitBase module, MessageHandlerDelegate<T> handler) where T : IMessage
		{
			Type msgType = typeof(T);
#else
		public void RegisterHandler(MessageProcessUnitBase module, MessageHandlerDelegate handler, Type msgType)
		{
			if (!typeof(IMessage).IsAssignableFrom(msgType))
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("消息结构体[{0}]没有实现IMessage接口", msgType.Name));
				return;
			}
#endif
#if UNITY_EDITOR
			if (!msgType.IsValueType)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("消息结构体[{0}]必须定义为值类型（struct）", msgType.Name));
				return;
			}
			object[] fieldsDefined = msgType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fieldsDefined.Length <= 0)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("消息结构体[{0}]中没有定义任何成员，如果不需要向外通知任何参数的话，使用Notification", msgType.Name));
				return;
			}
#endif
			if (module != null && handler != null)
			{
				MessageHandler messageHandler = new MessageHandler(msgType, handler);
				if(!messageHandlerTableLocked)
				{
					if (messageHandlerTable == null)
						messageHandlerTable = new Dictionary<Type, List<MessageHandler>>();
					CollectionUtil.AddIntoTable(msgType, messageHandler, messageHandlerTable);
#if UNITY_EDITOR
					if (LogUtil.ShowDebug != null)
						LogUtil.ShowDebug(string.Format("[本地消息中心]1.注册消息处理器：来自[{0}] 消息类型[{1}] 处理回调[{2}]", module.GetType().Name, msgType.Name, StringUtility.ToString(handler)));
#endif
				}
				else
				{
					RegisterModifier modifier = pool.Aquire();
					modifier.isUnregister = false;
					modifier.messageType = msgType;
					modifier.messageHandler = messageHandler;
					modifier.module = module;
					if (messageHandlerRegisterChangeCache == null)
						messageHandlerRegisterChangeCache = new List<RegisterModifier>();
					messageHandlerRegisterChangeCache.Add(modifier);
				}
				if (moduleMessageRegisterTable == null)
					moduleMessageRegisterTable = new Dictionary<MessageProcessUnitBase, Dictionary<Type, MessageHandler>>();
				CollectionUtil.AddIntoTable(module, msgType, messageHandler, moduleMessageRegisterTable);
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("注册消息处理器时传递的参数有[null]值：消息类型[{0}] 模块[{1}] 处理回调[{2}]",
						msgType.Name, StringUtility.ToString(module), StringUtility.ToString(handler)));
			}
#endif
		}

		public void UnregisterAllHandlersBelongToModule(MessageProcessUnitBase module)
		{
			if (module != null)
			{
				if (moduleMessageRegisterTable != null)
				{
					Dictionary<Type, MessageHandler> handlerTable;
					if (moduleMessageRegisterTable.TryGetValue(module, out handlerTable))
					{
						if (messageHandlerTable != null)
							foreach (KeyValuePair<Type, MessageHandler> handlerKV in handlerTable)
							{
								if (!messageHandlerTableLocked)
								{
									CollectionUtil.RemoveFromTable(handlerKV.Key, handlerKV.Value, messageHandlerTable);
#if UNITY_EDITOR
									if (LogUtil.ShowDebug != null)
										LogUtil.ShowDebug(string.Format("[本地消息中心]2.注销消息处理器：来自[{0}] 消息类型[{1}] 处理回调[{2}]", module.GetType().Name, handlerKV.Key.Name, StringUtility.ToString(handlerKV.Value.Handler)));
#endif
								}
								else
								{
									RegisterModifier modifier = pool.Aquire();
									modifier.isUnregister = true;
									modifier.messageType = handlerKV.Key;
									modifier.messageHandler = handlerKV.Value;
									modifier.module = module;
									if (messageHandlerRegisterChangeCache == null)
										messageHandlerRegisterChangeCache = new List<RegisterModifier>();
									messageHandlerRegisterChangeCache.Add(modifier);
								}
							}
						moduleMessageRegisterTable.Remove(module);
					}
				}
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("注销消息处理器时传递的参数有[null]值：模块[{0}]", StringUtility.ToString(module)));
			}
#endif
		}

		public void PauseAllHandlersBelongToModule(MessageProcessUnitBase module)
		{
			if (module != null)
			{
				if (moduleMessageRegisterTable != null)
				{
					Dictionary<Type, MessageHandler> handlerTable;
					if (moduleMessageRegisterTable.TryGetValue(module, out handlerTable))
						foreach (MessageHandler handler in handlerTable.Values)
							handler.Deactivate();
				}
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("暂停消息处理器时传递的参数有[null]值：模块[{0}]", StringUtility.ToString(module)));
			}
#endif
		}

		public void ResumeAllHandlersBelongToModule(MessageProcessUnitBase module)
		{
			if (module != null)
			{
				if (moduleMessageRegisterTable != null)
				{
					Dictionary<Type, MessageHandler> handlerTable;
					if (moduleMessageRegisterTable.TryGetValue(module, out handlerTable))
						foreach (MessageHandler handler in handlerTable.Values)
							handler.Activate();
				}
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("恢复消息处理器时传递的参数有[null]值：模块[{0}]", StringUtility.ToString(module)));
			}
#endif
		}

		#endregion

		#region Messaging 

		//public virtual void DeliverMessage(Message msg)
		//{
		//}

#if USE_GENERIC
		public void DeliverMessage<T>(T msg) where T : IMessage
#else
		public void DeliverMessage(IMessage msg)
#endif
		{
			if (msg != null)
			{
				//msg.TimeStamp = DateTimeUtil.NowMillisecond;
				if (messageHandlerTable != null)
				{
					messageHandlerTableLocked = true;
					List<MessageHandler> handlers;
					if (messageHandlerTable.TryGetValue(msg.GetType(), out handlers))
					{
						foreach (MessageHandler handler in handlers)
							Invoke(handler, msg);
					}
					messageHandlerTableLocked = false;
					// 如果在消息处理的过程中有生成新的消息处理器或者销毁老的处理器，那么这些操作都被挂起并缓存到了列表里，下面要对这个缓存列表做处理
					if (messageHandlerRegisterChangeCache != null && messageHandlerRegisterChangeCache.Count > 0)
					{
						for (int i = 0; i < messageHandlerRegisterChangeCache.Count; ++i)
						{
							RegisterModifier modifier = messageHandlerRegisterChangeCache[i];
							if (modifier.isUnregister)
							{
								CollectionUtil.RemoveFromTable(modifier.messageType, modifier.messageHandler, messageHandlerTable);
#if UNITY_EDITOR
								if (LogUtil.ShowDebug != null)
									LogUtil.ShowDebug(string.Format("[本地消息中心]2.注销消息处理器：来自[{0}] 消息类型[{1}] 处理回调[{2}]", modifier.module.GetType().Name, modifier.messageType.Name, StringUtility.ToString(modifier.messageHandler.Handler)));
#endif
							}
							else
							{
								if (messageHandlerTable == null)
									messageHandlerTable = new Dictionary<Type, List<MessageHandler>>();
								CollectionUtil.AddIntoTable(modifier.messageType, modifier.messageHandler, messageHandlerTable);
#if UNITY_EDITOR
								if (LogUtil.ShowDebug != null)
									LogUtil.ShowDebug(string.Format("[本地消息中心]1.注册消息处理器：来自[{0}] 消息类型[{1}] 处理回调[{2}]", modifier.module.GetType().Name, modifier.messageType.Name, StringUtility.ToString(modifier.messageHandler.Handler)));
#endif
							}
							pool.Return(modifier);
						}
						messageHandlerRegisterChangeCache.Clear();
					}
				}
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("发送消息时传递的参数有[null]值：消息[{0}]", StringUtility.ToString(msg)));
			}
#endif
		}

//		public virtual void DeliverMessage<T>(T msg, MessageProcessUnitBase terminal) where T : Message
//		{
//			if (msg != null && terminal != null)
//			{
//				msg.TimeStamp = DateTimeUtil.NowMillisecond;
//				if(moduleMessageRegisterTable != null)
//					InternalDeliverMessage(msg, terminal);
//			}
//#if UNITY_EDITOR
//			else
//			{
//				if (LogUtil.ShowError != null)
//					LogUtil.ShowError(string.Format("发送消息时传递的参数有[null]值：消息[{0}] 接收端[{1}]",
//						StringUtil.ToString(msg), StringUtil.ToString(terminal)));
//			}
//#endif
//		}

//		public virtual void DeliverMessage<T>(T msg, IEnumerable<MessageProcessUnitBase> terminals) where T : Message
//		{
//			if (msg != null && terminals != null)
//			{
//				msg.TimeStamp = DateTimeUtil.NowMillisecond;
//				if (moduleMessageRegisterTable != null)
//					foreach (MessageProcessUnitBase terminal in terminals)
//						InternalDeliverMessage(msg, terminal);
//			}
//#if UNITY_EDITOR
//			else
//			{
//				if (LogUtil.ShowError != null)
//					LogUtil.ShowError(string.Format("发送消息时传递的参数有[null]值：消息[{0}] 接收端[{1}]",
//						StringUtil.ToString(msg), StringUtil.ToString(terminals)));
//			}
//#endif
//		}

//		public virtual void DeliverMessage<T1, T2>(T1 msg, IEnumerable<T2> terminals)
//			where T1 : Message
//			where T2 : MessageProcessUnitBase
//		{
//			if (msg != null && terminals != null)
//			{
//				msg.TimeStamp = DateTimeUtil.NowMillisecond;
//				if (moduleMessageRegisterTable != null)
//					foreach (MessageProcessUnitBase terminal in terminals)
//						InternalDeliverMessage(msg, terminal);
//			}
//#if UNITY_EDITOR
//			else
//			{
//				if (LogUtil.ShowError != null)
//					LogUtil.ShowError(string.Format("发送消息时传递的参数有[null]值：消息[{0}] 接收端[{1}]",
//						StringUtil.ToString(msg), StringUtil.ToString(terminals)));
//			}
//#endif
//		}

//		public virtual void DeliverMessageExclude<T>(T msg, MessageProcessUnitBase excludeTerminal) where T : Message
//		{
//			if (msg != null && excludeTerminal != null)
//			{
//				msg.TimeStamp = DateTimeUtil.NowMillisecond;
//				Type msgType = typeof(T);
//				if(messageHandlerTable != null)
//				{
//					List<MessageHandler> handlers;
//					if (messageHandlerTable.TryGetValue(msgType, out handlers))
//					{
//						if(moduleMessageRegisterTable != null)
//						{
//							MessageHandler excludeHandler = CollectionUtil.GetFromTable(excludeTerminal, msgType, moduleMessageRegisterTable);
//							foreach (MessageHandler handler in handlers)
//							{
//								if (handler != excludeHandler)
//									Invoke(handler, msg);
//							}
//						}
//					}
//				}
//			}
//#if UNITY_EDITOR
//			else
//			{
//				if (LogUtil.ShowError != null)
//					LogUtil.ShowError(string.Format("发送消息时传递的参数有[null]值：消息[{0}] 接收端[{1}]",
//						StringUtil.ToString(msg), StringUtil.ToString(excludeTerminal)));
//			}
//#endif
//		}

//		public virtual void DeliverMessage<T>(T msg, MessageProcessUnitBase terminal, 投递选项 option) where T : Message
//		{
//			switch (option)
//			{
//				case 投递选项.指定目标为投递对象:
//					DeliverMessage(msg, terminal);
//					break;
//				case 投递选项.指定目标为排除对象:
//					DeliverMessageExclude(msg, terminal);
//					break;
//			}
//		}

//private void InternalDeliverMessage<T>(T msg, MessageProcessUnitBase terminal) where T : Message
//{
//	Dictionary<Type, MessageHandler> handlers;
//	if (moduleMessageRegisterTable.TryGetValue(terminal, out handlers))
//	{
//		MessageHandler handler;
//		if (handlers.TryGetValue(msg.GetType(), out handler))
//			Invoke(handler, msg);
//	}
//}

#if USE_GENERIC
		private void Invoke<T>(MessageHandler handler, T msg) where T : IMessage
#else
		private void Invoke(MessageHandler handler, IMessage msg)
#endif
		{
			try
			{
				if (handler.isActivated)
					handler.Handle(msg);
			}
			catch(NullReferenceException ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
			}
			catch (ArgumentNullException ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
			}
			catch (IndexOutOfRangeException ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
			}
		}

#endregion
	}
}
