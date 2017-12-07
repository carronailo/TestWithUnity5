using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityObject = UnityEngine.Object;

public class AssetPackage
{
	public UnityObject mainAsset = null;
	public UnityObject[] reletiveAssets = null;
	public AssetBundle rawAssetbundle = null;
}

public abstract class ResourceLoaderBase : IResourceLoader
{
	protected virtual string IdentifyString { get { return "Base/"; } set { } }
	protected virtual string ExtensionString { get { return ".bytes"; } set { } }
	protected virtual string CoroutineIdentifyString { get { return "ResourceLoaderBase."; } set { } }
	protected virtual Type AssetType { get { return typeof(UnityObject); } set { } }

	protected Dictionary<string, ResourceHolder> resourceTable = null;
	protected Dictionary<object, int> resourceReferenceTable = null;
	protected Dictionary<string, List<AsyncResourceRequester>> requestCacheTable = null;
	protected Queue<AsyncResourceRequester> requestPool = null;     // 回收释放掉的AsyncResourceRequester，避免每次申请资源时都new

	public virtual AsyncResourceRequester LoadResource(string resourceName, Action<AsyncResourceRequester, object> onResourceLoadedCallback, object callbackExtraParam)
	{
		ResourceHolder resourceHolder = null;
		AsyncResourceRequester res = null;
		if (resourceTable != null && resourceTable.TryGetValue(resourceName, out resourceHolder))
		{
			// 请求的资源已经加载上来了，直接增加资源引用计数，生成请求器，并立刻调用资源加载成功的回调
			IncreaseResourceReferenceCount(resourceName);
			if (requestPool != null && requestPool.Count > 0)
				res = requestPool.Dequeue();
			else
				res = new AsyncResourceRequester();
			res.Init(resourceName, resourceHolder, onResourceLoadedCallback, callbackExtraParam);
			res.progress = Constants.oneFloat;
			res.done = true;
			if (onResourceLoadedCallback != null)
				onResourceLoadedCallback(res, callbackExtraParam);
			//// 最好延迟调用，否则会导致先调用了回调，然后函数才返回的问题
			//if (onResourceLoadedCallback != null)
			//	DelayedFunctionCaller.DelayedCall(0, onResourceLoadedCallback, res, callbackExtraParam);
		}
		else
		{
			// 请求的资源还没有加载上来，调用异步加载资源的逻辑，生成请求器，将请求器加入缓存
			LoadResourceAsset(resourceName);
			if (requestPool != null && requestPool.Count > 0)
				res = requestPool.Dequeue();
			else
				res = new AsyncResourceRequester();
			res.Init(resourceName, null, onResourceLoadedCallback, callbackExtraParam);
			if (requestCacheTable == null)
				requestCacheTable = new Dictionary<string, List<AsyncResourceRequester>>();
			CollectionUtil.AddIntoTable(resourceName, res, requestCacheTable);
		}
		return res;
	}

	public virtual void UnloadResource(AsyncResourceRequester requester)
	{
		if (requester.done && !requester.fail && !requester.released)
			requester.resourceHolder.Release();
		else
			CollectionUtil.RemoveFromTable(requester.resourceName, requester, requestCacheTable);
		requester.released = true;
		requester.resourceHolder = null;
		requester.resourceHolderGenerateCallback = null;
		requester.callbackExtraParam = null;
		if (requestPool == null)
			requestPool = new Queue<AsyncResourceRequester>();
		requestPool.Enqueue(requester);
	}

	/// <summary>
	/// 卸载资源，并返回资源是否已经被删除
	/// </summary>
	/// <param name="resourceHolder">持有要卸载的资源的ResourceHolder</param>
	/// <returns>
	/// 资源已经被删除 true
	/// 资源还未被删除 false
	/// </returns>
	protected bool UnloadResource(ResourceHolder resourceHolder)

