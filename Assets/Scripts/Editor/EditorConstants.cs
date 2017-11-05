using System;
using UnityEngine;
using UnityEditor;

public static class EditorConstants
{
	public static readonly string PROJECT_ROOT_PATH = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') + 1);	//项目根目录，即Assets目录的上级目录，路径字符串以"/"结尾
	public static readonly string ASSETBUNDLE_RELATIVE_PATH = "Assets/../Assetbundles/";    // Assetbundles目录的相对路径，相对于Assets目录，路径字符串以"/"结尾
	public static readonly string ASSETBUNDLE_ABSOLUTE_PATH = PROJECT_ROOT_PATH + "Assetbundles/";  // Assetbundles目录的绝对路径，路径字符串以"/"结尾

}

public static class EditorLogTag
{
	public const string Builder = "Builder";
}
