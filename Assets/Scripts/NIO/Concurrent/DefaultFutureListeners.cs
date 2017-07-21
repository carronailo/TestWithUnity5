
namespace NIO
{
	using System;

	public class DefaultFutureListeners<T>
	{
		private IFutureListener<T>[] listeners;
		private int size;

		public DefaultFutureListeners(IFutureListener<T> first, IFutureListener<T> second)
		{
			listeners = new IFutureListener<T>[2];
			listeners[0] = first;
			listeners[1] = second;
			size = 2;
		}

		public IFutureListener<T>[] Listeners { get { return listeners; } }

		public int Size { get { return size; } }

		public void Add(IFutureListener<T> listener)
		{
			IFutureListener<T>[] listeners = this.listeners;
			if (size == listeners.Length)
			{
				this.listeners = new IFutureListener<T>[size << 1];
				Array.Copy(listeners, this.listeners, size);
			}
			this.listeners[size] = listener;
			++size;
		}

		public void Remove(IFutureListener<T> listener)
		{
			IFutureListener<T>[] listeners = this.listeners;
			for(int i = 0; i < size; ++i)
			{
				if(listeners[i] == listener)
				{
					int listenersToMove = size - i - 1;
					if (listenersToMove > 0)
						Array.Copy(listeners, i + 1, this.listeners, i, listenersToMove);
					this.listeners[--size] = null;

					return;
				}
			}
		}
	}
}