	{
		bool res = false;
		string resourceName = resourceHolder.resourceName;
		object resource = resourceHolder.resource;
		if (resource != null)
		{
			if (DecreaseResourceReferenceCount(resourceName))
			{
				// 引用计数已经降为零，删除资源
				resourceReferenceTable.Remove(resource);
				resourceTable.Remove(resourceName);
				UnloadResourceObject(resourceName, resource);
				res = true;
			}
		}
		else
		{
			if(LogUtil.ShowError != null)
				LogUtil.ShowError(string.Format("要释放的资源[{0}]不存在", resourceName));
			res = true;
		}
		return res;
	}

	/// <summary>
	/// 从内存里删除这个资源
	/// </summary>
	/// <param name="resourceName">资源名</param>
	/// <param name="resourceObject">资源实例</param>
	protected virtual void UnloadResourceObject(string resourceName, object resourceObject)
	{
		if(AssetType != typeof(AssetBundle))
		{
			AssetPackage asset = (AssetPackage)resourceObject;
			if (asset != null)
			{
				if(asset.mainAsset != null)
				{
					if (asset.mainAsset is Component || asset.mainAsset is GameObject)
						UnityObject.DestroyImmediate(asset.mainAsset, true);
					else
						Resources.UnloadAsset(asset.mainAsset);
				}
				if(asset.reletiveAssets != null)
				{
					for (int i = 0; i < asset.reletiveAssets.Length; ++i)
					{
						UnityObject reletiveAsset = asset.reletiveAssets[i];
						if (reletiveAsset == null)
							continue;
						if (reletiveAsset is Component || reletiveAsset is GameObject)
							UnityObject.DestroyImmediate(reletiveAsset, true);
						else if (reletiveAsset is Shader)
						{
							if (!ResourceManager.Instance.IsCommonResource(reletiveAsset.name))
								Resources.UnloadAsset(reletiveAsset);
						}
						else
							Resources.UnloadAsset(reletiveAsset);
					}
				}
				//asset.rawAssetbundle.Unload(true);
				asset = null;
				// 卸载不再使用的资源，推迟到场景卸载
				//Resources.UnloadUnusedAssets();
				if(LogUtil.ShowInfo != null)
					LogUtil.ShowInfo(string.Format("释放资源[{0}]", resourceName));
			}
			else if(LogUtil.ShowError != null)
				LogUtil.ShowError("这个资源不是AssetPackage类型的！");
		}
		else
		{
			AssetBundle asset = (AssetBundle)resourceObject;
			if (asset != null)
			{
				// 场景比较特殊，场景的资源释放并不是不再使用这部分资源了，而是把多余的资源释放掉，而已经加载进内存的实例是需要保留的
				// 场景资源的完全释放由Unity场景切换（卸载）时自动完成
				asset.Unload(false);
				asset = null;
				// 卸载不再使用的资源，推迟到场景卸载
				//Resources.UnloadUnusedAssets();
				if (LogUtil.ShowInfo != null)
					LogUtil.ShowInfo(string.Format("释放资源[{0}]", resourceName));
			}
			else if (LogUtil.ShowError != null)
				LogUtil.ShowError("这个资源不是AssetBundle类型的！");
		}
	}

