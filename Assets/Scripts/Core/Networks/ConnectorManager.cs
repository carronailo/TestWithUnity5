using UnityEngine;
using System.Collections;

namespace Networks
{
	public enum EProtocolType
	{
		TCP,
		UDP,
		SSH,
		HTTP,
		HTTPS,
		FTP,
	}

	public class ConnectorManager
	{
		public static ConnectorManager Instance { get { if (_instance == null) _instance = new ConnectorManager(); return _instance; } }
		private static ConnectorManager _instance = null;

		public IConnector CreateConnector(EProtocolType type)
		{
			switch (type)
			{
				case EProtocolType.TCP:
					return new TCPConnector();
				case EProtocolType.UDP:
					return new UDPConnector();
				default:
					LogUtil.ShowWarning("暂不支持的类型");
					return null;
			}
		}
	}
}

