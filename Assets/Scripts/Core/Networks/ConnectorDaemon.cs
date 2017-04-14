using UnityEngine;
using System.Collections.Generic;
using System;

namespace Networks
{
	public class ConnectorDaemon : MonoBehaviour
	{
		public struct DaemonParameter
		{
			public bool sendHeartBeat;  // 此连接器是否发送心跳
			public float heartBeatInterval;	// 此连接器心跳间隔
			public bool autoReconnect;  // 此连接器是否需要断线重连
			public int autoReconnectRetryMax;   // 此连接器断线重连最大重试次数
			public float autoReconnectRetryInterval;   // 此连接器断线重连重试间隔时间（秒）

			public ConnectorEventHandler eventHandler;

			public DaemonParameter(bool sendHeartBeat, bool autoReconnect, ConnectorEventHandler eventHandler)
				: this(sendHeartBeat, 3f, autoReconnect, 3, 3f, eventHandler)
			{
			}

			public DaemonParameter(bool sendHeartBeat, float heartBeatInterval, bool autoReconnect, int autoReconnectRetryMax, float autoReconnectRetryInterval, ConnectorEventHandler eventHandler)
			{
				this.sendHeartBeat = sendHeartBeat;
				this.heartBeatInterval = heartBeatInterval;
				this.autoReconnect = autoReconnect;
				this.autoReconnectRetryMax = autoReconnectRetryMax;
				this.autoReconnectRetryInterval = autoReconnectRetryInterval;
				this.eventHandler = eventHandler;
			}
		}

#if UNITY_EDITOR
		public class WatchObject
#else
		internal class WatchObject
#endif
		{
			public IConnector connector = null;
			public bool connected = false;      // 守护线程里保存的连接器连接状态，主要用来触发回调

			public bool sendHeartBeat = false;  // 此连接器是否定时发送心跳
			public float heartBeatInterval = 3f;    // 此连接器心跳间隔
			public float lastHeartBeatTime = 0f;    // 此连接器上一次心跳时间
			public float lastHeartBeatReturnTime = 0f;  // 此连接器上一次收到心跳反馈的时间
			public bool autoReconnect = false;  // 此连接器是否需要断线重连
			public int autoReconnectRetryMax = 3;	// 此连接器断线重连最大重试次数
			public int autoReconnectRetryCount = 0;		// 此连接器当前已经重试的次数
			public float autoReconnectRetryInterval = 3f;	// 此连接器断线重连重试间隔时间（秒）
			public float autoReconnectLastRetryTime = 0f;	// 此连接器上一次重试的引擎时间（秒）
			public bool reconnect = false;      // 当前是否已经触发重连

			public RingBuffer<byte> receiveBuffer = new RingBuffer<byte>(4096);
			public RingBuffer<byte> sendBuffer = new RingBuffer<byte>(4096);

			public ConnectorEventHandler eventHandler;

			public void Init(IConnector connector, DaemonParameter initParam)
			{
				this.connector = connector;
				connected = connector.State == EConnectorState.ExpectConnected;

				sendHeartBeat = initParam.sendHeartBeat;
				heartBeatInterval = initParam.heartBeatInterval;
				autoReconnect = initParam.autoReconnect;
				autoReconnectRetryMax = initParam.autoReconnectRetryMax;
				autoReconnectRetryInterval = initParam.autoReconnectRetryInterval;
				eventHandler = initParam.eventHandler;
			}

			public void Clear()
			{
				connector = null;
				connected = false;

				sendHeartBeat = false;
				heartBeatInterval = 3f;
				lastHeartBeatTime = 0f;
				lastHeartBeatReturnTime = 0f;
				autoReconnect = false;
				autoReconnectRetryMax = 3;
				autoReconnectRetryCount = 0;
				autoReconnectRetryInterval = 3f;
				autoReconnectLastRetryTime = 0f;
				reconnect = false;

				eventHandler = null;
			}
		}

