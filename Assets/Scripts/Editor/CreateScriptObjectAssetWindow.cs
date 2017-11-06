using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Tag = EditorLogTag;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CreatewScriptObjectAssetWindow : EditorToolWindowBase
{
	[MenuItem("My Tools/Create 'ScriptObject' asset", priority = 1000)]
	static void CreateWindow()
	{
		InitWindow(typeof(CreatewScriptObjectAssetWindow));
	}

	private string scriptObjectClassName = string.Empty;
	private string lastSavePath = string.Empty;
	private string[] scriptObjectClassCandidates = null;
	private string[] scriptObjectClassSearchResult = null;
	private string searchKeywords = string.Empty;
	private Regex searchPattern = null;

	private GUIStyle boxStyle = null;
	private GUIStyle searchFieldStyle = null;
	private GUIStyle searchFieldCancelButtonStyle = null;
	private Vector2 scriptObjectClassCandidatesScrollPosition = Vector2.zero;

	protected override void Init()
	{
		base.Init();
		titleContent = new GUIContent("Create Asset");
		lastSavePath = Application.dataPath;
		boxStyle = "CN Box";
		searchFieldStyle = "SearchTextField";
		searchFieldCancelButtonStyle = "SearchCancelButton";
	}

	private void OnGUI()
	{
		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("ScriptObject类名：", GUILayout.MaxWidth(110f));
		scriptObjectClassName = EditorGUILayout.TextField(scriptObjectClassName, GUILayout.MaxWidth(250f));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical(GUILayout.MaxWidth(50f));
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("保存位置：", GUILayout.MaxWidth(55f));
		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();
		if (GUILayout.Button("浏览...", GUILayout.Width(45f), GUILayout.Height(25f)))
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

		if (GUILayout.Button("创建Asset", GUILayout.MinWidth(100f), GUILayout.MaxWidth(200f), GUILayout.MinHeight(100f), GUILayout.MaxHeight(50f)))
		{
			if (string.IsNullOrEmpty(scriptObjectClassName))
			{
				EditorUtility.DisplayDialog("忘记填ScriptObject类名了？", "需要填写一个项目中存在的ScriptObject类的名称。", "返回");
			}
			else if (lastSavePath.Equals(Application.dataPath))
			{
				if (EditorUtility.DisplayDialog("确定要存放在项目根目录里？", "当前的保存路径是项目根目录，建议创建一个专门用来存储Asset文件的资源目录。", "确定保存", "返回"))
				{
					CreateAsset(scriptObjectClassName, lastSavePath);
				}
			}
			else
			{
				CreateAsset(scriptObjectClassName, lastSavePath);
			}
		}

		EditorGUILayout.Space();

		GUILayout.Box("", GUILayout.Height(2f), GUILayout.Width(position.width - 5f));

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("扫描所有ScriotObject类名", GUILayout.Width(200f), GUILayout.Height(50f)))
		{
			scriptObjectClassCandidates = ScanAllScriptObjectClasses();
			if (searchPattern != null)
				scriptObjectClassSearchResult = FilterBySearchPattern(scriptObjectClassCandidates, searchPattern);
			else
				scriptObjectClassSearchResult = scriptObjectClassCandidates;
		}
		EditorGUILayout.BeginVertical();
		GUILayout.Space(20f);
		EditorGUILayout.BeginHorizontal();
		EditorGUI.BeginChangeCheck();
		searchKeywords = EditorGUILayout.TextField(searchKeywords, searchFieldStyle, GUILayout.MaxWidth(250f));
		if (EditorGUI.EndChangeCheck())
		{
			try
			{
				searchPattern = string.IsNullOrEmpty(searchKeywords) ? null : new Regex(searchKeywords.Trim(), RegexOptions.Compiled);
				if(searchPattern != null)
					scriptObjectClassSearchResult = FilterBySearchPattern(scriptObjectClassCandidates, searchPattern);
			}
			catch { }
		}
		if (GUILayout.Button(string.Empty, searchFieldCancelButtonStyle))
		{
			// 清理搜索结果
			searchKeywords = string.Empty;
			searchPattern = null;
			scriptObjectClassSearchResult = scriptObjectClassCandidates;
			GUIUtility.hotControl = 0;
			GUIUtility.keyboardControl = 0;
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		scriptObjectClassCandidatesScrollPosition = EditorGUILayout.BeginScrollView(scriptObjectClassCandidatesScrollPosition);
		if (scriptObjectClassSearchResult != null && scriptObjectClassSearchResult.Length > 0)
		{
			foreach(string scriptObjectClass in scriptObjectClassSearchResult)
			{
				EditorGUILayout.SelectableLabel(scriptObjectClass, GUILayout.Height(17f));
			}
		}
		EditorGUILayout.EndScrollView();
	}

	void CreateAsset(string scriptObjectClassName, string savePath)
	{
		Type soClassType = null;
		string errorMessage = string.Empty;
		try
		{
			soClassType = Type.GetType(scriptObjectClassName, true);
		}
		catch (Exception ex)
		{
			errorMessage = ex.Message;
		}
		if (soClassType == null && !string.IsNullOrEmpty(errorMessage))
		{
			EditorUtility.DisplayDialog("找不到类型" + scriptObjectClassName, "如果类型存在于命名空间中，那么填写类名时需要填写完整的命名空间。\n" + errorMessage, "返回");
			return;
		}
		else if (soClassType == null)
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

	string[] ScanAllScriptObjectClasses()
	{
		List<string> res = new List<string>();
		string[] types = null;
		Type scriptObjectType = typeof(ScriptableObject);
		types = FindClassBySuperClass("Library/ScriptAssemblies/Assembly-CSharp.dll", scriptObjectType);
		res.AddRange(types);
		types = FindClassBySuperClass("Library/ScriptAssemblies/Assembly-CSharp-Editor.dll", scriptObjectType);
		res.AddRange(types);
		types = FindClassBySuperClass("Library/ScriptAssemblies/Assembly-CSharp-Editor-firstpass.dll", scriptObjectType);
		res.AddRange(types);
		types = FindClassBySuperClass("Library/ScriptAssemblies/Assembly-CSharp-firstpass.dll", scriptObjectType);
		res.AddRange(types);
		types = FindClassBySuperClass("Library/ScriptAssemblies/Assembly-UnityScript-firstpass.dll", scriptObjectType);
		res.AddRange(types);
		res.Sort();
		return res.ToArray();
	}

	string[] FindClassBySuperClass(string assemblyPath, Type superType)
	{
		List<string> res = new List<string>();
		Assembly assembly = Assembly.LoadFile(assemblyPath);
		Type[] allTypes = assembly.GetTypes();
		foreach (Type type in allTypes)
		{
			if(type.IsSubclassOf(superType))
			{
				res.Add(type.ToString());
			}
		}
		return res.ToArray();
	}

	string[] FilterBySearchPattern(string[] candidates, Regex pattern)
	{
		List<string> res = new List<string>();
		foreach(string s in candidates)
		{
			if (pattern.IsMatch(s))
				res.Add(s);
		}
		return res.ToArray();
	}
}