using UnityEngine;
using UnityEditor;
using System;

public class CreatewScriptObjectAssetWindow : EditorToolWindowBase
{
	[MenuItem("My Tools/Create 'ScriptObject' asset", priority = 1000)]
	static void CreateWindow()
	{
		//if(EditorUtility.DisplayDialog("My Tools", "About to open 'Create ScriptObject Asset' window.", "OK", "Cancel"))
		//{
		//	InitWindow();
		//}
		InitWindow();
	}

	private string scriptObjectClassName = string.Empty;
	private string lastSavePath = string.Empty;

	protected override void Init()
	{
		base.Init();
		titleContent = new GUIContent("Create Asset");
		lastSavePath = Application.dataPath;
	}

	private void OnGUI()
	{
		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("ScriptObject类名：", GUILayout.MaxWidth(110f));
		scriptObjectClassName = EditorGUILayout.TextField(scriptObjectClassName, GUILayout.MaxWidth(200f));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical(GUILayout.MaxWidth(50f));
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("保存位置", GUILayout.MaxWidth(50f));
		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();
		if(GUILayout.Button("浏览...", GUILayout.Width(45f), GUILayout.Height(25f)))
		{
			string path = EditorUtility.SaveFilePanelInProject("保存Asset文件", scriptObjectClassName, "asset", "显示一条消息？", lastSavePath);
			if (!string.IsNullOrEmpty(path))
				lastSavePath = path;
		}
		EditorGUILayout.BeginVertical();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField(lastSavePath.Equals(Application.dataPath) ? string.Empty : lastSavePath);
		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		if (GUILayout.Button("创建Asset", GUILayout.MinWidth(100f), GUILayout.MaxWidth(200f),  GUILayout.MinHeight(100f), GUILayout.MaxHeight(50f)))
		{
			if(string.IsNullOrEmpty(scriptObjectClassName))
			{
				EditorUtility.DisplayDialog("忘记填ScriptObject类名了？", "需要填写一个项目中存在的ScriptObject类的名称。", "返回");
			}
			else if(lastSavePath.Equals(Application.dataPath))
			{
				if(EditorUtility.DisplayDialog("确定要存放在项目根目录里？", "当前的保存路径是项目根目录，建议创建一个专门用来存储Asset文件的资源目录。", "确定保存", "返回"))
				{
					CreateAsset(scriptObjectClassName, lastSavePath);
				}
			}
			else
			{
				CreateAsset(scriptObjectClassName, lastSavePath);
			}
		}
	}

	void CreateAsset(string scriptObjectClassName, string savePath)
	{
		Type soClassType = null;
		string errorMessage = string.Empty;
		try
		{
			soClassType = Type.GetType(scriptObjectClassName, true);
		}
		catch(Exception ex)
		{
			errorMessage = ex.Message;
		}
		if(soClassType == null && !string.IsNullOrEmpty(errorMessage))
		{
			EditorUtility.DisplayDialog("找不到类型" + scriptObjectClassName, "如果类型存在于命名空间中，那么填写类名时需要填写完整的命名空间。\n" + errorMessage, "返回");
			return;
		}
		else if(soClassType == null)
		{
			EditorUtility.DisplayDialog("找不到类型" + scriptObjectClassName, "如果类型存在于命名空间中，那么填写类名时需要填写完整的命名空间。", "返回");
			return;
		}
		else
		{
			ScriptableObject so = CreateInstance(soClassType);
			AssetDatabase.CreateAsset(so, lastSavePath);
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			EditorUtility.DisplayDialog(string.Format("成功为类型{0}创建Asset", scriptObjectClassName), "保存路径：" + lastSavePath, "返回");
		}
	}
}