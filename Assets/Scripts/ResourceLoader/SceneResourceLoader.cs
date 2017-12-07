using System;
using UnityEngine;

public class SceneResourceLoader : ResourceLoaderBase
{
	protected override string IdentifyString { get { return "Scene/"; } }
	protected override string CoroutineIdentifyString { get { return "SceneResourceLoader."; } }
	protected override Type AssetType { get { return typeof(AssetBundle); } }

	public override AsyncResourceRequester LoadResource(string resourceName, Action<AsyncResourceRequester, object> onResourceLoadedCallback, object callbackExtraParam)
	{
		Debug.Log(string.Format("请求加载场景资源[{0}]", resourceName));
		AsyncResourceRequester res = null;
		if (requestCacheTable != null && requestCacheTable.Count > 0)
		{
			Debug.LogError("当前正在加载场景资源，在其加载完成并释放之前不允许加载其他场景资源");
#if UNITY_EDITOR
			foreach(string str in requestCacheTable.Keys)
			{
				Debug.Log("当前正在加载的场景资源是: " + str);
			}
#endif
			return res;
		}
		if(resourceTable != null && resourceTable.Count > 0)
		{
			Debug.LogError("当前已经加载场景资源，在其释放之前不允许加载其他场景资源");
#if UNITY_EDITOR
			foreach (string str in resourceTable.Keys)
			{
				Debug.Log("当前已经加载的场景资源是: " + str);
			}
#endif
			return res;
		}
		res = base.LoadResource(resourceName, onResourceLoadedCallback, callbackExtraParam);
		return res;
	}

	public override void UnloadResource(AsyncResourceRequester requester)
	{
		Debug.Log(string.Format("请求释放场景资源[{0}]", requester.resourceName));
		base.UnloadResource(requester);
	}
}

