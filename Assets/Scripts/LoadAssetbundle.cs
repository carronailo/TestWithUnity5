using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.Networking;

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

	public AssetBundle ab = null;

	// Use this for initialization
	IEnumerator Start()
	{
		string path = Application.dataPath + "/../AssetBundles/";
		if(useEditorOnly)
			path = Application.dataPath + "/../AssetBundles_Editor/";
		if(path.Contains("://"))
		{
			// if path is a URL, use WWW to load it
			foreach (string abPath in assetbundlePath)
			{
				using (UnityWebRequest request = UnityWebRequest.GetAssetBundle(path + abPath))
				{
					yield return request.Send();
					AssetBundle assetbundle = DownloadHandlerAssetBundle.GetContent(request);
					if (assetbundle == null)
						LogConsole.Log("TestAssetBundle", "Fail to load " + path + abPath);
					Object[] objs = assetbundle.LoadAllAssets();
					foreach (Object obj in objs)
					{
						Debug.Log(obj);
						if (instantiateObject)
							Process(obj);
					}
					//assetbundle.Unload(false);
					//assetbundle = null;
					if (ab != null)
						ab.Unload(false);
					ab = assetbundle;
				}
				Resources.UnloadUnusedAssets();
				Debug.Log("--------------------------------------------------");
			}
		}
		else
		{
			foreach (string abPath in assetbundlePath)
			{
				AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path + abPath);
				yield return request;
				AssetBundle assetbundle = request.assetBundle;
				if (assetbundle == null)
					LogConsole.Log("TestAssetBundle", "Fail to load " + path + abPath);
				Object[] objs = assetbundle.LoadAllAssets();
				foreach (Object obj in objs)
				{
					Debug.Log(obj);
					if (instantiateObject)
						Process(obj);
				}
				//assetbundle.Unload(false);
				//assetbundle = null;
				if (ab != null)
					ab.Unload(false);
				ab = assetbundle;
				Resources.UnloadUnusedAssets();
				Debug.Log("--------------------------------------------------");
			}
		}

		//InvokeRepeating("OutputKey", 0f, 1f);
	}

	private void OnDestroy()
	{
		if (ab != null)
			ab.Unload(true);
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
