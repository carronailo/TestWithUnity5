using UnityEngine;
using System.Collections;
using System.Net;
using System.Collections.Generic;
using System;
using System.Threading;

namespace Networks.Temp
{
	// @TODO: 将对Connector的操作都收回到NetworCenter内，由NetworkCenter对外提供必要的接口
	public class NetworkCenter : MonoBehaviour, IEventHandler, IDataHandler
	{
		private struct ConnectorParameter
		{
			public Connector connector;
			public IDataHandler dataHandler;
			public IEventHandler eventHandler;
		}

		private enum EConnectorEvent
		{
			ON_CONNECT_SUCCEEDED,
			ON_CONNECT_FAILED,
			ON_DISCONNECT_SUCCEEDED,
			ON_DISCONNECT_FAILED,
			ON_CONNECTION_LOST,
			ON_RECONNECT_SUCCEEDED,
			ON_RECONNECT_FAILED,
			ON_DATASEND_SUCCEEDED,
			ON_DATASEND_FAILED,
			ON_DATARECEIVE_SUCCEEDED,
			ON_DATARECEIVE_FAILED,
		}

		private enum EConnectorDataEvent
		{
			ON_DATA_RECEIVED,
		}

		private struct ConnectorEvent
		{
			public EConnectorEvent eventID;
			public Connector ctx;
			public object extraParam;
		}

		private struct ConnectorDataEvent
		{
			public EConnectorDataEvent eventID;
			public Connector ctx;
			public object extraParam;
		}

		// @TODO: 可能需要以RPC路由地址来为Connector做映射
		private Dictionary<Connector, ConnectorParameter> connectorTable = null;

		private Queue<ConnectorEvent> connectorEventQueue = null;       // 连接事件队列，关键资源，需要多线程安全
		private bool eventQueueOccupied = false;            // 连接事件队列多线程安全访问的主线程信号位
		private ManualResetEvent eventQueueOccupiedSignal = null;       // 连接事件队列多线程安全访问的其他线程信号量
		private List<ConnectorEvent> eventProcessBuffer = null;     // 主线程保存一次性出队列的待处理事件的缓存，用于减少其他线程阻塞的时间（将处理事件的流程放到多线程安全锁之外，保证安全锁以内只包含对关键资源的访问）

		private Queue<ConnectorDataEvent> connectorDataEventQueue = null;       // 数据事件队列，关键资源，需要多线程安全
		private bool dataEventQueueOccupied = false;            // 数据事件队列多线程安全访问的主线程信号位
		private ManualResetEvent dataEventQueueOccupiedSignal = null;       // 数据事件队列多线程安全访问的其他线程信号量
		private List<ConnectorDataEvent> dataEventProcessBuffer = null;     // 主线程保存一次性出队列的待处理事件的缓存，用于减少其他线程阻塞的时间（将处理事件的流程放到多线程安全锁之外，保证安全锁以内只包含对关键资源的访问）

		private void Awake()
		{
			connectorTable = new Dictionary<Connector, ConnectorParameter>();

			connectorEventQueue = new Queue<ConnectorEvent>();
			eventQueueOccupiedSignal = new ManualResetEvent(true);
			eventProcessBuffer = new List<ConnectorEvent>();

			connectorDataEventQueue = new Queue<ConnectorDataEvent>();
			dataEventQueueOccupiedSignal = new ManualResetEvent(true);
			dataEventProcessBuffer = new List<ConnectorDataEvent>();
		}

		private void OnDestroy()
		{
			foreach (Connector con in connectorTable.Keys)
				con.Release();
			connectorTable.Clear();

			eventQueueOccupiedSignal.Reset();
			connectorEventQueue.Clear();
			eventQueueOccupiedSignal.Set();
			eventProcessBuffer.Clear();

			dataEventQueueOccupiedSignal.Reset();
			connectorDataEventQueue.Clear();
			dataEventQueueOccupiedSignal.Set();
			dataEventProcessBuffer.Clear();
		}

		private void Update()
		{
			ProcessEventQueue();
		}

		private void LateUpdate()
		{
			ProcessEventQueue();
		}

