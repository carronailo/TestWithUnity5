using UnityEngine;
using System;

public class MonsterModelResourceLoader : ResourceLoaderBase
{
	protected override string IdentifyString { get { return "Character/Monster/"; } }
	protected override string CoroutineIdentifyString { get { return "MonsterModelResourceLoader."; } }
	protected override Type AssetType { get { return null; } }

	public override AsyncResourceRequester LoadResource(string resourceName, Action<AsyncResourceRequester, object> onResourceLoadedCallback, object callbackExtraParam)
	{
		Debug.Log(string.Format("请求加载怪物角色资源[{0}]", resourceName));
		return base.LoadResource(resourceName, onResourceLoadedCallback, callbackExtraParam);
	}

	public override void UnloadResource(AsyncResourceRequester requester)
	{
		Debug.Log(string.Format("请求释放怪物角色资源[{0}]", requester.resourceName));
		base.UnloadResource(requester);
	}
}