	/// <summary>
	/// 开启异步加载资源的携程，并决定资源从外部文件加载还是从内部资源加载
	/// </summary>
	/// <param name="resourceName">资源名</param>
	protected void LoadResourceAsset(string resourceName)
	{
		string assetName = IdentifyString + resourceName;
		string internalAssetFile = string.Format("{0}{1}{2}", Constants.internalResourcePath, assetName, ExtensionString);
		string externalAssetFile = string.Format("{0}{1}{2}", Constants.externalResourcePath, assetName, ExtensionString);
		bool externalFileExists = File.Exists(externalAssetFile);

#if UNITY_EDITOR && !UNITY_STANDALONE_WIN
		string assetFileEditor = string.Format("{0}{1}{2}", Constants.externalResourcePath_Editor, assetName, ExtensionString);
		if (File.Exists(assetFileEditor))
		{
			externalFileExists = true;
			externalAssetFile = assetFileEditor;
		}
		else if(File.Exists(externalAssetFile))
			externalFileExists = true;
#endif

		// 不会执行相同识别字符串的携程，所以在资源加载完之前重复请求执行携程并不会导致资源重复加载
		if (externalFileExists && !SystemParameters.forceInternal)
		{
			// 如果存在下载的资源文件，那么优先加载下载的资源
			if (LogUtil.ShowInfo != null)
				LogUtil.ShowInfo(string.Format("请求的资源[{0}]将从外部文件加载", resourceName));
			// 因为创建协程时会确保一个名字只能同时拥有一个协程，所以如果当前已经有一个同名资源在加载中的话，并不会重复加载
			Coroutines.StartMyCoroutine(CoroutineIdentifyString + resourceName, LoadAssetFromExternalFile(resourceName, externalAssetFile), RefreshResourceLoadingProgress, resourceName);
		}
		else
		{
			// 如果不存在，那么加载游戏包内自带的资源
			if (LogUtil.ShowInfo != null)
				LogUtil.ShowInfo(string.Format("请求的资源[{0}]将从内部资源加载", resourceName));
			// 因为创建协程时会确保一个名字只能同时拥有一个协程，所以如果当前已经有一个同名资源在加载中的话，并不会重复加载
			Coroutines.StartMyCoroutine(CoroutineIdentifyString + resourceName, LoadAssetFromStreamingAssets(resourceName, internalAssetFile), RefreshResourceLoadingProgress, resourceName);
		}
	}

	/// <summary>
	/// 从外部文件异步加载资源的携程体
	/// </summary>
	/// <param name="assetName">资源名</param>
	/// <param name="assetExternalFile">外部文件</param>
	/// <returns></returns>
	protected virtual IEnumerator LoadAssetFromExternalFile(string assetName, string assetExternalFile)
	{
		if (LogUtil.ShowDebug != null)
			LogUtil.ShowDebug(string.Format("开始从外部文件加载资源：请求资源[{0}] 外部文件[{1}]", assetName, assetExternalFile));
		WWW www = new WWW(Constants.urlFileHeader + assetExternalFile);
		while (!www.isDone)
			yield return www.progress;
		if (!string.IsNullOrEmpty(www.error))
		{
			if (LogUtil.ShowWarning != null)
				LogUtil.ShowWarning(string.Format("读取外部文件出错[{0}]:\n{1}", Constants.urlFileHeader + assetExternalFile, www.error));
			www.Dispose();
			www = null;
			// 资源加载失败后的处理
			ProcessFailedAsset(assetName);
			yield break;
		}
		AssetBundle assetbundle = www.assetBundle;
		www.Dispose();
		www = null;
		if (assetbundle == null)
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError(string.Format("从外部文件创建assetbundle出错[{0}]", assetExternalFile));
			// 资源加载失败后的处理
			ProcessFailedAsset(assetName);
			yield break;
		}

