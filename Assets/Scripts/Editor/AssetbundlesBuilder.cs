using UnityEngine;
using UnityEditor;
using System.Text;
using Tag = EditorLogTag;

public class AssetbundlesBuilder : AssetPostprocessor
{
	[MenuItem("My Tools/Build all assetbundles (default)", priority = 1)]
	public static void BuildAllAssetbundles()
	{
		// 确保路径存在
		IOUtil.MakeSurePath(EditorConstants.ASSETBUNDLE_ABSOLUTE_PATH);
#if UNITY_STANDALONE_WIN
#if USE_64BIT
		BuildPipeline.BuildAssetBundles(EditorConstants.ASSETBUNDLE_RELATIVE_PATH, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
#else
		AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(EditorConstants.ASSETBUNDLE_RELATIVE_PATH, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
		string[] assetbundles = manifest.GetAllAssetBundles();
		StringBuilder sb = new StringBuilder();
		foreach(string assetbundle in assetbundles)
		{
			sb.AppendLine(manifest.GetAssetBundleHash(assetbundle).ToString());
		}
		LogConsole.Log(Tag.Builder, sb.ToString());
		uint crc;
		BuildPipeline.GetCRCForAssetBundle(EditorConstants.ASSETBUNDLE_RELATIVE_PATH + "joystick", out crc);
		LogConsole.Log(Tag.Builder, crc);
#endif
#elif UNITY_ANDROID

#elif UNITY_IOS

#endif
	}

	[MenuItem("My Tools/Get all assetbundles names", priority = 2)]
	public static void GetAllAssetbundlesNames()
	{
		string[] abNames = AssetDatabase.GetAllAssetBundleNames();
		StringBuilder sb = new StringBuilder();
		foreach(string abName in abNames)
			sb.AppendLine(abName);
		LogConsole.Log(Tag.Builder, sb.ToString());
	}

	void OnPostprocessAssetbundleNameChanged(string path, string previous, string next)
	{
		LogConsole.Log(Tag.Builder, string.Format("AB name change [{0}] ({1})>>({2})", path, previous, next));
	}
}
