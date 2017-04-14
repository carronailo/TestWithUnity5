using UnityEngine;

public static class GameObject_CloneComponent_Extension
{
	public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
	{
		if (toAdd == null)
			return null;
		return go.AddComponent<T>().GetCopyOf(toAdd) as T;
	}

	public static T ReplaceComponent<T>(this GameObject go, T toReplace) where T : Component
	{
		if (toReplace == null)
			return null;
		T compToBeReplaced = go.GetComponent<T>();
		if (compToBeReplaced == null)
			return null;
		return compToBeReplaced.GetCopyOf(toReplace) as T;
	}

	public static string GetPathFromRoot(this GameObject go, GameObject root)
	{
		Transform myTransform = go.transform;
		Transform rootTrans = myTransform.root;
		if (root != null)
			rootTrans = root.transform;
		string path = "";
		while (myTransform != rootTrans)
		{
			path = "/" + myTransform.name + path;
			myTransform = myTransform.parent;
		}
		path = rootTrans.name + path;
		return path;
	}
}

public static class GameObject_Is_Extension
{
	private const string terrainTag = "Terrain";
	private const string playerTag = "Player";

	public static bool IsMainPlayer(this GameObject go)
	{
		return go.CompareTag(playerTag);
	}

	public static bool IsTerrain(this GameObject go)
	{
		return go.CompareTag(terrainTag);
	}
}
