using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class LoadAssetbundle : MonoBehaviour
{
	[DllImport("user32.dll", EntryPoint = "keybd_event")]
	public static extern void keybd_event(
			byte bVk,    //虚拟键值 对应按键的ascll码十进制值
			byte bScan,// 0
			int dwFlags,  //0 为按下，1按住，2为释放
			int dwExtraInfo  // 0
		);

	public bool useEditorOnly = false;
	public bool instantiateObject = false;
	public string[] assetbundlePath;

	// Use this for initialization
	IEnumerator Start()
	{
		string path = Application.dataPath + "/../AssetBundles/";
		if(useEditorOnly)
			path = Application.dataPath + "/../AssetBundles_Editor/";
		foreach (string abPath in assetbundlePath)
		{
			WWW www = new WWW(path + abPath);
			while (!www.isDone)
				yield return null;
			AssetBundle assetbundle = www.assetBundle;
			Object[] objs = assetbundle.LoadAllAssets();
			foreach (Object obj in objs)
			{
				Debug.Log(obj);
				if(instantiateObject)
					Process(obj);
			}
			assetbundle.Unload(false);
			assetbundle = null;
			www.Dispose();
			www = null;
			Resources.UnloadUnusedAssets();
			Debug.Log("--------------------------------------------------");
		}

		//InvokeRepeating("OutputKey", 0f, 1f);
	}

	//void OutputKey()
	//{
	//	keybd_event(121, 0, 0, 0);
	//	keybd_event(121, 0, 1, 0);
	//	keybd_event(121, 0, 2, 0);
	//}

	// Update is called once per frame
	void Update()
	{
	}

	Material tempMat = null;
	void Process(Object obj)
	{
		if (obj is Material)
			tempMat = obj as Material;
		if (obj is GameObject)
		{
			GameObject temp = Instantiate(obj) as GameObject;
			Renderer rdr = temp.GetComponent<Renderer>();
			if(rdr != null)
				rdr.material = tempMat;
		}
			
	}
}