		object res = null;
		if (AssetType != typeof(AssetBundle))
		{
			AssetPackage package = new AssetPackage();
			package.reletiveAssets = assetbundle.LoadAllAssets();
			if (package.reletiveAssets == null || package.reletiveAssets.Length < 1)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("在外部文件[{0}]中找不到资源", assetExternalFile));
				// 资源加载失败后的处理
				ProcessFailedAsset(assetName);
				yield break;
			}
			if(AssetType != null)
				package.mainAsset = assetbundle.LoadAsset(assetName, AssetType);
			//package.rawAssetbundle = assetbundle;
			yield return 0.99f;
			assetbundle.Unload(false);
			assetbundle = null;
			//if (AssetType != null && package.mainAsset == null)
			//{
			//	Debug.LogError(string.Format("在外部文件[{0}]中找不到指定资源[{1}]", assetExternalFile, assetName));
			//	// 资源加载失败后的处理
			//	ProcessFailedAsset(assetName);
			//	yield break;
			//}
			res = package;
		}
		else
			res = assetbundle;

		yield return Constants.oneFloat;
		if (LogUtil.ShowDebug != null)
			LogUtil.ShowDebug(string.Format("从外部文件加载资源完成：加载资源[{0}]", assetName));
		// 资源加载成功后的处理
		ProcessLoadedAsset(assetName, res);
	}

	/// <summary>
	/// 从Resources异步加载资源的携程体
	/// </summary>
	/// <param name="assetName">资源名</param>
	/// <param name="assetResourcePath">内部资源路径</param>
	/// <returns></returns>
	protected virtual IEnumerator LoadAssetFromResource(string assetName, string assetResourcePath)
	{
		if (LogUtil.ShowDebug != null)
			LogUtil.ShowDebug(string.Format("开始从内部资源加载资源：请求资源[{0}] 内部资源[{1}]", assetName, assetResourcePath));
		float weight1 = 0.5f;
		ResourceRequest request1 = Resources.LoadAsync(assetResourcePath, typeof(TextAsset));
		while (!request1.isDone)
			yield return request1.progress * weight1;
		TextAsset ta = (TextAsset)request1.asset;
		request1 = null;
		if (ta == null)
		{
			if (LogUtil.ShowWarning != null)
				LogUtil.ShowWarning(string.Format("找不到内部资源[{0}]", assetResourcePath));
			// 资源加载失败后的处理
			ProcessFailedAsset(assetName);
			yield break;
		}

		float weight2 = 0.5f;
		AssetBundleCreateRequest request2 = AssetBundle.LoadFromMemoryAsync(ta.bytes);
		while (!request2.isDone)
			yield return weight1 + request2.progress * weight2;
		AssetBundle assetbundle = request2.assetBundle;
		request2 = null;
		Resources.UnloadAsset(ta);
		ta = null;
		if (assetbundle == null)
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError(string.Format("从内部资源创建assetbundle出错[{0}]", assetResourcePath));
			// 资源加载失败后的处理
			ProcessFailedAsset(assetName);
			yield break;
		}

		object res = null;
		if (AssetType != typeof(AssetBundle))
		{
			AssetPackage package = new AssetPackage();
			package.reletiveAssets = assetbundle.LoadAllAssets();
			if (package.reletiveAssets == null || package.reletiveAssets.Length < 1)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("在内部资源[{0}]中找不到资源", assetResourcePath));
				// 资源加载失败后的处理
				ProcessFailedAsset(assetName);
				yield break;
			}
			if(AssetType != null)
				package.mainAsset = assetbundle.LoadAsset(assetName, AssetType);
			//package.rawAssetbundle = assetbundle;
			yield return 0.99f;
			assetbundle.Unload(false);
			assetbundle = null;
			//if (AssetType != null && package.mainAsset == null)
			//{
			//	Debug.LogError(string.Format("在内部资源[{0}]中找不到指定资源[{1}]", assetResourcePath, assetName));
			//	// 资源加载失败后的处理
			//	ProcessFailedAsset(assetName);
			//	yield break;
			//}
			res = package;
		}
		else
			res = assetbundle;

		yield return Constants.oneFloat;
		if(LogUtil.ShowDebug != null)
			LogUtil.ShowDebug(string.Format("从内部资源加载资源完成：加载资源[{0}]", assetName));
		// 资源加载成功后的处理
		ProcessLoadedAsset(assetName, res);
	}

	/// <summary>
	/// 从StreamingAssets异步加载资源的携程体
	/// </summary>
	/// <param name="assetName">资源名</param>
	/// <param name="assetInternalFile">内部文件</param>
	/// <returns></returns>
	protected virtual IEnumerator LoadAssetFromStreamingAssets(string assetName, string assetInternalFile)
	{
		if (LogUtil.ShowDebug != null)
			LogUtil.ShowDebug(string.Format("开始从内部资源加载资源：请求资源[{0}] 内部资源[{1}]", assetName, assetInternalFile));
		string path = assetInternalFile;
		if (!assetInternalFile.Contains("://"))
			path = Constants.urlFileHeader + assetInternalFile;
		WWW www = new WWW(path);
		while (!www.isDone)
			yield return www.progress;
		if (!string.IsNullOrEmpty(www.error))
		{
			if (LogUtil.ShowWarning != null)
				LogUtil.ShowWarning(string.Format("读取内部资源出错[{0}]:\n{1}", Constants.urlFileHeader + assetInternalFile, www.error));
			www.Dispose();
			www = null;
			// 资源加载失败后的处理
			ProcessFailedAsset(assetName);
			yield break;
		}
		AssetBundle assetbundle = www.assetBundle;
		www.Dispose();
		www = null;
		if (assetbundle == null)
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError(string.Format("从内部资源创建assetbundle出错[{0}]", assetInternalFile));
			// 资源加载失败后的处理
			ProcessFailedAsset(assetName);
			yield break;
		}

		object res = null;
		if (AssetType != typeof(AssetBundle))
		{
			AssetPackage package = new AssetPackage();
			package.reletiveAssets = assetbundle.LoadAllAssets();
			if (package.reletiveAssets == null || package.reletiveAssets.Length < 1)
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("在内部资源[{0}]中找不到资源", assetInternalFile));
				// 资源加载失败后的处理
				ProcessFailedAsset(assetName);
				yield break;
			}