		public static ConnectorDaemon Instance { get { return _instance; } }
		private static ConnectorDaemon _instance = null;

		public float checkInterval = 0f;	// 在需要监控的连接器不是太多的情况下可以每帧都检查，否则可以设置这个参数让监控间隔拉长（秒）
		private float lastCheckTime = 0f;

		// 因为需要守护的连接不会太多，所以这里使用顺序表，如果数量增大需要考虑使用其他容器
#if UNITY_EDITOR
		public List<WatchObject> connectorsUnderWatchList = new List<WatchObject>();
#else
		internal List<WatchObject> connectorsUnderWatchList = new List<WatchObject>();
#endif
		private Queue<WatchObject> watchObjectPool = new Queue<WatchObject>();

		private byte[] heartBeatPack = new byte[8];
		private const byte heartBeatModuleID = 127;
		private const byte heartBeatFunctionID = 127;
		//private const byte heartBeatReturnFunctionID = 254;
		private const byte heartBeatReturnFunctionID = heartBeatFunctionID;
		private const float heartBeatTimeout = 3f;

		private void Awake()
		{
			if (Instance != null)
			{
				if (LogUtil.ShowWarning != null)
					LogUtil.ShowWarning("已经启用过了ConnectorDaemon，不能重复启用");
			}
			else
				_instance = this;

			// 初始化心跳数据包
			heartBeatPack[0] = 0xC8;
			heartBeatPack[1] = 0xEF;
			byte[] lenBytes = BitConverterEx.GetBytes((short)8, true);
			heartBeatPack[2] = lenBytes[0];
			heartBeatPack[3] = lenBytes[1];
			heartBeatPack[4] = 1; // 版本
			heartBeatPack[5] = 1; // 序列化类型，2-flatbuffers
			heartBeatPack[6] = heartBeatModuleID;
			heartBeatPack[7] = heartBeatFunctionID;
		}

		private void Update()
		{
			float now = Time.realtimeSinceStartup;
			if (now - lastCheckTime > checkInterval)
			{
				lastCheckTime = now;
				if (connectorsUnderWatchList.Count > 0)
					CheckAllConnectors();
			}
		}

		private WatchObject GetWatchObjectFromPool()
		{
			if (watchObjectPool.Count > 0)
				return watchObjectPool.Dequeue();
			else
				return new WatchObject();
		}

		private void ReturnWatchObjectToPool(WatchObject wo)
		{
			wo.Clear();
			watchObjectPool.Enqueue(wo);
		}

		private void CheckAllConnectors()
		{
			for (int i = 0; i < connectorsUnderWatchList.Count; ++i)
			{
				WatchObject wo = connectorsUnderWatchList[i];
				if (wo == null || wo.connector == null)     // 无效的WatchObject
					continue;
				CheckHeartBeat(wo);
				CheckConnectorState(wo);
				CheckConnectorReceive(wo);
				CheckConnectorSend(wo);
			}
		}

		private void CheckHeartBeat(WatchObject wo)
		{
			if (!wo.sendHeartBeat)
				return;
			// 连接器状态是连接上的，也没有处于重连过程中，那么可以检查心跳和发送心跳
			if (wo.connected && !wo.reconnect && wo.connector.State == EConnectorState.ExpectConnected)
			{
				// 只有发送过心跳了，才需要检查心跳反馈
				if (wo.lastHeartBeatTime > 0f)
				{
					if (wo.lastHeartBeatReturnTime <= wo.lastHeartBeatTime && Time.realtimeSinceStartup - wo.lastHeartBeatTime >= heartBeatTimeout)
					{
						// 心跳超时，认为连接已经断开
						ProcessDisconnectUnexpected(wo);
					}
				}
				// 再次检查连接器状态，如果一切正常，那么就可以检查并发送心跳
				if (wo.connected && !wo.reconnect && wo.connector.State == EConnectorState.ExpectConnected)
				{
					if(Time.realtimeSinceStartup - wo.lastHeartBeatTime >= wo.heartBeatInterval)
					{
						wo.connector.Send(heartBeatPack, 0, heartBeatPack.Length);
						wo.lastHeartBeatTime = Time.realtimeSinceStartup;
					}
				}
			}
		}

