using System;
using UnityEngine;

public class CharacterElementDatabaseLoader : ResourceLoaderBase
{
	protected override string IdentifyString { get { return "Character/"; } }
	protected override string CoroutineIdentifyString { get { return "CharacterElementDatabaseLoader."; } }
	protected override Type AssetType { get { return typeof(CharacterElementDatabase); } }

	public override AsyncResourceRequester LoadResource(string resourceName, Action<AsyncResourceRequester, object> onResourceLoadedCallback, object callbackExtraParam)
	{
		Debug.Log(string.Format("请求加载角色部件数据库资源[{0}]", resourceName));
		return base.LoadResource(resourceName, onResourceLoadedCallback, callbackExtraParam);
	}

	public override void UnloadResource(AsyncResourceRequester requester)
	{
		Debug.Log(string.Format("请求释放角色部件数据库资源[{0}]", requester.resourceName));
		base.UnloadResource(requester);
	}
}
