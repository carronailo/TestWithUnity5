using System;

namespace MessageSystem
{
	public delegate void NotificationHandlerDelegate();

	internal class NotificationHandler
	{
		internal int noticeID;
		internal bool isActivated;
		internal NotificationHandlerDelegate Handler { get { return handler; } }
		private NotificationHandlerDelegate handler;

		internal NotificationHandler(int noticeID, NotificationHandlerDelegate handler)
		{
			this.noticeID = noticeID;
			this.handler = handler;
			this.isActivated = true;
		}

		internal void Activate()
		{
			isActivated = true;
		}

		internal void Deactivate()
		{
			isActivated = false;
		}

		internal void Handle(int noticeID)
		{
			if (isActivated && noticeID == this.noticeID)
				handler();
		}
	}

}
