using System;
using System.Collections.Generic;
using System.Reflection;

namespace Networks
{
	public class NetworkMessageCentralHub
	{
		public static NetworkMessageCentralHub Instance
		{
			get
			{
				if (_instance == null)
					_instance = new NetworkMessageCentralHub();
				return _instance;
			}
		}
		private static NetworkMessageCentralHub _instance = null;

		private Dictionary<long, List<NetworkMessageHandler>> messageHandlerTable = new Dictionary<long, List<NetworkMessageHandler>>();
		private Dictionary<NetworkMessageProcessUnitBase, Dictionary<long, List<NetworkMessageHandler>>> moduleMessageRegisterTable = 
			new Dictionary<NetworkMessageProcessUnitBase, Dictionary<long, List<NetworkMessageHandler>>>();

		#region Register & Unregister

		public void RegisterHandler<T>(int classID, int functionID, NetworkMessageProcessUnitBase module, NetworkMessageHandlerDelegate<T> handler) where T : INetworkMessage
		{
			Type msgType = typeof(T);
#if UNITY_EDITOR
			if (!msgType.IsValueType)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("网络消息结构体[{0}]必须定义为值类型（struct）", msgType.Name));
				return;
			}
			object[] fieldsDefined = msgType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fieldsDefined.Length <= 0)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("网络消息结构体[{0}]中没有定义任何成员，如果不需要向外通知任何参数的话，使用Notification", msgType.Name));
				return;
			}
#endif
			if (module != null && handler != null)
			{
				long identity = ((long)classID << 32) | (uint)functionID;
#if UNITY_EDITOR
				if (messageHandlerTable.ContainsKey(identity) && messageHandlerTable[identity].Count > 0 && messageHandlerTable[identity][0].messageType != msgType)
				{
					if (LogUtil.ShowError != null)
						LogUtil.ShowError(string.Format("网络消息[{0}-{1}]绑定了多个不同的消息结构体!", classID, functionID));
					return;
				}
#endif
				NetworkMessageHandler messageHandler = new NetworkMessageHandler(identity, msgType, handler);
				CollectionUtil.AddIntoTable(identity, messageHandler, messageHandlerTable);
				CollectionUtil.AddIntoTable(module, identity, messageHandler, moduleMessageRegisterTable);
#if UNITY_EDITOR
				if (LogUtil.ShowDebug != null)
					LogUtil.ShowDebug(string.Format("[网络消息中心]1.注册消息处理器：来自[{0}] 消息类型[{1}-{2}][{3}] 处理回调[{4}]", module.GetType().Name, classID, functionID, msgType.Name, StringUtility.ToString(handler)));
#endif
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("注册网络消息处理器时传递的参数有[null]值：消息标识[{0}-{1}] 参数类型[{2}] 模块[{3}] 处理回调[{4}]",
						classID, functionID, msgType.Name, module.GetType().Name, StringUtility.ToString(handler)));
			}
#endif
		}

		public void PauseAllHandlersBelongToModule(NetworkMessageProcessUnitBase module)
		{
			if (module != null)
			{
				Dictionary<long, List<NetworkMessageHandler>> tmpTable;
				if (moduleMessageRegisterTable.TryGetValue(module, out tmpTable))
					foreach (List<NetworkMessageHandler> handlerList in tmpTable.Values)
						for (int i = 0; i < handlerList.Count; ++i)
							handlerList[i].Deactivate();
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("暂停网络消息处理器时传递的参数有[null]值：模块[{0}]", module.GetType().Name));
			}
#endif
		}

		public void ResumeAllHandlersBelongToModule(NetworkMessageProcessUnitBase module)
		{
			if (module != null)
			{
				Dictionary<long, List<NetworkMessageHandler>> tmpTable;
				if (moduleMessageRegisterTable.TryGetValue(module, out tmpTable))
					foreach (List<NetworkMessageHandler> handlerList in tmpTable.Values)
						for(int i = 0; i < handlerList.Count; ++i)
							handlerList[i].Activate();
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("恢复网络消息处理器时传递的参数有[null]值：模块[{0}]", module.GetType().Name));
			}
#endif
		}

		public void UnregisterAllHandlersBelongToModule(NetworkMessageProcessUnitBase module)
		{
			if (module != null)
			{
				Dictionary<long, List<NetworkMessageHandler>> tmpTable;
				if (moduleMessageRegisterTable.TryGetValue(module, out tmpTable))
				{
					foreach (KeyValuePair<long, List<NetworkMessageHandler>> tmpKV in tmpTable)
					{
						long identity = tmpKV.Key;
						List<NetworkMessageHandler> tempList = tmpKV.Value;
						for(int i = 0; i < tempList.Count; ++i)
						{
							CollectionUtil.RemoveFromTable(identity, tempList[i], messageHandlerTable);
#if UNITY_EDITOR
							if (LogUtil.ShowDebug != null)
								LogUtil.ShowDebug(string.Format("[网络消息中心]2.注销消息处理器：来自[{0}] 消息类型[{1}] 处理回调[{2}]", module.GetType().Name, tempList[i].messageType.Name, StringUtility.ToString(tempList[i].Handler)));
#endif
						}
					}
					moduleMessageRegisterTable.Remove(module);
				}
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("注销网络消息处理器时传递的参数有[null]值：模块[{0}]", module.GetType().Name));
			}
#endif
		}

		#endregion

		#region Messaging 

		public virtual void DeliverMessage(ref RawNetworkMessage msg)
		{
			//msg.TimeStamp = DateTimeUtil.NowMillisecond;
			List<NetworkMessageHandler> tmpList;
			if (messageHandlerTable.TryGetValue(msg.Identity, out tmpList))
			{
				if(tmpList.Count > 0)
				{
					// 因为不允许相同消息使用不同结构体，所以随便取一个Handler里面的消息类型做反序列化即可
					Type msgType = tmpList[0].messageType;
					INetworkMessage msgInstance = (INetworkMessage)Activator.CreateInstance(msgType);
					ReflectionUtil.Deserialize(msgInstance, new BitConverterEx(msg.Content, 0, true), false);
					for(int i = 0; i < tmpList.Count; ++i)
						Invoke(tmpList[i], msgInstance);
				}
			}
			//else
			//{
			//	LogUtil.ShowWarning(string.Format("网络消息[{0}-{1}]没有模块处理", msg.ClassID, msg.FunctionID));
			//}
		}

		private void Invoke<T>(NetworkMessageHandler handler, T msg) where T : INetworkMessage
		{
			if (handler.isActivated)
				handler.Handle(msg);
		}

		#endregion
	}
}

