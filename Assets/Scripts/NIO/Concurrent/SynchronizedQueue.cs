/**
 * @TODO: 基于.Net3.5版本写的同步队列，使用了.Net提供的Queue.Synchronized，效率上不如.Net4的ConcurrentQueue，更新.Net版本时考虑换过来
 **/
namespace NIO
{
	using System.Collections;

	public class SynchronizedQueue<T> : IQueue<T>
	{
		Queue rawQueue = null;

		public SynchronizedQueue()
		{
			rawQueue = Queue.Synchronized(new Queue());
		}

		public int Count { get { return rawQueue.Count; } }

		public bool IsEmpty { get { return rawQueue.Count <= 0; } }

		public bool TryEnqueue(T item)
		{
			if (item == null)
				return false;
			try
			{
				rawQueue.Enqueue(item);
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool TryDequeue(out T item)
		{
			item = default(T);
			if (IsEmpty)
				return false;
			try
			{
				item = (T)rawQueue.Dequeue();
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool TryPeek(out T item)
		{
			item = default(T);
			if (IsEmpty)
				return false;
			try
			{
				item = (T)rawQueue.Peek();
			}
			catch
			{
				return false;
			}
			return true;
		}

		public void Clear()
		{
			try
			{
				rawQueue.Clear();
			}
			catch { }
		}

	}
}
