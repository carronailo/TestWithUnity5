using UnityEngine;
using System;

public class ShaderResourceLoader : ResourceLoaderBase
{
	protected override string IdentifyString { get { return ""; } }
	protected override string CoroutineIdentifyString { get { return "ShaderResourceLoader."; } }
	protected override Type AssetType { get { return typeof(AssetBundle); } }

	public override AsyncResourceRequester LoadResource(string resourceName, Action<AsyncResourceRequester, object> onResourceLoadedCallback, object callbackExtraParam)
	{
		Debug.Log(string.Format("请求加载Shader资源[{0}]", resourceName));
		return base.LoadResource(resourceName, onResourceLoadedCallback, callbackExtraParam);
	}

	public override void UnloadResource(AsyncResourceRequester requester)
	{
		Debug.Log(string.Format("请求释放Shader资源[{0}]", requester.resourceName));
		base.UnloadResource(requester);
	}
}
