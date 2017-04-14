
namespace Networks
{
	public abstract class NetworkMessageProcessUnitBase
	{
		protected NetworkMessageProcessUnitBase()
		{
			OnRegisteringRemoteMessageHandler();
		}

		/// <summary>
		/// 需要显示调用Release来释放MessageProcessUnitBase占用的资源
		/// </summary>
		public void Release()
		{
			NetworkMessageCentralHub.Instance.UnregisterAllHandlersBelongToModule(this);
		}

		abstract protected void OnRegisteringRemoteMessageHandler();

		//TODO: 把这里的classID和functionID都改成结构或者枚举
		protected void TryRegisterRemoteMessageHandler<T>(int classID, int functionID, NetworkMessageHandlerDelegate<T> handler) where T : INetworkMessage
		{
			if (handler != null)
			{
				// Register to remote message central hub
				NetworkMessageCentralHub.Instance.RegisterHandler(classID, functionID, this, handler);
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("模块[{0}]注册消息的调用中有无效参数：消息类型[{1}] 监视器委托[{2}]",
						GetType().Name, typeof(T).Name, StringUtility.ToString(handler)));
			}
#endif
		}
	}
}
