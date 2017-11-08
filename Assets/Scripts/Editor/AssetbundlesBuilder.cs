using UnityEngine;
using UnityEditor;
using System.Text;
using Tag = EditorLogTag;

public class AssetbundlesBuilder
{
	static AssetbundlesBuilder()
	{
		EditorApplication.delayCall += () =>
		{
			Menu.SetChecked("My Tools/Auto Build On Name Changing", EditorPrefs.GetBool("AutoBuildOnAssetBundleNameChanged"));
		};
	}

	[MenuItem("My Tools/Build All Assetbundles (Default)", priority = 1)]
	public static void BuildAllAssetbundles()
	{
		AssetBundleManifest manifest = null;
		string outputPath = EditorConstants.ASSETBUNDLE_RELATIVE_PATH;
#if UNITY_STANDALONE_WIN
#if USE_64BIT
		outputPath += BuildTarget.StandaloneWindows64 + "/";
		// 确保路径存在
		IOUtil.MakeSurePath(outputPath);
		manifest = BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
#else
		outputPath += BuildTarget.StandaloneWindows + "/";
		// 确保路径存在
		IOUtil.MakeSurePath(outputPath);
		manifest = BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
#endif
#elif UNITY_ANDROID
		outputPath += BuildTarget.Android + "/";
		// 确保路径存在
		IOUtil.MakeSurePath(outputPath);
		manifest = BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.Android);
#elif UNITY_IOS
		outputPath += BuildTarget.iPhone + "/";
		// 确保路径存在
		IOUtil.MakeSurePath(outputPath);
		manifest = BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.iPhone);
#endif
		string[] assetbundles = manifest.GetAllAssetBundles();
		StringBuilder sb = new StringBuilder();
		foreach (string assetbundle in assetbundles)
		{
			sb.AppendLine(manifest.GetAssetBundleHash(assetbundle).ToString());
		}
		LogConsole.Log(Tag.Builder, sb.ToString());
		uint crc;
		BuildPipeline.GetCRCForAssetBundle(outputPath + "joystick", out crc);
		LogConsole.Log(Tag.Builder, crc);
	}

	[MenuItem("My Tools/Get All Assetbundles Names", priority = 2)]
	public static void GetAllAssetbundlesNames()
	{
		string[] abNames = AssetDatabase.GetAllAssetBundleNames();
		StringBuilder sb = new StringBuilder();
		foreach(string abName in abNames)
			sb.AppendLine(abName);
		LogConsole.Log(Tag.Builder, sb.ToString());
		UnityEditorInternal.InternalEditorUtility.OpenEditorConsole();
	}

	[MenuItem("My Tools/Build All Assetbundles (Custom)", priority = 3)]
	public static void BuildAllAssetbundlesCustom()
	{
		// 确保路径存在
		IOUtil.MakeSurePath(EditorConstants.ASSETBUNDLE_ABSOLUTE_PATH);
		AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
		string[] joystickAssets = new string[2];
		joystickAssets[0] = "Assets/UITexture/Joystick/摇杆_底.png";
		joystickAssets[1] = "Assets/UITexture/Joystick/摇杆_钮.png";
		buildMap[0].assetNames = joystickAssets;
		buildMap[0].assetBundleName = "joystick";
#if UNITY_STANDALONE_WIN
		AssetBundleManifest manifest = null;
#if USE_64BIT
		manifest = BuildPipeline.BuildAssetBundles(EditorConstants.ASSETBUNDLE_RELATIVE_PATH, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
#else
		manifest = BuildPipeline.BuildAssetBundles(EditorConstants.ASSETBUNDLE_RELATIVE_PATH, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
#endif
		string[] assetbundles = manifest.GetAllAssetBundles();
		StringBuilder sb = new StringBuilder();
		foreach (string assetbundle in assetbundles)
		{
			sb.AppendLine(manifest.GetAssetBundleHash(assetbundle).ToString());
		}
		LogConsole.Log(Tag.Builder, sb.ToString());
		uint crc;
		BuildPipeline.GetCRCForAssetBundle(EditorConstants.ASSETBUNDLE_RELATIVE_PATH + "joystick", out crc);
		LogConsole.Log(Tag.Builder, crc);
#elif UNITY_ANDROID

#elif UNITY_IOS

#endif
	}

	[MenuItem("My Tools/(toggles)", priority = 10000)]
	public static void TogglesStartFromHere()
	{
	}

	[MenuItem("My Tools/(toggles)", validate = true, priority = 10000)]
	public static bool TogglesStartFromHereValidate()
	{
		return false;
	}

	[MenuItem("My Tools/Auto Build On Name Changing", priority = 10001)]
	public static void ToggleAutoBuildOnAssetBundleNameChanged()
	{
		bool currentValue = EditorPrefs.GetBool("AutoBuildOnAssetBundleNameChanged");
		currentValue = !currentValue;
		EditorPrefs.SetBool("AutoBuildOnAssetBundleNameChanged", currentValue);
		Menu.SetChecked("My Tools/Auto Build On Name Changing", currentValue);
	}
}
