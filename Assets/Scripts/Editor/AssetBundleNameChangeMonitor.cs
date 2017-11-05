using UnityEngine;
using UnityEditor;
using Tag = EditorLogTag;

public class AssetBundleNameChangeMonitor : AssetPostprocessor
{
	void OnPostprocessAssetbundleNameChanged(string path, string previous, string next)
	{
		LogConsole.Log(Tag.Builder, string.Format("AB name change [{0}] ({1})>>({2})", path, previous, next));
		bool autoBuild = EditorPrefs.GetBool("AutoBuildOnAssetBundleNameChanged");
		if(autoBuild)
		{
			AssetbundlesBuilder.BuildAllAssetbundles();
		}
	}
}