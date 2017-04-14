
namespace Networks
{
	public interface IConnectionManager
	{
		void Send(int classID, int methodID, byte[] bytes);
	}
}
