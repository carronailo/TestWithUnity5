using System;
using System.Collections.Generic;
using System.Text;

public class ByteArrayRef
{
	public byte[] array;
	public int start;
	public int length;
}

public class BitConverterEx
{
	private static byte[] revertTmp = new byte[8];

	#region Convert prime type to byte[]

	public static byte[] GetBytes(bool b)
	{
		return BitConverter.GetBytes(b);
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(bool[] arr, bool useBigEndian)
	{
		List<byte> tmp = new List<byte>();
		tmp.AddRange(GetBytes((short)arr.Length, useBigEndian));
		for (int i = 0; i < arr.Length; i++)
			tmp.AddRange(GetBytes(arr[i]));
		return tmp.ToArray();
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(short n, bool useBigEndian)
	{
		byte[] bytes = BitConverter.GetBytes(n);
		if ((useBigEndian && BitConverter.IsLittleEndian) || (!useBigEndian && !BitConverter.IsLittleEndian))
			Array.Reverse(bytes);
		return bytes;
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(short[] arr, bool useBigEndian)
	{
		List<byte> tmp = new List<byte>();
		tmp.AddRange(GetBytes((short)arr.Length, useBigEndian));
		for (int i = 0; i < arr.Length; i++)
			tmp.AddRange(GetBytes(arr[i], useBigEndian));
		return tmp.ToArray();
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(int i, bool useBigEndian)
	{
		byte[] bytes = BitConverter.GetBytes(i);
		if ((useBigEndian && BitConverter.IsLittleEndian) || (!useBigEndian && !BitConverter.IsLittleEndian))
			Array.Reverse(bytes);
		return bytes;
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(int[] arr, bool useBigEndian)
	{
		List<byte> tmp = new List<byte>();
		tmp.AddRange(GetBytes((short)arr.Length, useBigEndian));
		for (int i = 0; i < arr.Length; i++)
			tmp.AddRange(GetBytes(arr[i], useBigEndian));
		return tmp.ToArray();
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(long dw, bool useBigEndian)
	{
		byte[] bytes = BitConverter.GetBytes(dw);
		if ((useBigEndian && BitConverter.IsLittleEndian) || (!useBigEndian && !BitConverter.IsLittleEndian))
			Array.Reverse(bytes);
		return bytes;
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(long[] arr, bool useBigEndian)
	{
		List<byte> tmp = new List<byte>();
		tmp.AddRange(GetBytes((short)arr.Length, useBigEndian));
		for (int i = 0; i < arr.Length; i++)
			tmp.AddRange(GetBytes(arr[i], useBigEndian));
		return tmp.ToArray();
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(float f, bool useBigEndian)
	{
		byte[] bytes = BitConverter.GetBytes(f);
		if ((useBigEndian && BitConverter.IsLittleEndian) || (!useBigEndian && !BitConverter.IsLittleEndian))
			Array.Reverse(bytes);
		return bytes;
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(float[] arr, bool useBigEndian)
	{
		List<byte> tmp = new List<byte>();
		tmp.AddRange(GetBytes((short)arr.Length, useBigEndian));
		for (int i = 0; i < arr.Length; i++)
			tmp.AddRange(GetBytes(arr[i], useBigEndian));
		return tmp.ToArray();
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(double dw, bool useBigEndian)
	{
		byte[] bytes = BitConverter.GetBytes(dw);
		if ((useBigEndian && BitConverter.IsLittleEndian) || (!useBigEndian && !BitConverter.IsLittleEndian))
			Array.Reverse(bytes);
		return bytes;
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(double[] arr, bool useBigEndian)
	{
		List<byte> tmp = new List<byte>();
		tmp.AddRange(GetBytes((short)arr.Length, useBigEndian));
		for (int i = 0; i < arr.Length; i++)
			tmp.AddRange(GetBytes(arr[i], useBigEndian));
		return tmp.ToArray();
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(string s, bool useBigEndian)
	{
		if (s == null)
			return null;
		List<byte> tmp = new List<byte>();
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		tmp.AddRange(GetBytes((short)bytes.Length, useBigEndian));
		tmp.AddRange(bytes);
		return tmp.ToArray();
	}

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public static byte[] GetBytes(string[] arr, bool useBigEndian)
	{
		List<byte> tmp = new List<byte>();
		tmp.AddRange(GetBytes((short)arr.Length, useBigEndian));
		for (int i = 0; i < arr.Length; i++)
			tmp.AddRange(GetBytes((string)arr[i], useBigEndian));
		return tmp.ToArray();
	}

	#endregion

	#region Convert byte[] to prime type

	private byte[] buffer;
	private int index;
	private bool isBigEndian;

	/// <summary>
	/// 注意，所有网络消息的传递都使用大头，而本地数据处理中，x86，x64（绝大部分PC），ARM（绝大部分手机）架构CPU的设备是使用小头，只有PowerPC（一部分PC）架构CPU设备使用大头，以及Java虚拟机中会使用大头
	/// </summary>
	public BitConverterEx(byte[] bytes, int startIndex, bool isBigEndian)
	{
		buffer = bytes;
		index = startIndex;
		this.isBigEndian = isBigEndian;
	}

	public bool PopBoolean()
	{
		bool c = false;
		if (index < buffer.Length)
		{
			c = BitConverter.ToBoolean(buffer, index);
			++index;
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return c;
	}

	public byte PopByte()
	{
		byte b = byte.MinValue;
		if (index < buffer.Length)
		{
			b = buffer[index];
			++index;
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return b;
	}

	public short PopShort()
	{
		short s = short.MinValue;
		if (index < buffer.Length)
		{
			if ((isBigEndian && BitConverter.IsLittleEndian) || (!isBigEndian && !BitConverter.IsLittleEndian))
			{
				revertTmp[0] = buffer[index + 1];
				revertTmp[1] = buffer[index];
				s = BitConverter.ToInt16(revertTmp, 0);
				//Array.Reverse(buffer, index, 2);
				//s = BitConverter.ToInt16(buffer, index);
				//Array.Reverse(buffer, index, 2);
			}
			else
				s = BitConverter.ToInt16(buffer, index);
			index += 2;
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return s;
	}

	public int PopInteger()
	{
		int i = int.MinValue;
		if (index < buffer.Length)
		{
			if ((isBigEndian && BitConverter.IsLittleEndian) || (!isBigEndian && !BitConverter.IsLittleEndian))
			{
				revertTmp[0] = buffer[index + 3];
				revertTmp[1] = buffer[index + 2];
				revertTmp[2] = buffer[index + 1];
				revertTmp[3] = buffer[index];
				i = BitConverter.ToInt32(revertTmp, 0);
				//Array.Reverse(buffer, index, 4);
				//i = BitConverter.ToInt32(buffer, index);
				//Array.Reverse(buffer, index, 4);
			}
			else
				i = BitConverter.ToInt32(buffer, index);
			index += 4;
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return i;
	}

	public long PopLong()
	{
		long l = long.MinValue;
		if (index < buffer.Length)
		{
			if ((isBigEndian && BitConverter.IsLittleEndian) || (!isBigEndian && !BitConverter.IsLittleEndian))
			{
				revertTmp[0] = buffer[index + 7];
				revertTmp[1] = buffer[index + 6];
				revertTmp[2] = buffer[index + 5];
				revertTmp[3] = buffer[index + 4];
				revertTmp[4] = buffer[index + 3];
				revertTmp[5] = buffer[index + 2];
				revertTmp[6] = buffer[index + 1];
				revertTmp[7] = buffer[index];
				l = BitConverter.ToInt64(revertTmp, 0);
				//Array.Reverse(buffer, index, 8);
				//l = BitConverter.ToInt64(buffer, index);
				//Array.Reverse(buffer, index, 8);
			}
			else
				l = BitConverter.ToInt64(buffer, index);
			index += 8;
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return l;
	}

	public float PopFloat()
	{
		float f = float.NaN;
		if (index < buffer.Length)
		{
			if ((isBigEndian && BitConverter.IsLittleEndian) || (!isBigEndian && !BitConverter.IsLittleEndian))
			{
				revertTmp[0] = buffer[index + 3];
				revertTmp[1] = buffer[index + 2];
				revertTmp[2] = buffer[index + 1];
				revertTmp[3] = buffer[index];
				f = BitConverter.ToSingle(revertTmp, 0);
				//Array.Reverse(buffer, index, 4);
				//f = BitConverter.ToSingle(buffer, index);
				//Array.Reverse(buffer, index, 4);
			}
			else
				f = BitConverter.ToSingle(buffer, index);
			index += 4;
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return f;
	}

	public double PopDouble()
	{
		double d = double.NaN;
		if (index < buffer.Length)
		{
			if ((isBigEndian && BitConverter.IsLittleEndian) || (!isBigEndian && !BitConverter.IsLittleEndian))
			{
				revertTmp[0] = buffer[index + 7];
				revertTmp[1] = buffer[index + 6];
				revertTmp[2] = buffer[index + 5];
				revertTmp[3] = buffer[index + 4];
				revertTmp[4] = buffer[index + 3];
				revertTmp[5] = buffer[index + 2];
				revertTmp[6] = buffer[index + 1];
				revertTmp[7] = buffer[index];
				d = BitConverter.ToDouble(revertTmp, 0);
				//Array.Reverse(buffer, index, 8);
				//d = BitConverter.ToDouble(buffer, index);
				//Array.Reverse(buffer, index, 8);
			}
			else
				d = BitConverter.ToDouble(buffer, index);
			index += 8;
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return d;
	}

	public string PopString()
	{
		string str = string.Empty;
		if (index < buffer.Length)
		{
			int length = (ushort)PopShort();
			if (length > 0)
				str = Encoding.UTF8.GetString(buffer, index, length);
			index += length;
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return str;
	}

	public bool[] PopBooleanArray()
	{
		bool[] bools = null;
		if (index < buffer.Length)
		{
			int length = (ushort)PopShort();
			if (length > 0)
			{
				bools = new bool[length];
				for (int i = 0; i < length; ++i)
				{
					bools[i] = BitConverter.ToBoolean(buffer, index);
					++index;
				}
			}
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return bools;
	}

	public byte[] PopByteArray()
	{
		byte[] bytes = null;
		if (index < buffer.Length)
		{
			int length = (ushort)PopShort();
			if (length > 0)
			{
				bytes = new byte[length];
				Array.Copy(buffer, index, bytes, 0, length);
			}
			index += length;
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return bytes;
	}

	public byte[] PopHugeByteArray()
	{
		byte[] bytes = null;
		if (index < buffer.Length)
		{
			int length = PopInteger();
			if (length > 0)
			{
				bytes = new byte[length];
				Array.Copy(buffer, index, bytes, 0, length);
			}
			index += length;
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return bytes;
	}

	public ByteArrayRef PopHugeByteArrayRef()
	{
		ByteArrayRef res = null;
		if (index < buffer.Length)
		{
			int length = PopInteger();
			if (length > 0)
			{
				res = new ByteArrayRef { array = buffer, start = index, length = length };
			}
			index += length;
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return res;
	}

	public short[] PopShortArray()
	{
		short[] shorts = null;
		if (index < buffer.Length)
		{
			int length = (ushort)PopShort();
			if (length > 0)
			{
				shorts = new short[length];
				for (int i = 0; i < length; ++i)
				{
					shorts[i] = PopShort();
				}
			}
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return shorts;
	}

	public int[] PopIntegerArray()
	{
		int[] ints = null;
		if (index < buffer.Length)
		{
			int length = (ushort)PopShort();
			if (length > 0)
			{
				ints = new int[length];
				for (int i = 0; i < length; ++i)
				{
					ints[i] = PopInteger();
				}
			}
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return ints;
	}

	public long[] PopLongArray()
	{
		long[] longs = null;
		if (index < buffer.Length)
		{
			int length = (ushort)PopShort();
			if (length > 0)
			{
				longs = new long[length];
				for (int i = 0; i < length; ++i)
				{
					longs[i] = PopLong();
				}
			}
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return longs;
	}

	public float[] PopFloatArray()
	{
		float[] floats = null;
		if (index < buffer.Length)
		{
			int length = (ushort)PopShort();
			if (length > 0)
			{
				floats = new float[length];
				for (int i = 0; i < length; ++i)
				{
					floats[i] = PopFloat();
				}
			}
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return floats;
	}

	public double[] PopDoubleArray()
	{
		double[] doubles = null;
		if (index < buffer.Length)
		{
			int length = (ushort)PopShort();
			if (length > 0)
			{
				doubles = new double[length];
				for (int i = 0; i < length; ++i)
				{
					doubles[i] = PopDouble();
				}
			}
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return doubles;
	}

	public string[] PopStringArray()
	{
		string[] strings = null;
		if (index < buffer.Length)
		{
			int length = (ushort)PopShort();
			if (length > 0)
			{
				strings = new string[length];
				for (int i = 0; i < length; ++i)
				{
					strings[i] = PopString();
				}
			}
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError("字节数组剩余字节不足。");
		}
		return strings;
	}
		
	#endregion

	public static short GetShort(byte[] bytes, int offset, bool isBigEndian)
	{
		short s;
		if ((isBigEndian && BitConverter.IsLittleEndian) || (!isBigEndian && !BitConverter.IsLittleEndian))
		{
			revertTmp[0] = bytes[offset + 1];
			revertTmp[1] = bytes[offset];
			s = BitConverter.ToInt16(revertTmp, 0);
			//Array.Reverse(bytes, offset, 2);
			//s = BitConverter.ToInt16(bytes, offset);
			//Array.Reverse(bytes, offset, 2);
		}
		else
			s = BitConverter.ToInt16(bytes, offset);
		return s;
	}
}
