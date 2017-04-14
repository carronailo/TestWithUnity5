using System.Collections;

namespace Networks
{
	/// <summary>
	/// 环形缓存，采用头出尾入的队列式操作，如果T类型为自定义类型，那么需要为RingBuffer设置一个比较器用来对比两个T类型数据是否相等，
	/// 否则RingBuffer将会对比T类型引用地址是否相等，比较器需实现System.Collections.Generic的IComparer接口
	/// </summary>
	public class RingBuffer<T>
	{
		/// <summary>
		/// RingBuffer是否支持自动扩容
		/// </summary>
		public bool AutoResize { get; set; }
		/// <summary>
		/// RingBuffer支持自动扩容时，最大扩容的上限，当扩容后容量会超出CapacityLimit时，扩容失败，设置为0表示无限制
		/// </summary>
		public int CapacityLimit { get; set; }

		private T[] buffer = null;
		private int head = 0;
		private int tail = 0;
		private IComparer comparer = null;

		/// <summary>
		/// RingBuffer有效数据的长度
		/// </summary>
		public int Length
		{
			get
			{
				if (buffer == null || buffer.Length == 0)
					return 0;
				if (tail >= head)
					return tail - head;
				else
					return tail - head + buffer.Length;
			}
		}

		/// <summary>
		/// RingBuffer最大能够容纳的数据长度
		/// </summary>
		public int Capacity
		{
			get
			{
				if (buffer == null || buffer.Length == 0)
					return 0;
				return buffer.Length - 1;
			}
		}

		/// <summary>
		/// RingBuffer是否已经满了
		/// </summary>
		public bool IsFull
		{
			get
			{
				if (buffer == null || buffer.Length == 0)
					return true;
				return (head == 0) ? (tail == buffer.Length - 1) : (tail + 1 == head);
			}
		}

