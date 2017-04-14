using System;

namespace MessageSystem
{
	public interface IMessage
	{
	}

	public static class Message
	{
		public static T Broadcast<T>(T msg) where T : IMessage
		{
#if UNITY_EDITOR
			if(!typeof(T).IsValueType)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("消息结构体[{0}]必须定义为值类型（struct）", typeof(T).Name));
				return msg;
			}
			object[] fieldsDefined = typeof(T).GetFields(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			if(fieldsDefined.Length <= 0)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("消息结构体[{0}]中没有定义任何成员，如果不需要向外通知任何参数的话，使用Notification", typeof(T).Name));
				return msg;
			}
#endif
			msg.Log();
			MessageCentralHub.Instance.DeliverMessage(msg);
			return msg;
		}

		//public virtual long TimeStamp { get; set; }
		//public virtual object Sender { get; set; }

		//override public string ToString()
		//{
		//	return string.Format("消息[{0}] 时间戳[{1}]", GetType().Name, TimeStamp);
		//}

		//public static Message operator >>(Message msg, MessageProcessUnitBase terminal)
		//{
		//    return msg;
		//}
	}

	public static class Message_Extension
	{
		//public static T From<T>(this T msg, object sender) where T : Message
		//{
		//	msg.Sender = sender;
		//	return msg;
		//}

		//public static T To<T>(this T msg, MessageProcessUnitBase terminal) where T : Message
		//{
		//	try
		//	{
		//		MessageCentralHub.Instance.DeliverMessage(msg, terminal);
		//	}
		//	catch (Exception ex)
		//	{
		//		if (LogUtil.ShowException != null)
		//			LogUtil.ShowException(ex);
		//	}
		//	return msg;
		//}

		//public static T To<T>(this T msg, IEnumerable<MessageProcessUnitBase> terminals) where T : Message
		//{
		//	try
		//	{
		//		MessageCentralHub.Instance.DeliverMessage(msg, terminals);
		//	}
		//	catch (Exception ex)
		//	{
		//		if (LogUtil.ShowException != null)
		//			LogUtil.ShowException(ex);
		//	}
		//	return msg;
		//}

		//public static T1 To<T1, T2>(this T1 msg, IEnumerable<T2> terminals)
		//	where T1 : Message
		//	where T2 : MessageProcessUnitBase
		//{
		//	try
		//	{
		//		MessageCentralHub.Instance.DeliverMessage(msg, terminals);
		//	}
		//	catch (Exception ex)
		//	{
		//		if (LogUtil.ShowException != null)
		//			LogUtil.ShowException(ex);
		//	}
		//	return msg;
		//}

		//public static T Broadcast<T>(this T msg) where T : IMessage
		//{
		//	try
		//	{
		//		MessageCentralHub.Instance.DeliverMessage(msg);
		//	}
		//	catch (Exception ex)
		//	{
		//		if (LogUtil.ShowException != null)
		//			LogUtil.ShowException(ex);
		//	}
		//	return msg;
		//}

		//public static Message Broadcast(this Message msg)
		//{
		//	try
		//	{
		//		MessageCentralHub.Instance.DeliverMessage(msg);
		//	}
		//	catch (Exception ex)
		//	{
		//		if (LogUtil.ShowException != null)
		//			LogUtil.ShowException(ex);
		//	}
		//	return msg;
		//}

		//public static T BroadcastExclude<T>(this T msg, MessageProcessUnitBase excludeTerminal) where T : Message
		//{
		//	try
		//	{
		//		MessageCentralHub.Instance.DeliverMessage(msg, excludeTerminal, 投递选项.指定目标为排除对象);
		//	}
		//	catch (Exception ex)
		//	{
		//		if (LogUtil.ShowException != null)
		//			LogUtil.ShowException(ex);
		//	}
		//	return msg;
		//}

		public static T Log<T>(this T msg) where T : IMessage
		{
			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug(string.Format("[本地消息中心]本地消息类型[{0}]", msg.GetType().Name));
			return msg;
		}
	}

}
