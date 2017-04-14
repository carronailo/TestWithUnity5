using System.Collections.Generic;

namespace Networks
{
	public class PacketBuffer
	{
		private static PacketBuffer instance = null;

		public static PacketBuffer GetInstance()
		{
			if (instance == null)
			{
				instance = new PacketBuffer();
			}
			return instance;
		}

		private PacketBuffer()
		{

		}

		private int classID;
		private int methodID;

		private List<byte> buffer = new List<byte>();

		public override string ToString()
		{
			return string.Format("远程消息 标识[{0}-{1}] 长度[{2}]字节", classID, methodID, buffer.Count);
		}

		public PacketBuffer Clear()
		{
			buffer.Clear();
			return this;
		}

		public PacketBuffer Send(IConnectionManager mgr)
		{
			buffer.InsertRange(0, BitConverterEx.GetBytes((short)(buffer.Count), true));
			mgr.Send(classID, methodID, buffer.ToArray());
			if (LogUtil.ShowDebug != null && LogUtil.ShowWarning != null)
				LogUtil.ShowWarning("[网络消息中心][发送]" + ToString());
			return this;
		}

		public PacketBuffer SetObjMethod(int classID, int methodID)
		{
			this.classID = classID;
			this.methodID = methodID;
			buffer.Add((byte)classID);
			buffer.Add((byte)methodID);
			return this;
		}

		public PacketBuffer Add(string s)
		{
			buffer.AddRange(BitConverterEx.GetBytes(s, true));
			return this;
		}

		public PacketBuffer Add(byte n)
		{
			buffer.Add(n);
			return this;
		}

		public PacketBuffer Add(bool b)
		{
			buffer.AddRange(BitConverterEx.GetBytes(b));
			return this;
		}

		public PacketBuffer Add(short n)
		{
			buffer.AddRange(BitConverterEx.GetBytes(n, true));
			return this;
		}

		public PacketBuffer Add(int n)
		{
			buffer.AddRange(BitConverterEx.GetBytes(n, true));
			return this;
		}

		public PacketBuffer Add(float f)
		{
			buffer.AddRange(BitConverterEx.GetBytes(f, true));
			return this;
		}

		public PacketBuffer Add(long dw)
		{
			buffer.AddRange(BitConverterEx.GetBytes(dw, true));
			return this;
		}

		public PacketBuffer Add(double dw)
		{
			buffer.AddRange(BitConverterEx.GetBytes(dw, true));
			return this;
		}

		public PacketBuffer Add(byte[] narr)
		{
			buffer.AddRange(BitConverterEx.GetBytes((short)narr.Length, true));
			foreach (byte n in narr)
			{
				buffer.Add(n);
			}
			return this;
		}

		public PacketBuffer Add(bool[] barr)
		{
			buffer.AddRange(BitConverterEx.GetBytes((short)barr.Length, true));
			foreach (bool b in barr)
			{
				buffer.AddRange(BitConverterEx.GetBytes(b));
			}
			return this;
		}

		public PacketBuffer Add(short[] narr)
		{
			buffer.AddRange(BitConverterEx.GetBytes((short)narr.Length, true));
			foreach (short n in narr)
			{
				buffer.AddRange(BitConverterEx.GetBytes(n, true));
			}
			return this;
		}

		public PacketBuffer Add(int[] narr)
		{
			buffer.AddRange(BitConverterEx.GetBytes((short)narr.Length, true));
			foreach (int n in narr)
			{
				buffer.AddRange(BitConverterEx.GetBytes(n, true));
			}
			return this;
		}

		public PacketBuffer Add(float[] farr)
		{
			buffer.AddRange(BitConverterEx.GetBytes((short)farr.Length, true));
			foreach (float f in farr)
			{
				buffer.AddRange(BitConverterEx.GetBytes(f, true));
			}
			return this;
		}

		public PacketBuffer Add(long[] dwarr)
		{
			buffer.AddRange(BitConverterEx.GetBytes((short)dwarr.Length, true));
			foreach (long dw in dwarr)
			{
				buffer.AddRange(BitConverterEx.GetBytes(dw, true));
			}
			return this;
		}

		public PacketBuffer Add(double[] dwarr)
		{
			buffer.AddRange(BitConverterEx.GetBytes((short)dwarr.Length, true));
			foreach (double dw in dwarr)
			{
				buffer.AddRange(BitConverterEx.GetBytes(dw, true));
			}
			return this;
		}

		//public PacketBuffer Add(RemoteCommandParam p_kParam)
		//{
		//    m_vBuffer.AddRange(p_kParam.Serialize());
		//    return this;
		//}

	}
}