		/// <summary>
		/// 创建一个Socket连接，连接远程地址address，address的格式为“[IP地址或者Host地址]:[端口]”
		/// </summary>
		/// <param name="address"></param>
		public Connector MakeConnection(string address, EProtocolType protocol, IDataHandler dataHandler, IEventHandler eventHandler)
		{
			if (connectorTable == null)
				return null;
			if (string.IsNullOrEmpty(address))
				return null;
			string[] tmp = address.Split(':');
			if (tmp.Length != 2)
				return null;
			if (string.IsNullOrEmpty(tmp[0]))
				return null;
			Connector connector = new Connector();
			connector.ProtocolType = protocol;
			connector.EventHandler = this;
			connector.DataHandler = this;
			ConnectorParameter parameter = new ConnectorParameter { connector = connector, dataHandler = dataHandler, eventHandler = eventHandler };
			connectorTable[connector] = parameter;
			IPAddress ipAddress = null;
			if (!IPAddress.TryParse(tmp[0], out ipAddress))
				connector.Host = address;
			else
				connector.IP = address;
			connector.Start();
			return connector;
		}

		private void ProcessEventQueue()
		{
			// 这里是主线程，不能阻塞，所以通过标志位判断
			if (connectorEventQueue != null && eventProcessBuffer != null && !eventQueueOccupied)
			{
				eventQueueOccupiedSignal.Reset();       // 避免其他线程访问，加锁
				while (connectorEventQueue.Count > 0)
					eventProcessBuffer.Add(connectorEventQueue.Dequeue());
				eventQueueOccupiedSignal.Set();         // 为其他线程解锁
				for (int i = 0; i < eventProcessBuffer.Count; ++i)
					HandleConnectorEvent(eventProcessBuffer[i]);
				eventProcessBuffer.Clear();
			}
			if (connectorDataEventQueue != null && dataEventProcessBuffer != null && !dataEventQueueOccupied)
			{
				dataEventQueueOccupiedSignal.Reset();       // 避免其他线程访问，加锁
				while (connectorDataEventQueue.Count > 0)
					dataEventProcessBuffer.Add(connectorDataEventQueue.Dequeue());
				dataEventQueueOccupiedSignal.Set();         // 为其他线程解锁
				for (int i = 0; i < dataEventProcessBuffer.Count; ++i)
					HandleConnectorDataEvent(dataEventProcessBuffer[i]);
				dataEventProcessBuffer.Clear();
			}
		}

		private void HandleConnectorEvent(ConnectorEvent evt)
		{
			ConnectorParameter ctxParameter;
			if (connectorTable != null && connectorTable.TryGetValue(evt.ctx, out ctxParameter))
			{
				switch (evt.eventID)
				{
					case EConnectorEvent.ON_CONNECT_SUCCEEDED:
						if (ctxParameter.eventHandler != null)
							ctxParameter.eventHandler.OnConnected(evt.ctx);
						break;
					case EConnectorEvent.ON_CONNECT_FAILED:
						if (ctxParameter.eventHandler != null)
							ctxParameter.eventHandler.OnConnectFailed(evt.ctx, (string)evt.extraParam);
						break;
					case EConnectorEvent.ON_DISCONNECT_SUCCEEDED:
						if (ctxParameter.eventHandler != null)
							ctxParameter.eventHandler.OnDisconnected(evt.ctx);
						break;
					case EConnectorEvent.ON_DISCONNECT_FAILED:
						if (ctxParameter.eventHandler != null)
							ctxParameter.eventHandler.OnDisconnectFailed(evt.ctx, (string)evt.extraParam);
						break;
					case EConnectorEvent.ON_CONNECTION_LOST:
						if (ctxParameter.eventHandler != null)
							ctxParameter.eventHandler.OnConnectionLost(evt.ctx);
						break;
					case EConnectorEvent.ON_RECONNECT_SUCCEEDED:
						if (ctxParameter.eventHandler != null)
							ctxParameter.eventHandler.OnReconnected(evt.ctx);
						break;
					case EConnectorEvent.ON_RECONNECT_FAILED:
						if (ctxParameter.eventHandler != null)
							ctxParameter.eventHandler.OnReconnectFailed(evt.ctx, (string)evt.extraParam);
						break;
					case EConnectorEvent.ON_DATASEND_SUCCEEDED:
						if (ctxParameter.eventHandler != null)
							ctxParameter.eventHandler.OnDataSent(evt.ctx, (int)evt.extraParam);
						break;
					case EConnectorEvent.ON_DATASEND_FAILED:
						if (ctxParameter.eventHandler != null)
							ctxParameter.eventHandler.OnDataSendFailed(evt.ctx, (string)evt.extraParam);
						break;
					case EConnectorEvent.ON_DATARECEIVE_SUCCEEDED:
						if (ctxParameter.eventHandler != null)
							ctxParameter.eventHandler.OnDataReceived(evt.ctx, (int)evt.extraParam);
						break;
					case EConnectorEvent.ON_DATARECEIVE_FAILED:
						if (ctxParameter.eventHandler != null)
							ctxParameter.eventHandler.OnDataReceiveFailed(evt.ctx, (string)evt.extraParam);
						break;
					default:
						break;
				}
			}
		}

