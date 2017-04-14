
namespace Networks.Temp
{
	public class ProtocolDefine
	{
		public const int minimalHeadLength = 12;
		public const int dynamicEncryptBlockLength = 32;
		public const int maximumProtocolLength = 20480;	// 系统允许的最大协议大小，20K，超过此容量的协议不予处理
		public static byte[] protocolMagicNumber = new byte[2] { 0xC8, 0xEF };
		public const byte heartBeatMask = 0x80;
		public const byte oneWayMask = 0x40;
		public const byte responseMask = 0x20;
		public const byte encryptMask = 0x10;
		public const byte encryptTypeMask = 0x0C;
		public const int encryptTypeBitLeftOffset = 4;
		public const int encryptTypeBitRightOffset = 2;
		public const byte compressMask = 0x02;
		public const byte splitMask = 0x01;

		private static byte[] standardHeartBeat = null; 

		public static byte[] ComposeHeartBeat(byte version)
		{
			if(standardHeartBeat == null)
			{
				standardHeartBeat = new byte[minimalHeadLength];
				// 初始化心跳数据包
				standardHeartBeat[0] = protocolMagicNumber[0];
				standardHeartBeat[1] = protocolMagicNumber[1];
				standardHeartBeat[2] = minimalHeadLength;
				standardHeartBeat[3] = version; // 版本
				standardHeartBeat[4] = 0; // 序列化类型，1-二进制 2-flatbuffers
				standardHeartBeat[5] = 0xC0;    // 二进制的 1100 0000，心跳|单向
			}
			else
			{
				standardHeartBeat[3] = version; // 版本
			}
			return standardHeartBeat;
		}

		public static int ComposeMessageHead(ref byte[] messageBuffer, byte version, bool encrypt = false, int encrytType = 0)
		{
			if (messageBuffer == null)
				return -1;
			int composeLength = minimalHeadLength;
			if (encrypt && encrytType == 0)
				composeLength += dynamicEncryptBlockLength;
			if (messageBuffer.Length < composeLength)
				return -2;
			System.Array.Clear(messageBuffer, 0, composeLength);
			messageBuffer[0] = protocolMagicNumber[0];
			messageBuffer[1] = protocolMagicNumber[1];
			messageBuffer[2] = (byte)composeLength;
			messageBuffer[3] = version; // 版本
			messageBuffer[4] = 0; // 序列化类型，1-二进制 2-flatbuffers
			if(encrypt)
			{
				byte token = 0x10;    // 二进制的 0001 0000，加密
				token |= (byte)((((byte)encrytType) << encryptTypeBitRightOffset) & encryptTypeMask);	// 添加加密类型
				messageBuffer[5] = token;
			}
			return composeLength;
		}

		public static int AttachMessageBody(ref byte[] messageBuffer, byte[] messageBody)
		{
			return AttachMessageBody(ref messageBuffer, messageBody, 0, messageBody.Length);
		}

		public static int AttachMessageBody(ref byte[] messageBuffer, byte[] messageBody, int bodyOffset, int bodyLength)
		{
			if (messageBuffer == null)
				return -1;
			int composeLength = minimalHeadLength;
			if (messageBuffer.Length < composeLength)
				return -2;
			int headLength = messageBuffer[2];
			if (headLength != composeLength && headLength != composeLength + dynamicEncryptBlockLength)
				return -3;
			composeLength = headLength + bodyLength;
			if (messageBuffer.Length < composeLength)
				return -4;
			byte[] bodyLengthBytes = BitConverterEx.GetBytes((short)bodyLength, true);
			messageBuffer[10] = bodyLengthBytes[0];
			messageBuffer[11] = bodyLengthBytes[1];
			System.Buffer.BlockCopy(messageBody, bodyOffset, messageBuffer, headLength, bodyLength);
			// @TODO: 如果需要加密，要计算消息体的密文或者散列值的密文
			// @TODO: 如果需要压缩，要压缩消息体
			// @TODO: 如果消息体过大，需要拆分消息体
			return composeLength;
		}
	}
}
