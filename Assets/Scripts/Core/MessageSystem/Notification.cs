using System.Collections.Generic;
using UnityEngine;

namespace MessageSystem
{
	public static class Notification
	{
		private static int currentNoticeID = -1;

		public static void SendTo(int noticeID, MessageProcessUnitBase terminal)
		{
			try
			{
				currentNoticeID = noticeID;
				NotificationCentralHub.Instance.DeliverNotification(noticeID, terminal);
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void SendTo(int noticeID, IEnumerable<MessageProcessUnitBase> terminals)
		{
			try
			{
				currentNoticeID = noticeID;
				NotificationCentralHub.Instance.DeliverNotification(noticeID, terminals);
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void SendTo<T>(int noticeID, IEnumerable<T> terminals)
			where T : MessageProcessUnitBase
		{
			try
			{
				NotificationCentralHub.Instance.DeliverNotification(noticeID, terminals);
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void Broadcast(int noticeID)
		{
			try
			{
				NotificationCentralHub.Instance.DeliverNotification(noticeID);
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void BroadcastExclude(int noticeID, MessageProcessUnitBase excludeTerminal)
		{
			try
			{
				NotificationCentralHub.Instance.DeliverNotification(noticeID, excludeTerminal, 投递选项.指定目标为排除对象);
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public new static string ToString()
		{
			return string.Format("通知[{0}]", currentNoticeID);
		}


	}
}