		private void CheckConnectorState(WatchObject wo)
		{
			if (!wo.connected)  // 如果上一次检查时连接器状态是断开状态
			{
				if (wo.connector.State == EConnectorState.ExpectHasIpAddress)
				{
					// 如果连接器已经解析好了IP地址，但是没有连接上，也没有在等待连接，那么认为连接器处在刚刚解析完域名等待调用连接的状态，调用连接
					wo.connector.BeginConnect();
				}
				else if (wo.connector.State == EConnectorState.ExpectConnected)
				{
					// 如果连接器上一次检测时还没有连接上，但是现在已经连接上了，那么认为连接器刚刚连接成功
					wo.connected = true;
					if (!wo.reconnect)
					{
						// 非重连的情况下，触发连接成功回调
						if (wo.eventHandler != null)
							wo.eventHandler.OnConnected();
					}
					else
					{
						// 重连状况下，触发重连成功回调，并退出重连流程
						wo.reconnect = false;
						if (wo.eventHandler != null)
							wo.eventHandler.OnReconnectedByDaemon();
					}
				}
				else if (wo.connector.State == EConnectorState.ExpectDisconnected || wo.connector.State == EConnectorState.Initial)
				{
					// 连接器处于断开状态，并且已进入重连流程，那么按照参数指定的周期尝试连接
					if (wo.reconnect)
					{
						if (wo.autoReconnectRetryCount >= wo.autoReconnectRetryMax)
						{
							// 已经超过最大重试次数，退出重连流程
							wo.reconnect = false;
							// 触发重连失败的回调
							if (wo.eventHandler != null)
								wo.eventHandler.OnReconnectFailed();
						}
						else if (Time.realtimeSinceStartup - wo.autoReconnectLastRetryTime > wo.autoReconnectRetryInterval)
						{
							// 重试间隔已到，发起重连，并记录这一次重试的时间，重试次数加1
							wo.autoReconnectLastRetryTime = Time.realtimeSinceStartup;
							++wo.autoReconnectRetryCount;
							wo.connector.BeginConnect();
						}
					}
				}
			}
			else    // 如果上一次检查时连接器状态是连接状态
			{
				// 判断连接意外断开改成用心跳判断
				//if (wo.connector.State == EConnectorState.ExpectConnected && !wo.connector.IsSending && !wo.connector.IsReceiving && !wo.connector.Connected)
				//{
				//	// 期望处于连接状态，但实际上并没有连接上，可以认为是意外断开了
				//	ProcessDisconnectUnexpected(wo);
				//}
				//else 
				if (wo.connector.State == EConnectorState.ExpectDisconnected || wo.connector.State == EConnectorState.Initial)
				{
					// 如果连接器上一次检测时是连接上的，但是现在连接断开了，那么认为连接器刚刚断开连接
					ProcessDisconnected(wo);
				}
			}
		}

