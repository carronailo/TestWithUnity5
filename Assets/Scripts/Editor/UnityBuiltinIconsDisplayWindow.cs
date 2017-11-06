using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UnityBuiltinIconsDisplayWindow : EditorToolWindowBase
{
	struct BuiltinIcon : System.IEquatable<BuiltinIcon>, System.IComparable<BuiltinIcon>
	{
		public GUIContent icon;
		public GUIContent name;

		public override bool Equals(object o)
		{
			return o is BuiltinIcon && this.Equals((BuiltinIcon)o);
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}

		public bool Equals(BuiltinIcon o)
		{
			return this.name.text == o.name.text;
		}

		public int CompareTo(BuiltinIcon o)
		{
			return this.name.text.CompareTo(o.name.text);
		}
	}

	[MenuItem("Window/Unity Internal Icons")]
	static void CreateWindow()
	{
		defaultWindowSize = new Vector2(400f, 800f);
		InitWindow(typeof(UnityBuiltinIconsDisplayWindow));
	}

	List<BuiltinIcon> _icons = new List<BuiltinIcon>();
	Vector2 _scroll_pos;
	GUIContent _refresh_button;

	protected override void Init()
	{
		base.Init();
		titleContent = new GUIContent("Builtin Icons");
	}

	void OnEnable()
	{
		_refresh_button = new GUIContent(EditorGUIUtility.IconContent("d_preAudioLoopOff").image,
			"Refresh : Icons are only loaded in memory when the appropriate window is opened.");

		FindIcons();
	}

	/* Find all textures and filter them to narrow the search. */
	void FindIcons()
	{
		_icons.Clear();

		Texture2D[] t = Resources.FindObjectsOfTypeAll<Texture2D>();
		foreach (Texture2D x in t)
		{
			if (x.name.Length == 0)
				continue;

			if (x.hideFlags != HideFlags.HideAndDontSave && x.hideFlags != (HideFlags.HideInInspector | HideFlags.HideAndDontSave))
				continue;

			if (!EditorUtility.IsPersistent(x))
				continue;

			/* This is the *only* way I have found to confirm the icons are indeed unity builtin. Unfortunately
			 * it uses LogError instead of LogWarning or throwing an Exception I can catch. So make it shut up. */
			DisableLogging();
			GUIContent gc = EditorGUIUtility.IconContent(x.name);
			EnableLogging();

			if (gc == null)
				continue;
			if (gc.image == null)
				continue;

			_icons.Add(new BuiltinIcon()
			{
				icon = gc,
				name = new GUIContent(x.name)
			});
		}

		_icons.Sort();
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
		Repaint();
	}

	void OnGUI()
	{
		_scroll_pos = EditorGUILayout.BeginScrollView(_scroll_pos);
		EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
		if (GUILayout.Button(_refresh_button, EditorStyles.toolbarButton))
		{
			FindIcons();
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField("Found " + _icons.Count + " icons");
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField("Double-click name to copy", GetMiniGreyLabelStyle());

		EditorGUILayout.Space();

		EditorGUIUtility.labelWidth = 100;
		for (int i = 0; i < _icons.Count; ++i)
		{
			EditorGUILayout.LabelField(_icons[i].icon, _icons[i].name);

			if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.clickCount > 1)
			{
				EditorGUIUtility.systemCopyBuffer = _icons[i].name.text;
				Debug.Log(_icons[i].name.text + " copied to clipboard.");
			}
		}

		EditorGUILayout.EndScrollView();
	}

	static void DisableLogging()
	{
#if UNITY_5_3_OR_NEWER
		Debug.logger.logEnabled = false;
#endif
	}

	static void EnableLogging()
	{
#if UNITY_5_3_OR_NEWER
		Debug.logger.logEnabled = true;
#endif
	}

	static GUIStyle GetMiniGreyLabelStyle()
	{
#if UNITY_5_3_OR_NEWER
		return EditorStyles.centeredGreyMiniLabel;
#else
		return EditorStyles.miniLabel;
#endif
	}

	static Texture2D GetAssociatedAlphaSplitTexture(Sprite s)
	{
#if UNITY_5_3_OR_NEWER
		return s.associatedAlphaSplitTexture;
#else
		return null;
#endif
	}

}