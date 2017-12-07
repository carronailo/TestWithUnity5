using System;
using UnityEngine;

public class PanelResourceLoader : ResourceLoaderBase
{
    protected override string IdentifyString { get { return "UI/Panel/"; } }
    protected override string CoroutineIdentifyString { get { return "PanelResourceLoader."; } }
    protected override Type AssetType { get { return typeof(GameObject); } }

    public override AsyncResourceRequester LoadResource(string resourceName, Action<AsyncResourceRequester, object> onResourceLoadedCallback, object callbackExtraParam)
    {
        Debug.Log(string.Format("请求加载面板[{0}]", resourceName));
//#if UNITY_EDITOR
//        if (DebugParameters.enableDebug && DebugResourceManager.Instance != null)
//        {
//            AsyncResourceRequester res =
//                new AsyncResourceRequester
//                {
//                    progress = Constants.oneFloat,
//                    done = true,
//                    resourceName = resourceName,
//                    resourceHolder = new ResourceHolder(resourceName, new AssetPackage { mainAsset = DebugResourceManager.Instance.GetResource(EResourceType.Panel, resourceName) }, null),
//                    resourceHolderGenerateCallback = onResourceLoadedCallback,
//                    callbackExtraParam = callbackExtraParam
//                };
//            if (onResourceLoadedCallback != null)
//                DelayedFunctionCaller.DelayedCall(0, onResourceLoadedCallback, res, callbackExtraParam);
//            return res;
//        }
//        else
//#endif
            return base.LoadResource(resourceName, onResourceLoadedCallback, callbackExtraParam);
    }

    public override void UnloadResource(AsyncResourceRequester requester)
    {
        Debug.Log(string.Format("请求释放面板[{0}]", requester.resourceName));
//#if UNITY_EDITOR
//        if (DebugParameters.enableDebug && DebugResourceManager.Instance != null)
//        {
//        }
//        else
//#endif
            base.UnloadResource(requester);
    }
}
