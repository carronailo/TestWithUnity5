using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Networks
{
	public interface IConnector
	{
		EConnectorState State { get; }
		bool Connected { get; }
		bool HasDataAvailable { get; }
		bool IsReceiving { get; }
		bool IsSending { get; }

		void BeginConnect(string ip, int port);
		void BeginConnectByHost(string host, int port);
		/// <summary>
		/// 请使用BeginConnect(ip, port)或者BeginConnectByHost(host, port)接口
		/// </summary>
		void BeginConnect();
		void BeginDisconnect();
		void Receive(RingBuffer<byte> dataPool);
		void Send(byte[] data, int offset, int length);
		void Send();
		void Release();
	}

	public enum EConnectorState
	{
		Initial,
		WaitingForDnsResolve,
		ExpectHasIpAddress,
		WaitingForConnect,
		ExpectConnected,
		WaitingForDisconnect,
		ExpectDisconnected,

		NotAvailable,
	}


	internal abstract class ConnectorBase : IConnector
	{
		protected class StateObject
		{
			public IPEndPoint ipEndPoint = null;
			public Socket workSocket = null;
			public EConnectorState state = EConnectorState.Initial;

			// 内部调用Socket时使用的发送和接收缓存
			public const int receiveBufferSize = 2048;
			public byte[] receiveBuffer = new byte[receiveBufferSize];
			public const int sendBufferSize = 4096;
			public byte[] sendBuffer = new byte[sendBufferSize];

			// 与外部交互用的数据池
			// （网络->socket->receiveBuffer->receiveDataPool->外部模块）
			// （外部模块->sendDataPool->sendBuffer->Socket->网络）
			// （两层缓存的存在主要用来处理接收时的粘包，以及发送繁忙时的缓冲）
			internal RingBuffer<byte> receivedDataPool = null;
			internal RingBuffer<byte> sendDataPool = new RingBuffer<byte>(sendBufferSize);

			public bool IsReceiving { get; set; }
			public bool IsSending { get; set; }

			public void Reset()
			{
				if (workSocket != null)
					workSocket.Close();
				workSocket = null;
				state = EConnectorState.Initial;

				// TODO 清理buffer

				IsReceiving = false;
				IsSending = false;
			}

			public void Release()
			{
				ipEndPoint = null;
				if (workSocket != null)
					workSocket.Close();
				workSocket = null;
				state = EConnectorState.Initial;

				// TODO 释放buffer

				IsReceiving = false;
				IsSending = false;
			}

			public bool IsAvailableForConnectRequest()
			{
				return state == EConnectorState.Initial || state == EConnectorState.ExpectDisconnected || state == EConnectorState.ExpectHasIpAddress;
			}

			public bool IsConnectingOrConnected()
			{
				return workSocket != null && (state == EConnectorState.WaitingForConnect || state == EConnectorState.ExpectConnected || state == EConnectorState.WaitingForDisconnect);
			}

			public bool IsAvailableForReceiveAndSend()
			{
				return state == EConnectorState.ExpectConnected;
			}
		}

		protected StateObject state = null;

		public EConnectorState State
		{
			get
			{
				return state == null ? EConnectorState.NotAvailable : state.state;
			}
		}

		public bool Connected
		{
			get
			{
				if (state == null)
					return false;
				if (state.workSocket == null)
					return false;
				if (!state.workSocket.Connected)
					return false;
				else if (state.workSocket.Poll(0, SelectMode.SelectRead))       // 这段代码大多数时候都好用，但有时会误报。由于Poll方法是阻塞的，所以这里用了0的参数，让Poll能立刻返回，不知道是不是跟这个参数有关
				{
					int nAvailable = state.workSocket.Available;
					if (nAvailable == 0)
						return false;
				}
				return true;
			}
		}

		public bool HasDataAvailable
		{
			get
			{
				if (state == null)
					return false;
				if (state.workSocket == null)
					return false;
				if (!state.workSocket.Connected)
					return false;
				return state.workSocket.Available > 0;
			}
		}


		public bool IsReceiving
		{
			get
			{
				return state == null ? false : state.IsReceiving;
			}
		}

		public bool IsSending
		{
			get
			{
				return state == null ? false : state.IsSending;
			}
		}

		protected abstract Socket CreateSocket();

		public virtual void BeginConnect(string ip, int port)
		{
			if (state == null)
				state = new StateObject();
			if (!state.IsAvailableForConnectRequest())
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("连接器状态错误，无法开始新的连接，当前状态：" + state.state);
				return;
			}

			IPAddress ipAddress = null;
			if(!IPAddress.TryParse(ip, out ipAddress) || ipAddress == null)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("无法解析" + ip);
				return;
			}
			if(port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("端口无效" + port);
				return;
			}

			IPEndPoint ipEndPoint = null;
			try
			{
				ipEndPoint = new IPEndPoint(ipAddress, port);
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
			}
			if (ipEndPoint == null)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("无法解析" + ip + ":" + port);
				return;
			}

			state.ipEndPoint = ipEndPoint;
			state.state = EConnectorState.ExpectHasIpAddress;

			BeginConnect();
		}

		public virtual void BeginConnectByHost(string host, int port)
		{
			if (state == null)
				state = new StateObject();
			if (!state.IsAvailableForConnectRequest())
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("连接器状态错误，无法开始新的连接，当前状态：" + state.state);
				return;
			}

			if (string.IsNullOrEmpty(host) || port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("地址无效");
				return;
			}

			IPEndPoint ipEndPoint = null;
			try
			{
				ipEndPoint = new IPEndPoint(IPAddress.None, port);
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
			}
			if (ipEndPoint == null)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("无法解析" + host + ":" + port);
				return;
			}

			state.ipEndPoint = ipEndPoint;

			BeginResolveHost(host);
		}

		public virtual void BeginConnect()
		{
			if (state == null)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("无法使用一个空的连接器");
				return;
			}
			else if(!state.IsAvailableForConnectRequest())
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("连接器状态错误，当前状态：" + state.state);
				return;
			}
			else if (state.ipEndPoint == null)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("未设置连接地址");
				return;
			}

			if (state.workSocket == null)
				state.workSocket = CreateSocket();
			else if (state.workSocket.Connected)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("此connector的连接尚未断开，无法发起新的连接。连接器状态未同步？！");
				return;
			}

			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("开始连接" + state.ipEndPoint.ToString());

			try
			{
				state.state = EConnectorState.WaitingForConnect;
				state.workSocket.BeginConnect(state.ipEndPoint, new AsyncCallback(BeginConnectCallback), state);
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
				state.Reset();
			}
		}

		/// <summary>
		/// 调用此接口之前，先确保连接器没有使用ConnectorDaemon的断线重连，否则连接可能无法正常断开
		/// </summary>
		public virtual void BeginDisconnect()
		{
			if (state == null)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("无法使用一个空的连接器");
				return;
			}
			else if (state.workSocket == null)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("无法断开一个空的Socket连接");
				return;
			}

			if (state.state == EConnectorState.WaitingForDisconnect)
			{
				if (LogUtil.ShowWarning != null)
					LogUtil.ShowWarning("此connector已经发起断开连接，无需重复断开");
				return;
			}
			else if (state.state == EConnectorState.WaitingForConnect)
			{
				if (LogUtil.ShowWarning != null)
					LogUtil.ShowWarning("此connector正在发起连接，此操作将中止发起中的连接");
				state.Reset();
				return;
			}
			else if (!state.workSocket.Connected)
			{
				if (LogUtil.ShowWarning != null)
					LogUtil.ShowWarning("此connector并未连接，无需断开");
				state.Reset();
				return;
			}

			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("开始断开连接" + state.ipEndPoint.ToString());

			try
			{
				state.workSocket.Shutdown(SocketShutdown.Both);
				state.state = EConnectorState.WaitingForDisconnect;
				state.workSocket.BeginDisconnect(false, new AsyncCallback(BeginDisconnectCallback), state);    // 注意，即便是reuse参数设为true，此socket也无法再次连接同一个EndPoint，所以还是不要设置重用了
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
				state.Reset();
			}
		}

		protected virtual void BeginResolveHost(string host)
		{
			if (state == null)
				state = new StateObject();
			if (!state.IsAvailableForConnectRequest())
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("连接器状态错误，当前状态：" + state.state);
				return;
			}

			if (string.IsNullOrEmpty(host))
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("无法解析一个空地址");
				return;
			}

			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("开始解析" + host);

			try
			{
				state.state = EConnectorState.WaitingForDnsResolve;
				Dns.BeginGetHostAddresses(host, new AsyncCallback(BeginGetHostAddressesCallback), state);
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
				state.Reset();
			}
		}

		public virtual void Receive(RingBuffer<byte> dataPool)
		{
			if (state == null || state.workSocket == null)
				return;
			if (!state.IsAvailableForReceiveAndSend())
			{
				if (LogUtil.ShowWarning != null)
					LogUtil.ShowWarning("连接器状态错误，无法接收网络数据，当前状态：" + state.state);
				return;
			}
			if(IsReceiving)
			{
				if (LogUtil.ShowWarning != null)
					LogUtil.ShowWarning("连接器正在接收中，请等待接收完毕");
				return;
			}

			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("开始接收");

			try
			{
				state.IsReceiving = true;
				state.receivedDataPool = dataPool;
				state.workSocket.BeginReceive(state.receiveBuffer, 0, StateObject.receiveBufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
			}
			catch(Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
				//state.Reset();		// 接收操作即便是失败也不应该改变连接器状态，连接状态由连接和断开连接操作
				state.IsReceiving = false;
			}
		}

		public virtual void Send(byte[] data, int offset, int length)
		{
			if (state == null || state.workSocket == null)
				return;
			if (!state.IsAvailableForReceiveAndSend())
			{
				if (LogUtil.ShowWarning != null)
					LogUtil.ShowWarning("连接器状态错误，无法接收网络数据，当前状态：" + state.state);
				return;
			}

			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("缓存发送数据");

			// 上一次发送数据还没有结束，将数据放入发送缓存
			if (!state.sendDataPool.TryPush(data, offset, length))
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("缓存发送数据失败");
				return;
			}

			if (!IsSending)
			{
				if (LogUtil.ShowDebug != null)
					LogUtil.ShowDebug("开始发送");

				int sendBytes = state.sendDataPool.Length;
				if (!state.sendDataPool.TryPull(sendBytes, ref state.sendBuffer))
				{
					state.sendDataPool.Clear();
					return;
				}

				// 当前空闲，那么立刻发送
				try
				{
					state.IsSending = true;
					state.workSocket.BeginSend(state.sendBuffer, 0, sendBytes, SocketFlags.None, new AsyncCallback(SendCallback), state);
				}
				catch (Exception ex)
				{
					if (LogUtil.ShowException != null)
						LogUtil.ShowException(ex);
					//state.Reset();		// 发送操作即便是失败也不应该改变连接器状态，连接状态由连接和断开连接操作
					state.IsSending = false;
				}
			}
		}

		public virtual void Send()
		{
			if (state == null || state.workSocket == null)
				return;
			if (!state.IsAvailableForReceiveAndSend())
			{
				if (LogUtil.ShowWarning != null)
					LogUtil.ShowWarning("连接器状态错误，无法接收网络数据，当前状态：" + state.state);
				return;
			}

			if (IsSending)
				return;
			if (state.sendDataPool.IsEmpty)
				return;

			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("开始发送缓存数据");

			int sendBytes = state.sendDataPool.Length;
			if (!state.sendDataPool.TryPull(sendBytes, ref state.sendBuffer))
			{
				state.sendDataPool.Clear();
				return;
			}

			try
			{
				state.IsSending = true;
				state.workSocket.BeginSend(state.sendBuffer, 0, sendBytes, SocketFlags.None, new AsyncCallback(SendCallback), state);
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
				//state.Reset();		// 发送操作即便是失败也不应该改变连接器状态，连接状态由连接和断开连接操作
				state.IsSending = false;
			}
		}

		public virtual void Release()
		{
			if (state == null || state.workSocket == null)
				return;

			if (state.workSocket.Connected)
				if (LogUtil.ShowWarning != null)
					LogUtil.ShowWarning("释放connector资源之前，最好先断开连接，否则socket中缓存的接收数据和待发送数据会丢失");

			state.workSocket.Close();
			state.Release();
		}

		#region 异步回调

		protected virtual void BeginGetHostAddressesCallback(IAsyncResult ar)
		{
			if (ar == null)
				return;

			IPAddress[] ipAddresses = Dns.EndGetHostAddresses(ar);

			StateObject s = (StateObject)ar.AsyncState;
			if (s == null)
				return;
			if (s != state)
			{
				// 回调里的Socket不是此Connector下属的Socket，需要关闭并释放
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("异步参数错误，传递的StateObject前后不一致");
				s.Release();
				return;
			}

			if (s.state != EConnectorState.WaitingForDnsResolve)
			{
				if (LogUtil.ShowWarning != null)
					LogUtil.ShowWarning("连接器状态错误，后续操作取消，期望状态：WaitingForDnsResolve，当前状态：" + state.state);
				return;
			}

			if (ipAddresses != null && ipAddresses.Length > 0)
			{
				try
				{
					if (s.ipEndPoint == null)
						s.ipEndPoint = new IPEndPoint(ipAddresses[0], IPEndPoint.MinPort);
					else
						s.ipEndPoint.Address = ipAddresses[0];
					s.state = EConnectorState.ExpectHasIpAddress;

					if (LogUtil.ShowDebug != null)
						LogUtil.ShowDebug("DNS解析成功" + s.ipEndPoint.Address.ToString());
				}
				catch(Exception ex)
				{
					if (LogUtil.ShowException != null)
						LogUtil.ShowException(ex);
					s.Reset();
				}
			}
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("DNS解析错误");
				s.Reset();
			}

		}

		protected virtual void BeginConnectCallback(IAsyncResult ar)
		{
			if (ar == null)
				return;
			StateObject s = (StateObject)ar.AsyncState;
			if (s == null || s.workSocket == null)
				return;

			try
			{
				s.workSocket.EndConnect(ar);

				if (s != state)
				{
					// 回调里的Socket不是此Connector下属的Socket，需要关闭并释放
					if (LogUtil.ShowError != null)
						LogUtil.ShowError("异步参数错误，传递的StateObject前后不一致");
					s.Release();
					return;
				}

				s.state = EConnectorState.ExpectConnected;

				if (LogUtil.ShowDebug != null)
					LogUtil.ShowDebug("连接成功" + state.ipEndPoint.ToString());
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
				if(ex is ObjectDisposedException && s.state == EConnectorState.ExpectDisconnected)
				{
					// socket被关闭，而且是由BeginDisconnect发起的（目的就是停止执行中的的BeginConnect），那么不需要释放资源，保持此状态
				}
				else
				{
					s.Reset();
				}
			}
		}

		protected virtual void BeginDisconnectCallback(IAsyncResult ar)
		{
			if (ar == null)
				return;
			StateObject s = (StateObject)ar.AsyncState;
			if (s == null || s.workSocket == null)
				return;

			try
			{
				s.workSocket.EndDisconnect(ar);

				if (s != state)
				{
					// 回调里的Socket不是此Connector下属的Socket，需要关闭并释放
					if (LogUtil.ShowError != null)
						LogUtil.ShowError("异步参数错误，传递的StateObject前后不一致");
					s.Release();
					return;
				}

				if (s.state != EConnectorState.WaitingForDisconnect)
				{
					if (LogUtil.ShowWarning != null)
						LogUtil.ShowWarning("连接器状态错误，后续操作取消，期望状态：WaitingForDisconnect，当前状态：" + state.state);
					return;
				}

				// 因为不能重用socket连接相同的EndPoint，所以重用没有什么意义，断开连接之后就释放掉socket资源
				s.workSocket.Close();
				s.workSocket = null;
				s.state = EConnectorState.ExpectDisconnected;
				s.IsReceiving = false;
				s.IsSending = false;

				if (LogUtil.ShowDebug != null)
					LogUtil.ShowDebug("断开连接成功" + state.ipEndPoint.ToString());
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
				s.Reset();
			}
		}

		protected virtual void ReceiveCallback(IAsyncResult ar)
		{
			if (ar == null)
				return;
			StateObject s = (StateObject)ar.AsyncState;
			if (s == null || s.workSocket == null)
				return;

			try
			{
				int bytesRead = s.workSocket.EndReceive(ar);

				if (s != state)
				{
					// 回调里的Socket不是此Connector下属的Socket，需要关闭并释放
					if (LogUtil.ShowError != null)
						LogUtil.ShowError("异步参数错误，传递的StateObject前后不一致");
					s.Release();
					return;
				}

				if(bytesRead > 0)
				{
					// 保存当前读取到的字节
					if (s.receivedDataPool != null)
						s.receivedDataPool.TryPush(s.receiveBuffer, 0, bytesRead);		// 如果保存出错，那么这些数据将会丢失

					if (LogUtil.ShowDebug != null)
						LogUtil.ShowDebug("接收到" + bytesRead + "字节");
				}
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
			}
			finally
			{
				s.receivedDataPool = null;
				s.IsReceiving = false;
			}
		}

		protected virtual void SendCallback(IAsyncResult ar)
		{
			if (ar == null)
				return;
			StateObject s = (StateObject)ar.AsyncState;
			if (s == null || s.workSocket == null)
				return;

			try
			{
				int bytesSent = s.workSocket.EndSend(ar);

				if (s != state)
				{
					// 回调里的Socket不是此Connector下属的Socket，需要关闭并释放
					if (LogUtil.ShowError != null)
						LogUtil.ShowError("异步参数错误，传递的StateObject前后不一致");
					s.Release();
					return;
				}

				if (LogUtil.ShowDebug != null)
					LogUtil.ShowDebug("发送完毕，此次发送" + bytesSent + "字节");

				if(s.sendDataPool.IsEmpty)
				{
					s.IsSending = false;
					return;
				}

				if (LogUtil.ShowDebug != null)
					LogUtil.ShowDebug("开始发送缓存数据");

				int sendBytes = s.sendDataPool.Length;
				if (!s.sendDataPool.TryPull(sendBytes, ref s.sendBuffer))
				{
					s.sendDataPool.Clear();
					s.IsSending = false;
					return;
				}

				s.workSocket.BeginSend(s.sendBuffer, 0, sendBytes, SocketFlags.None, new AsyncCallback(SendCallback), s);
				s.IsSending = true;
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(ex);
				s.IsSending = false;
			}
		}

		#endregion
	}

	internal class TCPConnector : ConnectorBase
	{
		protected override Socket CreateSocket()
		{
			return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}
	}

	internal class UDPConnector : ConnectorBase
	{
		protected override Socket CreateSocket()
		{
			return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
		}
	}

}
