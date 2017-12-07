using UnityEngine;
using System;

public class PlayerModelResourceLoader : ResourceLoaderBase
{
	protected override string IdentifyString { get { return "Character/"; } }
	protected override string CoroutineIdentifyString { get { return "PlayerCharacterResourceLoader."; } }
	protected override Type AssetType { get { return typeof(GameObject); } }

	public override AsyncResourceRequester LoadResource(string resourceName, Action<AsyncResourceRequester, object> onResourceLoadedCallback, object callbackExtraParam)
	{
		Debug.Log(string.Format("请求加载玩家角色资源[{0}]", resourceName));
		return base.LoadResource(resourceName, onResourceLoadedCallback, callbackExtraParam);
	}

	public override void UnloadResource(AsyncResourceRequester requester)
	{
		Debug.Log(string.Format("请求释放玩家角色资源[{0}]", requester.resourceName));
		base.UnloadResource(requester);
	}
}
