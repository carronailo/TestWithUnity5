
namespace NIO
{
	using System;
	using System.Net;
	using System.Net.Sockets;

	public class Bootstrap
	{
		volatile IEventExecutorGroup group;

		volatile Socket connectSocket = null;
		SynchronizedQueue<SocketAsyncEventArgs> eventArgsPool = null;

		public Bootstrap()
		{
			eventArgsPool = new SynchronizedQueue<SocketAsyncEventArgs>();
			for (int i = 0; i < 4; ++i)
			{
				SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
				eventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IOCompleted);
				eventArgsPool.TryEnqueue(eventArgs);
			}
		}

		public bool Connected { get; private set; }

		public Bootstrap Group(IEventExecutorGroup group)
		{
			if (group == null)
				throw new ArgumentNullException("group can not be null.");
			if (this.group != null)
				throw new InvalidOperationException("group has already been set.");
			this.group = group;
			return this;
		}

		Bootstrap Validate()
		{
			if (group == null)
				throw new InvalidOperationException("group not set.");
			return this;
		}

		public void ConnectAsync(string ip, int port)
		{
			Validate();

			if(connectSocket == null)
			{
				connectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				_ConfigureTcpSocket(connectSocket);
			}
			SocketAsyncEventArgs connectEventArg;
			if(eventArgsPool.TryDequeue(out connectEventArg))
			{
				connectEventArg.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
				if (!connectSocket.ConnectAsync(connectEventArg))
				{
					ProcessConnect(connectEventArg);
				}
			}
			else
			{
				// @TODO: 没有 SocketAsyncEventArgs 了
			}
		}

		public void DisconnectAsync()
		{
			if(connectSocket != null)
			{
				connectSocket.Shutdown(SocketShutdown.Both);
				SocketAsyncEventArgs disconnectEventArg;
				if(eventArgsPool.TryDequeue(out disconnectEventArg))
				{
					if (!connectSocket.DisconnectAsync(disconnectEventArg))
					{
						ProcessDisconnect(disconnectEventArg);
					}
				}
				else
				{
					// @TODO: 没有 SocketAsyncEventArgs 了
				}
			}
		}

		void IOCompleted(object sender, SocketAsyncEventArgs e)
		{
			switch (e.LastOperation)
			{
				case SocketAsyncOperation.Connect:
					ProcessConnect(e);
					break;
				case SocketAsyncOperation.Disconnect:
					ProcessDisconnect(e);
					break;
				case SocketAsyncOperation.Receive:
				case SocketAsyncOperation.ReceiveFrom:
				case SocketAsyncOperation.ReceiveMessageFrom:
					ProcessRead(e);
					break;
				case SocketAsyncOperation.Send:
				case SocketAsyncOperation.SendPackets:
				case SocketAsyncOperation.SendTo:
					ProcessWrite(e);
					break;
				default:
					throw new InvalidOperationException("socket operation not supported." + e.LastOperation.ToString());
			}
		}

		void ProcessConnect(SocketAsyncEventArgs e)
		{
			if(e.LastOperation == SocketAsyncOperation.Connect && e.SocketError == SocketError.Success)
			{
				// @TODO: 连接成功
			}
			eventArgsPool.TryEnqueue(e);
		}

		void ProcessDisconnect(SocketAsyncEventArgs e)
		{
			if (e.LastOperation == SocketAsyncOperation.Disconnect && e.SocketError == SocketError.Success)
			{
				// @TODO: 断开成功
				connectSocket = null;
				if (e.ConnectSocket != null)
					e.ConnectSocket.Close();
			}
			eventArgsPool.TryEnqueue(e);
		}

		void ProcessRead(SocketAsyncEventArgs e)
		{
			eventArgsPool.TryEnqueue(e);
		}

		void ProcessWrite(SocketAsyncEventArgs e)
		{
			eventArgsPool.TryEnqueue(e);
		}

		void _ConfigureTcpSocket(Socket socket)
		{
			if (socket.ProtocolType != ProtocolType.Tcp)
				return;

			// Don't allow another socket to bind to this port.
			socket.ExclusiveAddressUse = true;

			// Set the receive buffer size to 8k
			socket.ReceiveBufferSize = 8192;

			// Set the timeout for synchronous receive methods to 
			// 1 second (1000 milliseconds.)
			socket.ReceiveTimeout = 1000;

			// Set the send buffer size to 8k.
			socket.SendBufferSize = 8192;

			// Set the timeout for synchronous send methods
			// to 1 second (1000 milliseconds.)			
			socket.SendTimeout = 1000;

			// 注意：KeepAliveValues在mono里暂时不支持
			//// 设置socket底层参数，修改心跳间隔时间
			//byte[] inOptionValues = new byte[4 * 3];
			//BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);   // 是否启用Keep-Alive， 1启用
			//BitConverter.GetBytes((uint)5000).CopyTo(inOptionValues, 4);    // 多长时间开始第一次探测，5000ms
			//BitConverter.GetBytes((uint)5000).CopyTo(inOptionValues, 4 * 2);    // 探测时间间隔，5000ms
			//socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
		}
	}
}

