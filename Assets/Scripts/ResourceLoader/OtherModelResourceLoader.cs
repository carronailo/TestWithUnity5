using System;
using UnityEngine;

public class OtherModelResourceLoader : ResourceLoaderBase
{
	protected override string IdentifyString { get { return "Character/Other/"; } }
	protected override string CoroutineIdentifyString { get { return "OtherModelResourceLoader."; } }
	protected override Type AssetType { get { return typeof(GameObject); } }

	public override AsyncResourceRequester LoadResource(string resourceName, Action<AsyncResourceRequester, object> onResourceLoadedCallback, object callbackExtraParam)
	{
		Debug.Log(string.Format("请求加载其他角色资源[{0}]", resourceName));
		return base.LoadResource(resourceName, onResourceLoadedCallback, callbackExtraParam);
	}

	public override void UnloadResource(AsyncResourceRequester requester)
	{
		Debug.Log(string.Format("请求释放其他角色资源[{0}]", requester.resourceName));
		base.UnloadResource(requester);
	}
}
