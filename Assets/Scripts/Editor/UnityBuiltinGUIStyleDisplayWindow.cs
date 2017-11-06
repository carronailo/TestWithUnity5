using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UnityBuiltinGUIStyleDisplayWindow : EditorToolWindowBase
{
	[MenuItem("Window/Unity Internal GUIStyles")]
	static void CreateWindow()
	{
		defaultWindowSize = new Vector2(300f, 800f);
		InitWindow(typeof(UnityBuiltinGUIStyleDisplayWindow));
	}

	private bool init = false;
	private string[] allStyleNames = null;
	private Vector2 scrollPosition = Vector2.zero;

	protected override void Init()
	{
		base.Init();
		titleContent = new GUIContent("Builtin Style");
	}

	string[] GetAllBuiltinGUIStyleNames()
	{
		List<string> res = new List<string>();
		GUIStyle[] allStyles = GUI.skin.customStyles;
		foreach(GUIStyle style in allStyles)
		{
			res.Add(style.name);
		}
		return res.ToArray();
	}

	void OnGUI()
	{
		if (!init)
		{
			allStyleNames = GetAllBuiltinGUIStyleNames();
			init = true;
		}

		EditorGUILayout.Space();

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		if (allStyleNames != null && allStyleNames.Length > 0)
		{
			foreach (string name in allStyleNames)
			{
				EditorGUILayout.SelectableLabel(name, GUILayout.Height(17f));
			}
		}
		EditorGUILayout.EndScrollView();
	}
}