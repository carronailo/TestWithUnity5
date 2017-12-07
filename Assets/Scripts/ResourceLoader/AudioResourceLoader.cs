using System;
using UnityEngine;

public class AudioResourceLoader : ResourceLoaderBase
{
	protected override string IdentifyString { get { return "Sounds/"; } }
	protected override string CoroutineIdentifyString { get { return "AudioResourceLoader."; } }
	protected override Type AssetType { get { return typeof(GameObject); } }

	public override AsyncResourceRequester LoadResource(string resourceName, Action<AsyncResourceRequester, object> onResourceLoadedCallback, object callbackExtraParam)
	{
		Debug.Log(string.Format("请求加载音频资源[{0}]", resourceName));
//#if UNITY_EDITOR
//		if (DebugParameters.enableDebug && DebugResourceManager.Instance != null)
//		{
//			AsyncResourceRequester res =
//				new AsyncResourceRequester
//				{
//					progress = Constants.oneFloat,
//					done = true,
//					resourceName = resourceName,
//					resourceHolder = new ResourceHolder(resourceName, new AssetPackage { mainAsset = DebugResourceManager.Instance.GetResource(EResourceType.Audio, resourceName) }, null),
//					resourceHolderGenerateCallback = onResourceLoadedCallback,
//					callbackExtraParam = callbackExtraParam
//				};
//			if (onResourceLoadedCallback != null)
//				DelayedFunctionCaller.DelayedCall(0, onResourceLoadedCallback, res, callbackExtraParam);
//			return res;
//		}
//		else
//#endif
			return base.LoadResource(resourceName, onResourceLoadedCallback, callbackExtraParam);
	}

	public override void UnloadResource(AsyncResourceRequester requester)
	{
		Debug.Log(string.Format("请求释放音频资源[{0}]", requester.resourceName));
//#if UNITY_EDITOR
//		if (DebugParameters.enableDebug && DebugResourceManager.Instance != null)
//		{
//		}
//		else
//#endif
			base.UnloadResource(requester);
	}
}

