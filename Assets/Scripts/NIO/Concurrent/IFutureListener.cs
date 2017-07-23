
namespace NIO
{
	public interface IFutureListener<V>
	{
		void OperationComplete(IFuture<V> future);
	}
}