#if UNITY_EDITOR
			// 检查所有格式被设置为ARGB32的贴图（移动平台不支持ARGB32格式，必须都转成RGBA32）
			for(int i = 0; i < package.reletiveAssets.Length; ++i)
			{
				UnityObject uo = package.reletiveAssets[i];
				if(uo is Texture2D)
				{
					if (((Texture2D)uo).format == TextureFormat.ARGB32)
						Debug.LogWarning(string.Format("贴图{0}格式是ARGB32（5），{1}", uo, assetName));
				}
			}
#endif
			if (AssetType != null)
				package.mainAsset = assetbundle.LoadAsset(assetName, AssetType);
			//package.rawAssetbundle = assetbundle;
			yield return 0.99f;
			assetbundle.Unload(false);
			assetbundle = null;
			//if (AssetType != null && package.mainAsset == null)
			//{
			//	Debug.LogError(string.Format("在内部资源[{0}]中找不到指定资源[{1}]", assetInternalFile, assetName));
			//	// 资源加载失败后的处理
			//	ProcessFailedAsset(assetName);
			//	yield break;
			//}
			res = package;
		}
		else
			res = assetbundle;

		yield return Constants.oneFloat;
		if (LogUtil.ShowDebug != null)
			LogUtil.ShowDebug(string.Format("从内部资源加载资源完成：加载资源[{0}]", assetName));
		// 资源加载成功后的处理
		ProcessLoadedAsset(assetName, res);
	}

	/// <summary>
	/// 携程运行过程中处理中途抛出的数据，一般都是资源异步加载的百分比进度，将这些数据写入资源对应的资源请求器
	/// </summary>
	/// <param name="progress">加载进度，浮点数</param>
	/// <param name="resourceName">资源名</param>
	void RefreshResourceLoadingProgress(object progress, object resourceName)
	{
		List<AsyncResourceRequester> tmpCacheList = null;
		if (requestCacheTable != null && requestCacheTable.TryGetValue((string)resourceName, out tmpCacheList))
		{
			for (int i = 0; i < tmpCacheList.Count; ++i)
				tmpCacheList[i].progress = (float)progress;
		}
	}

	/// <summary>
	/// 处理异步加载携程执行完毕后加载上来的资源实例，将其放入资源表，为其生成ResourceHolder，并处理为该资源缓存的加载请求
	/// </summary>
	/// <param name="assetName">资源名</param>
	/// <param name="assetLoaded">资源实例</param>
	protected void ProcessLoadedAsset(string assetName, object assetLoaded)
	{
		if (resourceTable == null)
			resourceTable = new Dictionary<string, ResourceHolder>();
		ResourceHolder resourceHolder = new ResourceHolder(assetName, assetLoaded, UnloadResource);
		resourceTable[assetName] = resourceHolder;
		// 处理缓存的资源请求，给资源增加对应的引用计数，并逐次调用资源加载成功回调
		List<AsyncResourceRequester> tmpCacheList = null;
		if (requestCacheTable != null)
			requestCacheTable.TryGetValue(assetName, out tmpCacheList);
		if (tmpCacheList != null)
		{
			IncreaseResourceReferenceCount(assetName, tmpCacheList.Count);
			for (int i = 0; i < tmpCacheList.Count; ++i)
			{
				AsyncResourceRequester requester = tmpCacheList[i];
				requester.progress = 1f;
				requester.done = true;
				requester.resourceHolder = resourceHolder;
				if (requester.resourceHolderGenerateCallback != null)
					requester.resourceHolderGenerateCallback(requester, requester.callbackExtraParam);
			}
			requestCacheTable.Remove(assetName);
		}
		else
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError(string.Format("加载的资源[{0}]找不到请求者，只能释放掉", assetName));
			UnloadResource(resourceHolder);
		}
	}

	/// <summary>
	/// 处理异步加载携程执行中发生错误的情况，要将异步请求器里的fail标志位置为true，并清理请求缓存
	/// </summary>
	/// <param name="assetName">资源名</param>
	protected void ProcessFailedAsset(string assetName)
	{
		List<AsyncResourceRequester> tmpCacheList = null;
		if (requestCacheTable != null)
			requestCacheTable.TryGetValue(assetName, out tmpCacheList);
		if (tmpCacheList != null)
		{
			for (int i = 0; i < tmpCacheList.Count; ++i)
			{
				AsyncResourceRequester requester = tmpCacheList[i];
				requester.progress = 0f;
				requester.fail = true;
			}
			requestCacheTable.Remove(assetName);
		}
	}

	/// <summary>
	/// 增加资源引用计数
	/// </summary>
	/// <param name="resourceName">资源名</param>
	/// <param name="amount">增加的引用计数数值，默认 1</param>
	void IncreaseResourceReferenceCount(string resourceName, int amount = 1)
	{
		int curr = 0;
		if(resourceReferenceTable == null)
			resourceReferenceTable = new Dictionary<object, int>();
		else
			resourceReferenceTable.TryGetValue(resourceName, out curr);
		resourceReferenceTable[resourceName] = curr + amount;
		if (LogUtil.ShowDebug != null)
			LogUtil.ShowDebug(string.Format("资源[{0}]的引用计数为({1})", resourceName, curr + amount));
	}

	/// <summary>
	/// 减少资源引用计数，并返回资源引用计数是否已经降到零了
	/// </summary>
	/// <param name="resourceName">资源名</param>
	/// <param name="amount">减少的引用计数数值，默认 1</param>
	/// <returns>
	/// 降到零 true
	/// 比零大 false
	/// </returns>
	bool DecreaseResourceReferenceCount(string resourceName, int amount = 1)
	{
		int curr = 0;
		if (resourceReferenceTable == null)
			resourceReferenceTable = new Dictionary<object, int>();
		else if (resourceReferenceTable.TryGetValue(resourceName, out curr))
		{
			curr -= amount;
			resourceReferenceTable[resourceName] = curr;
		}
		if(curr <= 0)
		{
			if (curr < 0)
				if (LogUtil.ShowWarning != null)
					LogUtil.ShowWarning(string.Format("资源[{0}]的引用计数降到零以下了({1})，可能有错误发生", resourceName, curr));
			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug(string.Format("资源[{0}]的引用计数为({1})", resourceName, curr));
			return true;
		}
		else
		{
			if (LogUtil.ShowDebug != null)
				LogUtil.ShowDebug(string.Format("资源[{0}]的引用计数为({1})", resourceName, curr));
			return false;
		}

	}
}
