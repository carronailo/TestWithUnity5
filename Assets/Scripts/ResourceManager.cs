using System;
using System.Collections.Generic;
using UnityEngine;

public enum EResourceType
{
	Configure,
	Atlas,
	RawTexture,
	Audio,
	SFX,
	PlayerModel,
	MonsterModel,
	OtherModel,
	Map,
	Scene,
	Shader,
	CharacterMisc,
	CharacterElementDatabase,
	PlayerAnimation,
    Panel,
    PanelItem,
    Font
}

public class ResourceManager
{
	public static ResourceManager Instance
	{
		get
		{
			if (instance == null)
				instance = new ResourceManager();
			return instance;
		}
	}
	private static ResourceManager instance = null;

	private HashSet<string> commonResourceSet = null;

	private Dictionary<EResourceType, IResourceLoader> resourceLoaderTable = null;

	private Dictionary<AsyncResourceRequester, EResourceType> requesterTable = null;

	private List<AsyncResourceRequester> requesterRecordProgressList = null;
	private bool record = false;

	private ResourceManager()
	{
		RegisterResourceLoader(EResourceType.Shader, new ShaderResourceLoader());
		RegisterResourceLoader(EResourceType.Configure, new ConfigureResourceLoader());
		RegisterResourceLoader(EResourceType.Scene, new SceneResourceLoader());
		if (!SystemParameters.disableUIResourceLoader)
		{
			RegisterResourceLoader(EResourceType.Atlas, new AtlasResourceLoader());
			RegisterResourceLoader(EResourceType.RawTexture, new RawTextureResourceLoader());
		}
		if (!SystemParameters.disableAudioResourceLoader)
			RegisterResourceLoader(EResourceType.Audio, new AudioResourceLoader());
		if(!SystemParameters.disableSFXResourceLoader)
			RegisterResourceLoader(EResourceType.SFX, new SFXResourceLoader());
		if (!SystemParameters.disableCharacterResourceLoader)
		{
			RegisterResourceLoader(EResourceType.CharacterElementDatabase, new CharacterElementDatabaseLoader());
			RegisterResourceLoader(EResourceType.PlayerAnimation, new PlayerAnimationResourceLoader());
			RegisterResourceLoader(EResourceType.PlayerModel, new PlayerModelResourceLoader());
			RegisterResourceLoader(EResourceType.CharacterMisc, new CharacterMiscResourceLoader());
			//RegisterResourceLoader(EResourceType.OtherModel, new OtherModelResourceLoader());
		}
		if (!SystemParameters.disableMapResourceLoader)
		{
			RegisterResourceLoader(EResourceType.MonsterModel, new MonsterModelResourceLoader());
			RegisterResourceLoader(EResourceType.Map, new MapResourceLoader());
		}

        if (!SystemParameters.disablePanelResourceLoader)
        {
            RegisterResourceLoader(EResourceType.Panel, new PanelResourceLoader());
        }

        if (!SystemParameters.disablePanelResourceLoader)
        {
            RegisterResourceLoader(EResourceType.PanelItem, new PanelItemResourceLoader());
        }

        if (!SystemParameters.disablePanelResourceLoader)
        {
            RegisterResourceLoader(EResourceType.Font, new FontResourceLoader());
        }
    }

	private void RegisterResourceLoader(EResourceType type, IResourceLoader loader)
	{
		if (resourceLoaderTable == null)
			resourceLoaderTable = new Dictionary<EResourceType, IResourceLoader>();
		if (resourceLoaderTable.ContainsKey(type))
		{
			Debug.LogError(string.Format("重复注册资源加载器[{0}]", type.ToString()));
			return;
		}
		resourceLoaderTable[type] = loader;
	}

	public AsyncResourceRequester RequestResource(EResourceType type, string resourceName, Action<AsyncResourceRequester, object> onResourceLoadedCallback, object callbackExtraParam = null)
	{
		AsyncResourceRequester requester = null;
		if (string.IsNullOrEmpty(resourceName))
			return requester;
		IResourceLoader loader = null;
		if (resourceLoaderTable.TryGetValue(type, out loader))
		{
			requester = loader.LoadResource(resourceName, onResourceLoadedCallback, callbackExtraParam);
			if (requesterTable == null)
				requesterTable = new Dictionary<AsyncResourceRequester, EResourceType>();
			requesterTable[requester] = type;
			if (record)
				requesterRecordProgressList.Add(requester);
		}
		return requester;
	}

	public void ReleaseResource(AsyncResourceRequester resourceRequester)
	{
		if (resourceRequester == null)
			return;

		EResourceType resourceType;
		if (requesterTable != null && requesterTable.TryGetValue(resourceRequester, out resourceType))
		{
			IResourceLoader loader = null;
			if (resourceLoaderTable.TryGetValue(resourceType, out loader))
			{
				loader.UnloadResource(resourceRequester);
				requesterTable.Remove(resourceRequester);
			}
		}
		else
		{
			Debug.LogWarning("无法找到这个请求器的记录 " + resourceRequester.resourceName);
		}
	}

	public void BeginRecordProgress()
	{
		if (record)
			return;
		record = true;
		if (requesterRecordProgressList == null)
			requesterRecordProgressList = new List<AsyncResourceRequester>();
		else if (requesterRecordProgressList.Count > 0)
			requesterRecordProgressList.Clear();
	}

	public void EndRecordProgress()
	{
		record = false;
		if (requesterRecordProgressList != null)
			requesterRecordProgressList.Clear();
	}

	// 返回百分数（整数0~100）
	public int CalcRecordedProgress()
	{
		if (record)
		{
			int progressTotal = 0;
			if (requesterRecordProgressList.Count == 0)
				return 100;
			for (int i = 0; i < requesterRecordProgressList.Count; ++i)
				progressTotal += (int)(((requesterRecordProgressList[i].fail || requesterRecordProgressList[i].released) ? Constants.oneFloat : requesterRecordProgressList[i].progress) * 100);
			return progressTotal / requesterRecordProgressList.Count;
		}
		return 0;
	}

	public void AddCommonResource(string resourceName)
	{
		if (commonResourceSet == null)
			commonResourceSet = new HashSet<string>();
		commonResourceSet.Add(resourceName);
	}

	public bool IsCommonResource(string resourceName)
	{
		if (commonResourceSet == null)
			return false;
		return commonResourceSet.Contains(resourceName);
	}
}
