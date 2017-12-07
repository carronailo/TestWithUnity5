using System;
using UnityEngine;

public class ConfigureResourceLoader : ResourceLoaderBase
{
	protected override string IdentifyString { get { return "Config/"; } }
	protected override string CoroutineIdentifyString { get { return "ConfigureResourceLoader."; } }
	protected override Type AssetType { get { return typeof(TextAsset); } }

	public override AsyncResourceRequester LoadResource(string resourceName, Action<AsyncResourceRequester, object> onResourceLoadedCallback, object callbackExtraParam)
	{
		Debug.Log(string.Format("请求加载配置文件资源[{0}]", resourceName));
		return base.LoadResource(resourceName, onResourceLoadedCallback, callbackExtraParam);
	}

	public override void UnloadResource(AsyncResourceRequester requester)
	{
		Debug.Log(string.Format("请求释放配置文件资源[{0}]", requester.resourceName));
		base.UnloadResource(requester);
	}
}

