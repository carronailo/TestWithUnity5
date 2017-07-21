
namespace NIO
{
	using UnityEngine;

	public class ExampleNIO : MonoBehaviour
	{
		Bootstrap bootstrap = null;

		void Start()
		{
			IEventExecutorGroup group = new DefaultEventExecutorGroup();
			bootstrap = new Bootstrap();
			bootstrap.Group(group);
		}

		void OnGUI()
		{
			if(GUILayout.Button("Connect", GUILayout.Width(200f), GUILayout.Height(75f)))
			{
				bootstrap.ConnectAsync("127.0.0.1", 8080);
			}
			if(GUILayout.Button("Disconnect", GUILayout.Width(200f), GUILayout.Height(75f)))
			{
				bootstrap.DisconnectAsync();
			}
		}
	}
}

