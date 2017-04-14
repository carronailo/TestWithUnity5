using System;

namespace Networks
{
	public interface INetworkMessage
	{
	}

	public struct RawNetworkMessage
	{
		public int ClassID
		{
			get { return classID; }
			set
			{
				classID = value;
				Identity = ((long)classID << 32) | (uint)functionID;
			}
		}
		private int classID;
		public int FunctionID
		{
			get { return functionID; }
			set
			{
				functionID = value;
				Identity = ((long)classID << 32) | (uint)functionID;
			}
		}
		private int functionID;
		public long Identity { get; private set; }
		public byte[] Content { get; set; }
		//public bool IsDeserialized { get; set; }
		//public long TimeStamp { private get; set; }

		override public string ToString()
		{
			return string.Format("远程消息类型[{0}] 标识[{1}-{2}] 长度[{3}]字节", GetType().Name, ClassID, FunctionID, Content == null ? 0 : Content.Length);
			//return string.Format("远程消息类型[{0}] 标识[{1}-{2}] 长度[{3}]字节 时间戳[{4}]", GetType().Name, ClassID, FunctionID, Content == null ? 0 : Content.Length, TimeStamp);
		}
	}

	public static class NetworkMessage
	{
		public static void Broadcast(ref RawNetworkMessage msg)
		{
			if (LogUtil.ShowDebug != null && LogUtil.ShowError != null)
				LogUtil.ShowError("[网络消息中心][接收]" + msg.ToString());
			NetworkMessageCentralHub.Instance.DeliverMessage(ref msg);
		}

		////TODO: 把这些属性都改成private get
		//public int ClassID
		//{
		//	get { return classID; }
		//	set
		//	{
		//		classID = value;
		//		Identity = ((long)classID << 32) | (uint)functionID;
		//	}
		//}
		//private int classID;
		//public int FunctionID
		//{
		//	get { return functionID; }
		//	set
		//	{
		//		functionID = value;
		//		Identity = ((long)classID << 32) | (uint)functionID;
		//	}
		//}
		//private int functionID;
		//public long Identity { get; private set; }
		//public byte[] Content { get; set; }
		//public bool IsDeserialized { get; set; }
		//public long TimeStamp { private get; set; }

		//override public string ToString()
		//{
		//	return string.Format("远程消息类型[{0}] 标识[{1}-{2}] 长度[{3}]字节 时间戳[{4}]", GetType().Name, ClassID, FunctionID, Content == null ? 0 : Content.Length, TimeStamp);
		//}

		//public void CopyFrom(NetworkMessage from)
		//{
		//	classID = from.ClassID;
		//	functionID = from.FunctionID;
		//	Identity = from.Identity;
		//	Content = from.Content;
		//	TimeStamp = from.TimeStamp;
		//}

		//public NetworkMessage Log()
		//{
		//	if (LogUtil.ShowInfo != null)
		//		LogUtil.ShowInfo(ToString());
		//	return this;
		//}
	}
}

