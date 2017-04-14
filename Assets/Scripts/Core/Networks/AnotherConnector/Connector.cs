using System.Net;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Networks.Temp
{
	public class Connector
	{
		protected const int receiveBufferMaxSize = 8192;	// Socket接收缓存最大容量，实际使用时会从socket的receiveBufferSize和此值之间选取一个较小值作为缓存大小
		protected const int sendBufferMaxSize = 8192;	// Socket发送缓存最大容量，实际使用时会从socket的sendBufferSize和此值之间选取一个较小值作为缓存大小
		protected const int daemonLoopInterval = 50;	// 守护线程每次监护间隔，单位毫秒

		protected class StateObject
		{
			public Connector context = null;

			public IPEndPoint ipEndPoint = null;
			public Socket workSocket = null;

			public volatile EConnectorState state = EConnectorState.Initial;
			public volatile bool isReceiving = false;
			public volatile bool isSending = false;

			// 内部调用Socket时使用的发送和接收缓存
			public byte[] receiveBuffer = null;
			public byte[] sendBuffer = null;
			public RingBuffer<byte> sendCache = null;
			public RingBuffer<byte> receiveCache = null;

			public StateObject(Connector context, byte[] receiveBuffer, byte[] sendBuffer, RingBuffer<byte> receiveCache, RingBuffer<byte> sendCache)
			{
				this.context = context;
				this.receiveBuffer = receiveBuffer;
				this.sendBuffer = sendBuffer;
				this.receiveCache = receiveCache;
				this.sendCache = sendCache;
			}

			public void Reset()
			{
				if (workSocket != null)
					workSocket.Close();
				workSocket = null;
				state = EConnectorState.Initial;

				isReceiving = false;
				isSending = false;

				if (sendCache != null)
					sendCache.Clear();
				if (receiveCache != null)
					receiveCache.Clear();
			}

			public void Release()
			{
				ipEndPoint = null;
				Reset();
			}

			public bool IsAvailableForConnectRequest()
			{
				return state == EConnectorState.Initial || state == EConnectorState.ExpectDisconnected || state == EConnectorState.ExpectHasIpAddress;
			}

			public bool IsAvailableForDisconnectRequest()
			{
				return state != EConnectorState.WaitingForDisconnect;
			}

			public bool IsConnectingOrConnected()
			{
				return workSocket != null && (state == EConnectorState.WaitingForConnect || state == EConnectorState.ExpectConnected || state == EConnectorState.WaitingForDisconnect);
			}

			public bool IsDisconnected()
			{
				return state == EConnectorState.ExpectDisconnected || state == EConnectorState.Initial;
			}

			public bool IsAvailableForReceiveAndSend()
			{
				return state == EConnectorState.ExpectConnected;
			}

			public bool HasDataAvailable
			{
				get
				{
					if (workSocket == null)
						return false;
					if (!workSocket.Connected)
						return false;
					return workSocket.Available > 0;
				}
			}
		}

		protected class DaemonParameter
		{
			public Connector daemonObject;
			public bool isConnected;
			public bool isReconnecting;
			public int autoReconnectRetryCount;
			public long autoReconnectLastTime;
			public long lastHeartBeatTime;
			public long lastHeartBeatReturnTime;
		}

		#region 基本参数变量

		// Host和IP和IPEndPoint三选一，后设置的参数会替代先设置的，Host和IP的字符串后面可以跟":xxxx"形式的端口号，会自动解析到Port参数上，但是如果之后再次设置Port参数则会覆盖掉此端口
		// 设置Host参数的时候会自动将IP和Port置空，并会重新生成IPEndPoint
		public string Host
		{
			get
			{
				return host;
			}
			set
			{
				ip = string.Empty;
				Port = 0;
				host = value;
			}
		}
		protected string host;

		//设置IP参数的时候会自动将Host和Port置空，并会重新生成IPEndPoint
		public string IP
		{
			get
			{
				return ip;
			}
			set
			{
				host = string.Empty;
				Port = 0;
				ip = value;
			}
		}
		protected string ip;

		// 设置Port参数会覆盖IPEndPoint里的端口，但不会影响IP地址
		public int Port { get; set; }

		// 网络协议类型，支持最基础的TCP和UDP，其他协议等待扩展
		public EProtocolType ProtocolType { get; set; }

		// 连接事件的处理器
		public IEventHandler EventHandler
		{
			get
			{
				if (eventHandler == null)
					eventHandler = new EventHandlerBase();
				return eventHandler;
			}
			set
			{
				eventHandler = value;
			}
		}
		protected IEventHandler eventHandler = null;

		// 内部调用Socket时使用的发送和接收缓存，应该是多线程安全的容器，但是因为不能阻塞访问此容器的线程，所以转为使用State Object中的标识位来确保多线程安全
		protected byte[] receiveBuffer = null;
		protected byte[] sendBuffer = null;

		// 在繁忙时用来缓存发送请求，大小应该确保可以容纳系统定义的最大尺寸协议（最好是1.5倍到2倍），应该是多线程安全的容器，但是因为不能阻塞访问此容器的线程，所以转为使用State Object中的标识位来确保多线程安全
		public RingBuffer<byte> sendCache = new RingBuffer<byte>(ProtocolDefine.maximumProtocolLength * 2);
		// 为处理消息残缺和粘包而设置的缓存，大小应该确保可以容纳系统定义的最大尺寸协议（最好是1.5倍到2倍），应该是多线程安全的容器，但是因为不能阻塞访问此容器的线程，所以转为使用State Object中的标识位来确保多线程安全
		public RingBuffer<byte> receiveCache = new RingBuffer<byte>(ProtocolDefine.maximumProtocolLength * 2);

		// 数据接收的处理器
		public IDataHandler DataHandler
		{
			get
			{
				if (dataHandler == null)
					dataHandler = new DataHandlerBase();
				return dataHandler;
			}
			set
			{
				dataHandler = value;
			}
		}
		protected IDataHandler dataHandler = null;

		#endregion

		#region 心跳参数

		// 是否支持心跳，默认支持
		public bool SupportHeartBeat { get; set; }
		// 如果支持心跳，那么每一次心跳的时间间隔是多少毫秒？默认3000毫秒，可以修改
		public int HeartBeatInterval { get; set; }
		// 如果支持心跳，那么每一次等待网络心跳反馈的超时时间是多少毫秒？默认3000毫秒，可以修改
		public int HeartBeatReturnTimeout { get; set; }
		// 心跳消息对应的消息ID-ModuleID，默认127，可以修改
		public byte HeartBeatModuleID { get; set; }
		// 心跳消息对应的消息ID-FunctionID，默认127，可以修改
		public byte HeartBeatFunctionID { get; set; }

		#endregion

		#region 断线重连参数

		// 是否支持断线重连，默认支持
		public bool SupportAutoReconnect { get; set; }
		// 如果支持断线重连，那么重连的最大尝试次数是多少次？默认5次，可以修改
		public int ReconnectMaxRetry { get; set; }
		// 如果支持断线重连，那么每一次重连的超时时间是多少毫秒？默认5000毫秒，可以修改
		public int ReconnectRetryTimeout { get; set; }

		#endregion

		protected StateObject state = null;
		protected Thread daemonThread = null;
		//protected ManualResetEvent daemonSuspend = null;

		protected volatile bool released = false;

		public Connector()
		{
			SupportHeartBeat = true;
			HeartBeatInterval = 3000;
			HeartBeatReturnTimeout = 3000;
			HeartBeatModuleID = 127;
			HeartBeatFunctionID = 127;

			SupportAutoReconnect = true;
			ReconnectMaxRetry = 5;
			ReconnectRetryTimeout = 5000;
		}

		// 启用Connector，需保证各项必要参数都已经设置完毕，否则抛出异常
		public virtual void Start()
		{
			if (string.IsNullOrEmpty(Host) && string.IsNullOrEmpty(IP))
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("必要参数没有设置");
				return;
			}

			if (state == null)
				state = new StateObject(this, receiveBuffer, sendBuffer, receiveCache, sendCache);
			else
				state.Release();    // 确保连接器状态是处于初始化状态

			// 开启监控线程
			PrepareDaemonThread();
			// 发起连接
			StartConnect();
		}

		// 停用Connector，已经停用的Connector可以再次启用
		public virtual void Stop()
		{
			// @TODO: 如果发送缓存里还有等待发送的消息，那么应该等待都发送完再断开连接
			Disconnect();

			//if (daemonSuspend != null)
			//	daemonSuspend.Reset();
		}

		// 释放Connector所持有的所有资源，释放后的Connector无法再次重用
		public virtual void Release()
		{
			if (state != null)
				state.Release();

			released = true;
		}

		// 使用Connector发送数据
		public virtual void Send(byte[] data)
		{
			if(data != null)
				Send(data, 0, data.Length);
		}

		// 使用Connector发送数据，可以从一个数据序列中挑选特定部分发送
		public virtual void Send(byte[] data, int offset, int length)
		{
			SendData(data, offset, length);
		}

		protected virtual void PrepareDaemonThread()
		{
			//if (daemonSuspend == null)
			//	daemonSuspend = new ManualResetEvent(true);
			//else
			//	daemonSuspend.Set();

			if (daemonThread == null)
			{
				daemonThread = new Thread(new ParameterizedThreadStart(DaemonProcedure));
				daemonThread.IsBackground = true;       // 设置守护线程为后台线程，以确保此线程不会导致主进程阻塞
				daemonThread.Start(this);
			}
		}

		protected virtual void StartConnect()
		{
			// 优先IP，然后是Host
			if (!string.IsNullOrEmpty(IP))
			{
				string[] tmp = IP.Split(':');
				if (tmp.Length < 1)
				{
					if (EventHandler != null)
						EventHandler.OnConnectFailed(this, "无法解析" + IP);
					return;
				}
				if (Port <= 0)
				{
					// 没有独立设置端口，解析IP里的端口，覆盖Port
					int port;
					if (tmp.Length >= 2 && int.TryParse(tmp[1], out port))
						Port = port;
				}
				ConnectByIP(tmp[0], Port);
			}
			else if (!string.IsNullOrEmpty(Host))
			{
				string[] tmp = Host.Split(':');
				if (tmp.Length < 1)
				{
					if (EventHandler != null)
						EventHandler.OnConnectFailed(this, "无法解析" + Host);
					return;
				}
				if (Port <= 0)
				{
					// 没有独立设置端口，解析IP里的端口，覆盖Port
					int port;
					if (tmp.Length >= 2 && int.TryParse(tmp[1], out port))
						Port = port;
				}
				ConnectByHost(tmp[0], Port);
			}
		}

		#region Socket操作

		protected virtual void ConnectByIP(string ip, int port)
		{
			if (state == null)
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(this, "StateObject没有初始化");
				return;
			}
			else if (!state.IsAvailableForConnectRequest())
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(this, "连接器状态错误，无法开始新的连接，当前状态：" + state.state);
				return;
			}

			IPAddress ipAddress = null;
			if (!IPAddress.TryParse(ip, out ipAddress) || ipAddress == null)
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(this, "无法解析" + ip);
				return;
			}

			if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(this, "端口无效" + port);
				return;
			}

			IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
			state.ipEndPoint = ipEndPoint;
			state.state = EConnectorState.ExpectHasIpAddress;

			// 连接将由守护线程发起，守护线程会在连接器状态为ExpectHasIpAddress的时候自动调用连接接口
			//Connect();
		}

		protected virtual void ConnectByHost(string host, int port)
		{
			if (state == null)
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(this, "StateObject没有初始化");
				return;
			}
			else if (!state.IsAvailableForConnectRequest())
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(this, "连接器状态错误，无法开始新的连接，当前状态：" + state.state);
				return;
			}

			if (string.IsNullOrEmpty(host) || port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(this, "地址无效" + host + ":" + port);
				return;
			}

			IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.None, port);
			state.ipEndPoint = ipEndPoint;

			ResolveHost(host);
		}

		protected virtual void ResolveHost(string host)
		{
			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("开始解析" + host);

			state.state = EConnectorState.WaitingForDnsResolve;
			Dns.BeginGetHostAddresses(host, new AsyncCallback(BeginGetHostAddressesCallback), state);
		}

		protected virtual void Connect()
		{
			if (state == null)
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(this, "StateObject没有初始化");
				return;
			}
			else if (!state.IsAvailableForConnectRequest())
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(this, "连接器状态错误，当前状态：" + state.state);
				return;
			}
			else if (state.ipEndPoint == null)
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(this, "未设置连接地址");
				return;
			}

			if (state.workSocket == null)
			{
				state.workSocket = CreateSocket();
				if (state.workSocket == null)
					return;
				if (state.receiveBuffer == null)
					state.receiveBuffer = new byte[Math.Min(state.workSocket.ReceiveBufferSize, 8192)];
				if (state.sendBuffer == null)
					state.sendBuffer = new byte[Math.Min(state.workSocket.SendBufferSize, 8192)];
			}
			else if (state.workSocket.Connected)
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(this, "此connector的连接尚未断开，无法发起新的连接。连接器状态未同步？！");
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
					LogUtil.ShowException(new Exception("连接失败", ex));
				if (EventHandler != null)
					EventHandler.OnConnectFailed(this, "连接失败 " + ex.Message);
				state.Reset();
			}
		}

		protected virtual Socket CreateSocket()
		{
			switch (ProtocolType)
			{
				case EProtocolType.TCP:
					return new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
				case EProtocolType.UDP:
					return new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
				default:
					if (EventHandler != null)
						EventHandler.OnConnectFailed(this, "暂不支持的类型" + ProtocolType);
					return null;
			}
		}

		protected virtual void Disconnect()
		{
			if (state == null)
			{
				if (EventHandler != null)
					EventHandler.OnDisconnectFailed(this, "StateObject没有初始化");
				return;
			}
			else if (state.workSocket == null || state.state == EConnectorState.WaitingForDisconnect)
				return;
			else if (state.state == EConnectorState.WaitingForConnect || !state.workSocket.Connected)
			{
				// 还没有连接上，不管是正在等待连接还是没有发起连接，都重置连接器
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
				// 关闭的时候发生异常，不管是什么异常，都先重置连接器，然后在转抛异常
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(new Exception("断开连接失败", ex));
				if (EventHandler != null)
					EventHandler.OnDisconnectFailed(this, "断开连接失败 " + ex.Message);
				state.Reset();
			}
		}

		protected virtual void SendData(byte[] data, int offset, int length)
		{
			if (state == null || state.workSocket == null)
			{
				if (EventHandler != null)
					EventHandler.OnDataSendFailed(this, "StateObject没有初始化");
				return;
			}
			if (!state.IsAvailableForReceiveAndSend())
			{
				if (EventHandler != null)
					EventHandler.OnDataSendFailed(this, "连接器状态错误，无法接收网络数据，当前状态：" + state.state);
				return;
			}
			if(length > ProtocolDefine.maximumProtocolLength)
			{
				if (EventHandler != null)
					EventHandler.OnDataSendFailed(this, "数据大小超出系统允许的范围");
				return;
			}

			// 这个接口不论当前网络是否繁忙，都只将数据放入发送缓存，真正将数据发送到网络是在不带参数的SendData()里，主要目的是实现主线程和网络数据收发的隔离

			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("缓存发送数据");

			if (state.sendCache == null || !state.sendCache.TryPush(data, offset, length))
			{
				if (EventHandler != null)
					EventHandler.OnDataSendFailed(this, "发送失败，无法缓存发送数据");
			}
		}

		#endregion

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
				if (EventHandler != null)
					EventHandler.OnConnectFailed(state.context, "异步参数错误，传递的StateObject前后不一致");
				s.Release();
				return;
			}

			if (s.state != EConnectorState.WaitingForDnsResolve)
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(s.context, "连接器状态错误，后续操作取消，期望状态：WaitingForDnsResolve，当前状态：" + state.state);
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
				catch (Exception ex)
				{
					if (LogUtil.ShowException != null)
						LogUtil.ShowException(new Exception("DNS解析错误", ex));
					if (EventHandler != null)
						EventHandler.OnConnectFailed(s.context, "DNS解析错误 " + ex.Message);
					s.Reset();
				}
			}
			else
			{
				if (EventHandler != null)
					EventHandler.OnConnectFailed(s.context, "DNS解析错误");
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
					if (EventHandler != null)
						EventHandler.OnConnectFailed(state.context, "异步参数错误，传递的StateObject前后不一致");
					s.Release();
					return;
				}

				s.state = EConnectorState.ExpectConnected;

				if (LogUtil.ShowDebug != null)
					LogUtil.ShowDebug("连接成功" + state.ipEndPoint.ToString());
			}
			catch(ObjectDisposedException ex)
			{
				// socket被关闭，而且是由BeginDisconnect发起的（目的就是停止执行中的的BeginConnect），那么不需要释放资源，保持此状态，否则报错
				if (s.state != EConnectorState.ExpectDisconnected)
				{
					if (LogUtil.ShowException != null)
						LogUtil.ShowException(new Exception("连接失败", ex));
					if (EventHandler != null)
						EventHandler.OnConnectFailed(s.context, "连接失败 " + ex.Message);
					s.Reset();
				}
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(new Exception("连接失败", ex));
				if (EventHandler != null)
					EventHandler.OnConnectFailed(s.context, "连接失败 " + ex.Message);
				s.Reset();
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
					if (EventHandler != null)
						EventHandler.OnDisconnectFailed(state.context, "异步参数错误，传递的StateObject前后不一致");
					s.Release();
					return;
				}

				if (s.state != EConnectorState.WaitingForDisconnect)
				{
					if (EventHandler != null)
						EventHandler.OnDisconnectFailed(s.context, "连接器状态错误，后续操作取消，期望状态：WaitingForDisconnect，当前状态：" + state.state);
					return;
				}

				// 因为不能重用socket连接相同的EndPoint，所以重用没有什么意义，断开连接之后就释放掉socket资源
				s.Reset();
				s.state = EConnectorState.ExpectDisconnected;   // 重置连接之后，要把状态置成已经断开，因为直接重置后的状态是初始化

				if (LogUtil.ShowDebug != null)
					LogUtil.ShowDebug("断开连接成功" + state.ipEndPoint.ToString());
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(new Exception("断开连接失败", ex));
				if (EventHandler != null)
					EventHandler.OnDisconnectFailed(s.context, "断开连接失败 " + ex.Message);
				s.Reset();
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
					if (EventHandler != null)
						EventHandler.OnDataSendFailed(state.context, "异步参数错误，传递的StateObject前后不一致");
					s.Release();
					return;
				}

				if (EventHandler != null)
					EventHandler.OnDataSent(s.context, bytesSent);

				// 如果没有等待发送的缓存数据，那么可以结束
				if (s.sendCache == null || s.sendCache.IsEmpty)
				{
					s.isSending = false;
					return;
				}

				if (LogUtil.ShowDebug != null)
					LogUtil.ShowDebug("开始发送缓存数据");

				// 一次最多取出sendBuffer能够容纳的数据量
				int sendBytes = Math.Min(s.sendCache.Length, s.sendBuffer.Length);
				if (!s.sendCache.TryPull(sendBytes, ref s.sendBuffer))
				{
					s.sendCache.Clear();
					s.isSending = false;
					return;
				}

				s.isSending = true;
				s.workSocket.BeginSend(s.sendBuffer, 0, sendBytes, SocketFlags.None, new AsyncCallback(SendCallback), s);
			}
			catch (ObjectDisposedException)
			{
				// Socket被关闭了，常见于网络连接丢失，或者连接器主动断开，此异常不需要转抛
				s.isSending = false;
				if (EventHandler != null)
					EventHandler.OnDataSendFailed(s.context, "发送失败，连接已被关闭");
			}
			catch (Exception ex)
			{
				// 发送失败的情况下，只将标识正在发送的标识位翻转，然后转抛异常
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(new Exception("发送失败", ex));
				if (EventHandler != null)
					EventHandler.OnDataSendFailed(s.context, "发送失败 " + ex.Message);
				s.isSending = false;
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
					if (EventHandler != null)
						EventHandler.OnDataReceiveFailed(state.context, "异步参数错误，传递的StateObject前后不一致");
					s.Release();
					return;
				}

				if (bytesRead > 0)
				{
					// 保存当前读取到的字节
					if (s.receiveCache != null)
						s.receiveCache.TryPush(s.receiveBuffer, 0, bytesRead);      // 如果保存出错，那么这些数据将会丢失
					if (EventHandler != null)
						EventHandler.OnDataReceived(s.context, bytesRead);
				}
			}
			catch (ObjectDisposedException)
			{
				// Socket被关闭了，常见于网络连接丢失，或者连接器主动断开，此异常不需要转抛
				s.isReceiving = false;
				if (EventHandler != null)
					EventHandler.OnDataReceiveFailed(s.context, "接收失败，连接已被关闭");
			}
			catch (Exception ex)
			{
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(new Exception("接收失败", ex));
				if (EventHandler != null)
					EventHandler.OnDataReceiveFailed(s.context, "接收失败 " + ex.Message);
				s.isReceiving = false;
			}
			finally
			{
				s.isReceiving = false;
			}
		}

		#endregion

		#region 守护线程

		// 守护线程的线程体
		void DaemonProcedure(object param)
		{
			Connector daemonObject = param as Connector;
			if (daemonObject == null)
				return;

			if(LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("守护线程开始工作");

			DaemonParameter daemonParameter = new DaemonParameter
			{
				daemonObject = daemonObject,
				isConnected = daemonObject.state.state == EConnectorState.ExpectConnected,
				isReconnecting = false,
				autoReconnectRetryCount = 0,
				autoReconnectLastTime = 0,
				lastHeartBeatTime = 0,
				lastHeartBeatReturnTime = 0,
			};

			while(!daemonObject.released)
			{
				// 守护线程该干的活
				if(daemonObject.state != null)
				{
					_CheckHeartBeat(ref daemonParameter);
					_CheckDaemonObjectState(ref daemonParameter);
					_CheckSendData(ref daemonParameter);
					_CheckReceiveData(ref daemonParameter);
				}
				Thread.Sleep(daemonLoopInterval);
				//daemonObject.daemonSuspend.WaitOne();	// 如果信号被置为无信号状态，那么守护线程阻塞，直到恢复信号
			}

			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("守护线程退出");
		}

		void _CheckHeartBeat(ref DaemonParameter daemonParameter)
		{
			if (!daemonParameter.daemonObject.SupportHeartBeat)
				return;
			// 连接器状态是连接上的，也没有处于重连过程中，那么可以检查心跳和发送心跳
			if (daemonParameter.isConnected 
				&& !daemonParameter.isReconnecting 
				&& daemonParameter.daemonObject.state.state == EConnectorState.ExpectConnected)
			{
				// 只有发送过心跳了，并且还没有收到心跳反馈，才需要检查心跳反馈
				if (daemonParameter.lastHeartBeatTime > 0 && daemonParameter.lastHeartBeatReturnTime <= daemonParameter.lastHeartBeatTime)
				{
					if (DateTimeUtil.NowMillisecond - daemonParameter.lastHeartBeatTime >= daemonParameter.daemonObject.HeartBeatReturnTimeout)
					{
						// 心跳超时，认为连接已经断开
						_ProcessDisconnectUnexpected(ref daemonParameter);
					}
				}
				// 再次检查连接器状态，如果一切正常，那么就可以检查并发送心跳
				if (daemonParameter.isConnected 
					&& !daemonParameter.isReconnecting 
					&& daemonParameter.daemonObject.state.state == EConnectorState.ExpectConnected)
				{
					if (DateTimeUtil.NowMillisecond - daemonParameter.lastHeartBeatTime >= daemonParameter.daemonObject.HeartBeatInterval)
					{
						// 发送心跳，并记录心跳发送时间
						byte[] heartBeatPack = ProtocolDefine.ComposeHeartBeat(1);
						daemonParameter.daemonObject.Send(heartBeatPack, 0, heartBeatPack.Length);
						daemonParameter.lastHeartBeatTime = DateTimeUtil.NowMillisecond;
					}
				}
			}
		}

		void _CheckDaemonObjectState(ref DaemonParameter daemonParameter)
		{
			if (!daemonParameter.isConnected)
			{
				// 如果上一次检查时连接器状态是断开状态
				_CheckDaemonObjectStateExpectDisconnected(ref daemonParameter);
			}
			else
			{
				// 如果上一次检查时连接器状态是连接状态
				_CheckDaemonObjectStateExpectConnected(ref daemonParameter);
			}
		}

		void _CheckDaemonObjectStateExpectConnected(ref DaemonParameter daemonParameter)
		{
			if (daemonParameter.daemonObject.state.IsDisconnected())
			{
				// 如果连接器上一次检测时是连接上的，但是现在连接断开了，那么认为连接器刚刚断开连接
				_ProcessDisconnected(ref daemonParameter);
			}
		}

		void _CheckDaemonObjectStateExpectDisconnected(ref DaemonParameter daemonParameter)
		{
			if (daemonParameter.daemonObject.state.state == EConnectorState.ExpectHasIpAddress)
			{
				// 如果连接器已经解析好了IP地址，但是没有连接上，也没有在等待连接，那么认为连接器处在刚刚解析完域名等待调用连接的状态，调用连接
				daemonParameter.daemonObject.Connect();
			}
			else if (daemonParameter.daemonObject.state.state == EConnectorState.ExpectConnected)
			{
				// 如果连接器上一次检测时还没有连接上，但是现在已经连接上了，那么认为连接器刚刚连接成功
				_ProcessConnected(ref daemonParameter);
			}
			else if (daemonParameter.daemonObject.state.IsDisconnected())
			{
				// 连接器处于断开状态，并且已进入重连流程，那么按照参数指定的周期尝试连接
				if (daemonParameter.isReconnecting)
					_TryReconnect(ref daemonParameter);
			}
		}

		void _CheckSendData(ref DaemonParameter daemonParameter)
		{
			if (daemonParameter.isConnected 
				&& !daemonParameter.isReconnecting 
				&& daemonParameter.daemonObject.state.state == EConnectorState.ExpectConnected 
				&& !daemonParameter.daemonObject.state.isSending)
			{
				// 连接器维持连接状态下，如果当前不忙碌，就立刻执行发送，将缓存的发送数据发送出去
				_SendData(ref daemonParameter);
			}
		}

		void _CheckReceiveData(ref DaemonParameter daemonParameter)
		{
			if (!daemonParameter.daemonObject.state.isReceiving)
			{
				// 连接器不在接收状态，那么将当前buffer中已经接收到的数据进行处理
				_ProcessReceivedData(ref daemonParameter);
				if (daemonParameter.isConnected 
					&& !daemonParameter.isReconnecting 
					&& daemonParameter.daemonObject.state.state == EConnectorState.ExpectConnected 
					&& daemonParameter.daemonObject.state.HasDataAvailable)
				{
					// 如果当前处于连接状态，而且网络通道里有数据等待接收，就再次发起接收
					_ReceiveData(ref daemonParameter);
				}
			}
		}

		void _ProcessConnected(ref DaemonParameter daemonParameter)
		{
			daemonParameter.isConnected = true;
			if (!daemonParameter.isReconnecting)
			{
				// 非重连的情况下，触发连接成功回调
				if (daemonParameter.daemonObject.EventHandler != null)
					daemonParameter.daemonObject.EventHandler.OnConnected(daemonParameter.daemonObject);
			}
			else
			{
				// 重连状况下，触发重连成功回调，并退出重连流程
				daemonParameter.isReconnecting = false;
				if (daemonParameter.daemonObject.EventHandler != null)
					daemonParameter.daemonObject.EventHandler.OnReconnected(daemonParameter.daemonObject);
			}
		}

		void _ProcessDisconnected(ref DaemonParameter daemonParameter)
		{
			daemonParameter.isConnected = false;
			if (!daemonParameter.isReconnecting)
			{
				// 非重连的情况下，触发断开连接成功的回调（重连状况下，不触发回调而是进入等待重连流程）
				if (daemonParameter.daemonObject.EventHandler != null)
					daemonParameter.daemonObject.EventHandler.OnDisconnected(daemonParameter.daemonObject);
			}
			daemonParameter.lastHeartBeatTime = 0;
			daemonParameter.lastHeartBeatReturnTime = 0;
		}

		void _ProcessDisconnectUnexpected(ref DaemonParameter daemonParameter)
		{
			// 触发意外断开回调
			if (daemonParameter.daemonObject.EventHandler != null)
				daemonParameter.daemonObject.EventHandler.OnConnectionLost(daemonParameter.daemonObject);
			// 如果需要重连,标记重连
			if (daemonParameter.daemonObject.SupportAutoReconnect)
			{
				daemonParameter.isReconnecting = true;
				// 重置参数
				daemonParameter.autoReconnectRetryCount = 0;
				daemonParameter.autoReconnectLastTime = DateTimeUtil.NowMillisecond;		// 注意，这里将上一次重连时间设置为当前时间，可以让重连流程在发现连接断开时等待一个重连间隔然后再重新连接，而不是一发现断开立刻重连
			}
			daemonParameter.isConnected = false;
			// 主动断开连接，以便标志位状态同步
			daemonParameter.daemonObject.Disconnect();
			daemonParameter.lastHeartBeatTime = 0;
			daemonParameter.lastHeartBeatReturnTime = 0;
		}

		void _TryReconnect(ref DaemonParameter daemonParameter)
		{
			if (daemonParameter.autoReconnectRetryCount >= daemonParameter.daemonObject.ReconnectMaxRetry)
			{
				// 已经超过最大重试次数，退出重连流程
				daemonParameter.isReconnecting = false;
				// 触发重连失败的回调
				if (daemonParameter.daemonObject.EventHandler != null)
					daemonParameter.daemonObject.EventHandler.OnReconnectFailed(daemonParameter.daemonObject, "已达最大重试次数");
			}
			else if (DateTimeUtil.NowMillisecond - daemonParameter.autoReconnectLastTime > daemonParameter.daemonObject.ReconnectRetryTimeout)
			{
				// 重试超时已到，发起新一次的重连，并记录这一次重连的时间，重试次数加1
				daemonParameter.autoReconnectLastTime = DateTimeUtil.NowMillisecond;
				++daemonParameter.autoReconnectRetryCount;
				daemonParameter.daemonObject.Connect();
			}
		}

		void _ProcessReceivedData(ref DaemonParameter daemonParameter)
		{
			RingBuffer<byte> dataBuffer = daemonParameter.daemonObject.receiveCache;
			while (!dataBuffer.IsEmpty)
			{
				// 拆包，按完整消息返回给Handler
				// 寻找魔数
				if (dataBuffer.DiscardToFirstIndexOf(0xC8, 0xEF))
				{
					byte headLength = 0;
					if (dataBuffer.Length >= 3)
					{
						// 找到了魔数，而且数据长度足够解析出消息头长度
						// 解析出消息头长度
						dataBuffer.Peek(2, out headLength);
					}
					else
					{
						// 消息接收长度还不够，继续接收
						if (LogUtil.ShowDebug != null)
							LogUtil.ShowDebug("消息头没收全，继续接收1");
						break;
					}
					// 进一步解析出整个消息头
					if (headLength > 0 && dataBuffer.Length >= headLength)
					{
						if (headLength < ProtocolDefine.minimalHeadLength)
						{
							// 万一消息头长度不满足协议定义的最小长度，说明收到了一条非本系统能够处理的协议，抛弃（直接抛弃协议头即可，协议体会自然在下一个循环里被抛弃掉）
							dataBuffer.Discard(headLength);
							if (LogUtil.ShowDebug != null)
								LogUtil.ShowDebug("收到的消息头不满足系统定义的最小消息头长度，抛弃");
							continue;
						}
						// 解析出了消息头长度，而且数据长度足够解析出整个消息头
						// 解析出消息头里的必要数据
						byte token;
						dataBuffer.Peek(5, out token);
						// 获得心跳标识位
						bool heartBeat = (token & ProtocolDefine.heartBeatMask) != 0;
						/*
						// 获得单向消息标识位
						bool oneWay = (token & ProtocolDefine.oneWayMask) != 0;
						// 获得响应消息标识位
						bool response = (token & ProtocolDefine.responseMask) != 0;
						// 获得加密标识位
						bool encrypt = (token & ProtocolDefine.encryptMask) != 0;
						// 获得加密方式
						int encryptType = (token & ProtocolDefine.encryptTypeMask) >> ProtocolDefine.encryptTypeBitRightOffset;
						// 获得压缩标识位
						bool compress = (token & ProtocolDefine.compressMask) != 0;
						// 获得拆分标识位
						bool split = (token & ProtocolDefine.splitMask) != 0;
						*/
						// 如果是心跳消息，那么记录下收到的时间，抛弃消息头（心跳消息没有消息体，所以抛弃消息头就抛弃了整条心跳消息）
						if (heartBeat)
						{
							daemonParameter.lastHeartBeatReturnTime = DateTimeUtil.NowMillisecond;
							dataBuffer.Discard(headLength);
							continue;
						}
						// @TODO: 处理拆分的包，要将不完整的包临时放入一个为超大包准备的缓存，直到所有拆开的包体都收到了再合并成完整的包
						byte[] bodyLengthBytes = new byte[2];
						dataBuffer.Peek(10, out bodyLengthBytes[0]);
						dataBuffer.Peek(11, out bodyLengthBytes[1]);
						short bodyLength = BitConverterEx.GetShort(bodyLengthBytes, 0, true);
						if (dataBuffer.Length >= (headLength + bodyLength))
						{
							// 数据长度满足协议头和协议体，即完整消息已经接收到了
							byte[] msgBytes = new byte[headLength + bodyLength];
							if (dataBuffer.TryPull(msgBytes.Length, ref msgBytes) && daemonParameter.daemonObject.DataHandler != null)
								daemonParameter.daemonObject.DataHandler.OnDataReceived(daemonParameter.daemonObject, msgBytes);
						}
						else
						{
							// 消息接收长度还不够，继续接收
							if (LogUtil.ShowDebug != null)
								LogUtil.ShowDebug("消息体没收全，继续接收");
							break;
						}
					}
					else
					{
						// 消息接收长度还不够，继续接收
						if (LogUtil.ShowDebug != null)
							LogUtil.ShowDebug("消息头没收全，继续接收2");
						break;
					}
				}
				else
					dataBuffer.DiscardAll();
			}
		}

		protected virtual void _SendData(ref DaemonParameter daemonParameter)
		{
			StateObject s = daemonParameter.daemonObject.state;
			if (!s.IsAvailableForReceiveAndSend())
			{
				if (EventHandler != null)
					EventHandler.OnDataSendFailed(daemonParameter.daemonObject, "连接器状态错误，无法接收网络数据，当前状态：" + s.state);
				return;
			}

			// 没有缓存数据，放弃发送
			if (s.sendCache == null || s.sendCache.IsEmpty)
				return;

			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("开始发送缓存数据");

			if (s.sendBuffer == null)
			{
				if (EventHandler != null)
					EventHandler.OnDataSendFailed(this, "发送数据失败，发送缓存没有初始化");
				return;
			}
			// 一次最多取出sendBuffer能够容纳的数据量
			int sendBytes = Math.Min(s.sendBuffer.Length, s.sendCache.Length);
			if (!s.sendCache.TryPull(sendBytes, ref s.sendBuffer))
			{
				s.sendCache.Clear();
				if (EventHandler != null)
					EventHandler.OnDataSendFailed(this, "发送数据失败，获取缓存的发送数据失败");
				return;
			}

			try
			{
				s.isSending = true;
				if(s.workSocket != null)
					s.workSocket.BeginSend(s.sendBuffer, 0, sendBytes, SocketFlags.None, new AsyncCallback(SendCallback), s);
			}
			catch (ObjectDisposedException)
			{
				// Socket被关闭了，常见于网络连接丢失，或者连接器主动断开，此异常不需要转抛
				s.isSending = false;
				if (EventHandler != null)
					EventHandler.OnDataSendFailed(this, "发送数据失败，连接已被关闭");
			}
			catch (Exception ex)
			{
				// 发送失败的情况下，只将标识正在发送的标识位翻转，然后转抛异常
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(new Exception("发送数据失败", ex));
				if (EventHandler != null)
					EventHandler.OnDataSendFailed(this, "发送数据失败 " + ex.Message);
				s.isSending = false;
			}
		}

		protected virtual void _ReceiveData(ref DaemonParameter daemonParameter)
		{
			StateObject s = daemonParameter.daemonObject.state;
			if (!s.IsAvailableForReceiveAndSend())
			{
				if (EventHandler != null)
					EventHandler.OnDataReceiveFailed(this, "连接器状态错误，无法接收网络数据，当前状态：" + s.state);
				return;
			}

			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug("开始接收");

			try
			{
				s.isReceiving = true;
				if(s.workSocket != null)
					s.workSocket.BeginReceive(s.receiveBuffer, 0, s.receiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), s);
			}
			catch (ObjectDisposedException)
			{
				// Socket被关闭了，常见于网络连接丢失，或者连接器主动断开，此异常不需要转抛
				s.isReceiving = false;
				if (EventHandler != null)
					EventHandler.OnDataReceiveFailed(this, "接收数据失败，连接已被关闭");
			}
			catch (Exception ex)
			{
				// 接收失败的情况下，只将标识正在接收的标识位翻转，然后转抛异常
				if (LogUtil.ShowException != null)
					LogUtil.ShowException(new Exception("接收数据失败", ex));
				if (EventHandler != null)
					EventHandler.OnDataReceiveFailed(this, "接收数据失败 " + ex.Message);
				s.isReceiving = false;
			}
		}

		#endregion
	}

}