		/// <summary>
		/// RingBuffer是否是空的
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				if (buffer == null || buffer.Length == 0)
					return false;
				return head == tail;
			}
		}

		/// <summary>
		/// 构造一个RingBuffer
		/// </summary>
		/// <param name="capacity">RingBuffer内部将分配一个容量为capacity的数据数组作为缓存</param>
		public RingBuffer(int capacity, IComparer comparer = null)
		{
			ResetBuffer(capacity);
			this.comparer = comparer;
		}

		/// <summary>
		/// 构造一个RingBuffer
		/// </summary>
		/// <param name="buffer">RingBuffer内部将使用buffer对应的数据数组作为缓存，不会另外分配内存</param>
		public RingBuffer(T[] buffer, IComparer comparer = null)
		{
			ResetBuffer(buffer);
			this.comparer = comparer;
		}

		/// <summary>
		/// 重置RingBuffer的缓存
		/// </summary>
		/// <param name="capacity">RingBuffer将重新分配一个容量为capacity的数据数组作为新的缓存，老的缓存将随GC被系统收回，另外读取标识位也将重置为初始值</param>
		public void ResetBuffer(int capacity)
		{
			buffer = new T[capacity + 1];
			head = tail = 0;
		}

		/// <summary>
		/// 重置RingBuffer的缓存
		/// </summary>
		/// <param name="buffer">RingBuffer会将内部的缓存替换为buffer指定的数据数组，接下来的读取和插入都将在这个数据数组上操作，另外读取标识位也将重置为初始值</param>
		public void ResetBuffer(T[] buffer)
		{
			this.buffer = buffer;
			head = tail = 0;
		}

		/// <summary>
		/// 向RingBuffer中压入数据，数据来源自data指定的数据数组，将会完全读取这个数据数组的数据并全部存放在RingBuffer缓存的末尾
		/// </summary>
		/// <param name="data">压入的数据来源</param>
		/// <returns>true - 压入数据的过程中没有出错；false - 压入数据的过程有错误，错误内容参看控制台数据</returns>
		public bool TryPush(T[] data)
		{
			if (data == null)
				return false;

			return TryPush(data, 0, data.Length);
		}

		/// <summary>
		/// 向RingBuffer中压入数据，数据来源自data指定的数据数组，从数组的第offset个数据开始读取，一共读取count个数据，这些数据将存放在RingBuffer缓存的末尾
		/// </summary>
		/// <param name="data">压入的数据来源</param>
		/// <param name="offset">将从data数组的第offset个数据开始读取</param>
		/// <param name="count">将一共读取count个数据</param>
		/// <returns>true - 压入数据的过程中没有出错；false - 压入数据的过程有错误，错误内容参看控制台数据</returns>
		public bool TryPush(T[] data, int offset, int count)
		{
			if (data == null || offset < 0 || count <= 0 || offset + count > data.Length)
			{
				// @TODO: 修改为抛异常
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("传入的数据无法获取");
				return false;
			}
			if(buffer == null || buffer.Length == 0)
			{
				// @TODO: 修改为抛异常
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("RingBuffer未准备好");
				return false;
			}
			if(Length + count > Capacity)
			{
				if(!AutoResize)
				{
					// @TODO: 修改为抛异常
					if (LogUtil.ShowError != null)
						LogUtil.ShowError("RingBuffer无法容纳这么多数据");
					return false;
				}
				else
				{
					// 自动增长容量
					if(!IncreaseCapacity())
					{
						// @TODO: 修改为抛异常
						if (LogUtil.ShowError != null)
							LogUtil.ShowError("RingBuffer无法容纳这么多数据，自动扩容失败");
						return false;
					}
				}
			}

			//for (int i = 0; i < count; ++i)
			//{
			//	T b = data[offset + i];
			//	buffer[tail] = b;
			//	tail = (tail + 1) % buffer.Length;
			//}

			if(buffer.Length - tail >= count)
				CopyData(typeof(T), data, offset, buffer, tail, count);
			else
			{
				int pushBytes = System.Math.Min(count, buffer.Length - tail);
				CopyData(typeof(T), data, offset, buffer, tail, pushBytes);
				if(pushBytes < count)
					CopyData(typeof(T), data, offset + pushBytes, buffer, 0, count - pushBytes);
			}
			tail = (tail + count) % buffer.Length;

			return true;
		}

		/// <summary>
		/// 从RingBuffer中抛出所有数据，数据存入outputBuffer指定的数据数组中，该数据数组必须事前分配好能够容纳RingBuffer全部数据的内存空间
		/// </summary>
		/// <param name="outputBuffer">抛出的数据全部放入outputBuffer指定的数据数组中</param>
		/// <returns>true - 抛出数据的过程中没有错误；false - 抛出数据的过程中出错，错误内容参看控制台数据</returns>
		public bool TryPull(ref T[] outputBuffer)
		{
			return TryPull(Length, ref outputBuffer);
		}

		/// <summary>
		/// 从RingBuffer中抛出数据，一共抛出count个数据的数据，并存入outputBuffer指定的数据数组中，这个数据数组需要事先分配好能够存放count数据的内存空间，抛出的数据来自RingBuffer缓存的开头
		/// </summary>
		/// <param name="count">总共抛出count数据的数据</param>
		/// <param name="outputBuffer">抛出的数据都存入outputBuffer指定的数据数组中</param>
		/// <returns>true - 抛出数据的过程中没有错误；false - 抛出数据的过程中出错，错误内容参看控制台数据</returns>
		public bool TryPull(int count, ref T[] outputBuffer)
		{
			if (count <= 0)
				return true;

			if(outputBuffer == null || outputBuffer.Length < count)
			{
				// @TODO: 修改为抛异常
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("传入的数组无法接收数据");
				return false;
			}
			if(Length < count)
			{
				// @TODO: 修改为抛异常
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("RingBuffer中的数据不够，期望{0},实际只有{1}", count, Length));
				return false;
			}

			//for (int i = 0; i < count; ++i)
			//{
			//	T b = buffer[head];
			//	outputBuffer[i] = b;
			//	head = (head + 1) % buffer.Length;
			//}

			if (tail >= head)
				CopyData(typeof(T), buffer, head, outputBuffer, 0, count);
			else
			{
				int pullBytes = System.Math.Min(count, buffer.Length - head);
				CopyData(typeof(T), buffer, head, outputBuffer, 0, pullBytes);
				if(pullBytes < count)
					CopyData(typeof(T), buffer, 0, outputBuffer, pullBytes, count - pullBytes);
			}
			head = (head + count) % buffer.Length;

			return true;
		}

		/// <summary>
		/// 查看RingBuffer缓存中最开头的一个数据，如果成功，该数据的内容将存入result中，此操作不会造成RingBuffer读写标识位的变化
		/// </summary>
		/// <param name="result">查看到的结果数据</param>
		/// <returns>true - 查看数据过程中没有出错；false - 查看数据过程中出错，错误内容参看控制台数据</returns>
		public bool Peek(out T result)
		{
			return Peek(0, out result);
		}

		/// <summary>
		/// 查看RingBuffer缓存中开头的第offset个数据，如果成功，该数据的内容将存入result中，此操作不会造成RingBuffer读写标识位的变化
		/// </summary>
		/// <param name="offset">将查看到RingBuffer缓存中第offset个数据</param>
		/// <param name="result">查看到的结果数据</param>
		/// <returns>true - 查看数据过程中没有出错；false - 查看数据过程中出错，错误内容参看控制台数据</returns>
		public bool Peek(int offset, out T result)
		{
			result = default(T);
			if (Length < offset)
			{
				// @TODO: 修改为抛异常
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("RingBuffer中的数据不够");
				return false;
			}
			result = buffer[(head + offset) % buffer.Length];
			return true;
		}

		/// <summary>
		/// 在RingBuffer中查找数据value的首次出现的位置，以0起始的下标形式给出
		/// </summary>
		/// <param name="value">要查找的数据</param>
		/// <returns>数据value在RingBuffer中的位置，以0起始的下标形式给出，如果没有找到则返回 -1</returns>
		public int FindFirstIndexOf(T value)
		{
			if (IsEmpty)
				return -1;
			int i = head;
			// 首先处理从head到buffer末尾的数据
			for (; i < buffer.Length; ++i)
			{
				if (IsValueEqual(buffer[i], value))
					return i - head;
				if (i == tail)
					break;
			}
			// 然后如果有必要，处理越过了buffer末尾（也就是buffer开头到head）的数据
			if (i != tail)
			{
				i = 0;
				for (; i < tail; ++i)
				{
					if (IsValueEqual(buffer[i], value))
						return i + buffer.Length - head;
				}
			}
			return -1;
		}

		/// <summary>
		/// 从RingBuffer中抛弃count个数据
		/// </summary>
		/// <param name="count">将总共抛弃count个数据</param>
		/// <returns>true - 抛弃数据过程中没有出错；false - 抛弃数据过程中出错，错误内容参看控制台数据</returns>
		public bool Discard(int count)
		{
			if(Length < count)
			{
				// @TODO: 修改为抛异常
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("RingBuffer中的数据不够");
				return false;
			}
			head = (head + count) % buffer.Length;
			return true;
		}

		/// <summary>
		/// 将RingBuffer中所有数据都抛弃，RingBuffer的长度将归0
		/// </summary>
		/// <returns>true - 抛弃数据过程中没有出错；false - 抛弃数据过程中出错，错误内容参看控制台数据</returns>
		public bool DiscardAll()
		{
			head = tail;
			return true;
		}

		/// <summary>
		/// 从RingBuffer开头开始，逐个丢弃数据，直到开头第一个数据是value为止（value数据不会丢弃），如果没有找到该数据那么RingBuffer的全部内容都将被抛弃
		/// </summary>
		/// <param name="value">作为停止标记的数据</param>
		/// <returns>true - 抛弃数据过程中没有出错；false - 丢弃数据时失败，出现这种情况时，RingBuffer内的数据已经不能再信任，最好调用DiscardAll全部丢弃掉</returns>
		public bool DiscardToFirstIndexOf(T value)
		{
			int idx = FindFirstIndexOf(value);
			if (idx == -1)
				return DiscardAll();
			else
				return Discard(idx);
		}

		/// <summary>
		/// 从RingBuffer开头开始，逐个丢弃数据，直到开头第一个数据是value1，第二个数据是value2为止（value1和value2这两个数据不会抛弃），如果没有找到那么RingBuffer中的全部内容都会被抛弃
		/// </summary>
		/// <param name="value1">作为停止标记的第一个数据</param>
		/// <param name="value2">作为停止标记的第二个数据</param>
		/// <returns>true - 抛弃数据过程中没有出错；false - 丢弃数据时失败，出现这种情况时，RingBuffer内的数据已经不能再信任，最好调用DiscardAll全部丢弃掉</returns>
		public bool DiscardToFirstIndexOf(T value1, T value2)
		{
			int idx = FindFirstIndexOf(value1);
			if (idx == -1)
				return DiscardAll();	// 没找到value1，那么全buffer都丢弃
			else if (Discard(idx))	// 找到了value1，那么把buffer中value1之前的数据丢弃掉
			{
				// 判断value2
				T next;
				if(!Peek(1, out next) || IsValueEqual(next, value2))
				{
					// 数据不够判断value2，或者next满足value2，直接返回
					return true;
				}
				else
				{
					// 否则，说明不满足value2，把找到value1的数据丢弃掉，继续找
					Discard(1);
					return DiscardToFirstIndexOf(value1, value2);
				}
			}
			else
				return false;	// 丢弃时失败
		}

		/// <summary>
		/// 清空RingBuffer的内容
		/// </summary>
		public void Clear()
		{
			head = tail = 0;
		}

		bool IsValueEqual(T value1, T value2)
		{
			if (comparer == null)
				return value1.Equals(value2);
			else
				return comparer.Compare(value1, value2) == 0;

		}

		bool IncreaseCapacity()
		{
			// 计算扩容后容量，每次增长都是乘2
			int newCapacity = (buffer.Length - 1) << 1;
			// 如果设置了容量上限，而且扩容后容量会超过上限，那么失败
			if (CapacityLimit > 0 && newCapacity > CapacityLimit)
				return false;
			// 每次增长都是乘2
			T[] newBuffer = new T[newCapacity + 1]; 
			// 数据拷贝
			int copyTotalBytes = Length;
			if (tail >= head)
				CopyData(typeof(T), buffer, head, newBuffer, 0, copyTotalBytes);
			else
			{
				int copyBytes = System.Math.Min(copyTotalBytes, buffer.Length - head);
				CopyData(typeof(T), buffer, head, newBuffer, 0, copyBytes);
				if (copyBytes < copyTotalBytes)
					CopyData(typeof(T), buffer, 0, newBuffer, copyBytes, copyTotalBytes - copyBytes);
			}
			head = 0;
			tail = Length;
			buffer = newBuffer;
			return true;
		}

		void CopyData(System.Type dataType, System.Array srcArray, int srcOffset, System.Array dstArray, int dstOffset, int count)
		{
			if (dataType == typeof(byte))
				System.Buffer.BlockCopy(srcArray, srcOffset, dstArray, dstOffset, count);
			else
				System.Array.Copy(srcArray, srcOffset, dstArray, dstOffset, count);
		}
	}
}