		private void HandleConnectorDataEvent(ConnectorDataEvent evt)
		{
			ConnectorParameter ctxParameter;
			if (connectorTable != null && connectorTable.TryGetValue(evt.ctx, out ctxParameter))
			{
				switch (evt.eventID)
				{
					case EConnectorDataEvent.ON_DATA_RECEIVED:
						if (ctxParameter.dataHandler != null)
							ctxParameter.dataHandler.OnDataReceived(evt.ctx, (byte[])evt.extraParam);
						break;
					default:
						break;
				}
			}
		}

		#region 仅限内部Connector使用的事件处理接口，主要用来将Connector触发的连接事件转到主线程处理

		private void SubmitEvent(EConnectorEvent evt, Connector ctx, object extraParam)
		{
			// 此接口通常由其他线程调用，所以需要添加信号阻塞机制
			eventQueueOccupiedSignal.WaitOne();         // 等待解锁
			eventQueueOccupiedSignal.Reset();       // 避免其他线程访问，加锁
			eventQueueOccupied = true;      // 避免主线程访问，加锁
			if (connectorEventQueue != null)
				connectorEventQueue.Enqueue(new ConnectorEvent { eventID = evt, ctx = ctx, extraParam = extraParam });
			eventQueueOccupied = false;     // 为主线程解锁
			eventQueueOccupiedSignal.Set();         // 为其他线程解锁
		}

		void IEventHandler.OnConnected(Connector ctx)
		{
			SubmitEvent(EConnectorEvent.ON_CONNECT_SUCCEEDED, ctx, null);
		}

		void IEventHandler.OnConnectFailed(Connector ctx, string error)
		{
			SubmitEvent(EConnectorEvent.ON_CONNECT_FAILED, ctx, error);
		}

		void IEventHandler.OnDisconnected(Connector ctx)
		{
			SubmitEvent(EConnectorEvent.ON_DISCONNECT_SUCCEEDED, ctx, null);
		}

		void IEventHandler.OnDisconnectFailed(Connector ctx, string error)
		{
			SubmitEvent(EConnectorEvent.ON_DISCONNECT_FAILED, ctx, error);
		}

		void IEventHandler.OnConnectionLost(Connector ctx)
		{
			SubmitEvent(EConnectorEvent.ON_CONNECTION_LOST, ctx, null);
		}

		void IEventHandler.OnReconnected(Connector ctx)
		{
			SubmitEvent(EConnectorEvent.ON_RECONNECT_SUCCEEDED, ctx, null);
		}

		void IEventHandler.OnReconnectFailed(Connector ctx, string error)
		{
			SubmitEvent(EConnectorEvent.ON_RECONNECT_FAILED, ctx, error);
		}

		void IEventHandler.OnDataSent(Connector ctx, int sendBytes)
		{
			SubmitEvent(EConnectorEvent.ON_DATASEND_SUCCEEDED, ctx, sendBytes);
		}

		void IEventHandler.OnDataSendFailed(Connector ctx, string error)
		{
			SubmitEvent(EConnectorEvent.ON_DATASEND_FAILED, ctx, error);
		}

		void IEventHandler.OnDataReceived(Connector ctx, int receiveBytes)
		{
			SubmitEvent(EConnectorEvent.ON_DATARECEIVE_SUCCEEDED, ctx, receiveBytes);
		}

		void IEventHandler.OnDataReceiveFailed(Connector ctx, string error)
		{
			SubmitEvent(EConnectorEvent.ON_DATARECEIVE_FAILED, ctx, error);
		}

		private void SubmitDataEvent(EConnectorDataEvent evt, Connector ctx, object extraParam)
		{
			// 此接口通常由其他线程调用，所以需要添加信号阻塞机制
			dataEventQueueOccupiedSignal.WaitOne();     // 等待解锁
			dataEventQueueOccupiedSignal.Reset();       // 避免其他线程访问，加锁
			dataEventQueueOccupied = true;      // 避免主线程访问，加锁
			if (connectorDataEventQueue != null)
				connectorDataEventQueue.Enqueue(new ConnectorDataEvent { eventID = evt, ctx = ctx, extraParam = extraParam });
			dataEventQueueOccupied = false;     // 为主线程解锁
			dataEventQueueOccupiedSignal.Set();       // 为其他线程解锁
		}

		void IDataHandler.OnDataReceived(Connector ctx, byte[] data)
		{
			SubmitDataEvent(EConnectorDataEvent.ON_DATA_RECEIVED, ctx, data);
		}

		#endregion

	}
}