		private void CheckConnectorReceive(WatchObject wo)
		{
			if (!wo.connector.IsReceiving)
			{
				// 连接器不在接收状态，那么将当前buffer中已经接收到的数据进行处理
				while (!wo.receiveBuffer.IsEmpty)
				{
					// 拆包，按完整消息返回给Handler
					// 寻找魔数
					if(!wo.receiveBuffer.DiscardToFirstIndexOf(0xC8, 0xEF))
						wo.receiveBuffer.DiscardAll();
					if(wo.receiveBuffer.Length >= 8)
					{
						// 找到了魔数，而且数据长度够一个消息头
						// 解析消息头
						byte[] lenBytes = new byte[2];
						wo.receiveBuffer.Peek(2, out lenBytes[0]);
						wo.receiveBuffer.Peek(3, out lenBytes[1]);
						short len = BitConverterEx.GetShort(lenBytes, 0, true);
						byte ver;
						wo.receiveBuffer.Peek(4, out ver);
						byte serializeType;
						wo.receiveBuffer.Peek(5, out serializeType);
						byte moduleID;
						wo.receiveBuffer.Peek(6, out moduleID);
						byte functionID;
						wo.receiveBuffer.Peek(7, out functionID);
						// TODO 区分协议版本号和序列化类型？
						// TODO 按模块号分发？
						// 收到心跳，记录下收到的时间
						if(moduleID == heartBeatModuleID && functionID == heartBeatReturnFunctionID)
							wo.lastHeartBeatReturnTime = Time.realtimeSinceStartup;
						// 消息头丢弃
						wo.receiveBuffer.Discard(8);
						// 如果存在协议体，那么尝试处理协议体
						if (len > 8)	
						{
							byte[] msgBytes = new byte[len - 8];
							if (wo.receiveBuffer.TryPull(msgBytes.Length, ref msgBytes))
								wo.eventHandler.OnReceive(moduleID, functionID, msgBytes);
							else
							{
								// 消息接收长度还不够，继续接收
								Debug.Log(Time.frameCount + " 继续接收1");
								break;
							}
						}
					}
					else
					{
						// 消息接收长度还不够，继续接收
						Debug.Log(Time.frameCount + " 继续接收2");
						break;
					}
				}
				if (wo.connected && wo.connector.State == EConnectorState.ExpectConnected && wo.connector.HasDataAvailable)
				{
					// 如果当前处于连接状态，就再次发起接收
					wo.connector.Receive(wo.receiveBuffer);
				}
			}
		}

		private void CheckConnectorSend(WatchObject wo)
		{
			if (wo.connected && wo.connector.State == EConnectorState.ExpectConnected && !wo.connector.IsSending)
			{
				wo.connector.Send();
			}
		}

		private void ProcessDisconnected(WatchObject wo)
		{
			wo.connected = false;
			if (!wo.reconnect)
			{
				// 非重连的情况下，触发断开连接成功的回调（重连状况下，不触发回调而是进入等待重连流程）
				if (wo.eventHandler != null)
					wo.eventHandler.OnDisconnected();
			}
			wo.lastHeartBeatTime = 0f;
			wo.lastHeartBeatReturnTime = 0f;
		}

		private void ProcessDisconnectUnexpected(WatchObject wo)
		{
			// 触发意外断开回调
			if (wo.eventHandler != null)
				wo.eventHandler.OnDisconnectedUnexpected();
			// 如果需要重连,标记重连
			if (wo.autoReconnect)
			{
				wo.reconnect = true;
				// 重置参数
				wo.autoReconnectRetryCount = 0;
				wo.autoReconnectLastRetryTime = Time.realtimeSinceStartup;
			}
			// 主动断开连接，以便标志位状态同步
			wo.connector.BeginDisconnect();
			wo.lastHeartBeatTime = 0f;
			wo.lastHeartBeatReturnTime = 0f;
		}

		public void DaemonIt(IConnector connector, DaemonParameter daemonParam)
		{
			for (int i = 0; i < connectorsUnderWatchList.Count; ++i)
			{
				WatchObject tmp = connectorsUnderWatchList[i];
				if (tmp != null && tmp.connector == connector)
				{
					if(LogUtil.ShowWarning != null)
						LogUtil.ShowWarning("这个connector已经处于守护之下，不能重复添加");
					return;
				}
			}
			WatchObject wo = GetWatchObjectFromPool();
			wo.Init(connector, daemonParam);
			connectorsUnderWatchList.Add(wo);
		}

		public void ReleaseIt(IConnector connector)
		{
			for (int i = 0; i < connectorsUnderWatchList.Count; ++i)
			{
				WatchObject tmp = connectorsUnderWatchList[i];
				if (tmp != null && tmp.connector == connector)
				{
					ReturnWatchObjectToPool(tmp);
					connectorsUnderWatchList.RemoveAt(i);
					break;
				}
			}
		}
	}
}
