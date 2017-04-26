using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ToDoListWindow : EditorWindow
{
	[Serializable]
	private class TODOEntry
	{
		public string text;
		public string whole;
		public string file;
		public int line;

		public override string ToString()
		{
			return string.Format("{0}-{1}-{2}-{3}", file, line, text, whole);
		}
	}

	private class StyleConstants
	{
		public static bool ms_Loaded;

		public static GUIStyle Box;
		public static GUIStyle Button;
		public static GUIStyle MiniButton;
		public static GUIStyle MiniButtonLeft;
		public static GUIStyle MiniButtonMiddle;
		public static GUIStyle MiniButtonRight;
		public static GUIStyle EntryStyle;
		public static GUIStyle EntryIconStyle;
		public static GUIStyle EvenBackground;
		public static GUIStyle OddBackground;
		public static GUIStyle MessageStyle;
		public static GUIStyle MessageButtonStyle;
		public static GUIStyle MessageButtonBoldStyle;
		public static GUIStyle Toolbar;
		public static GUIStyle SearchField;
		public static GUIStyle SearchFieldCancelButton;

		public static Texture iconTag;

		public static Texture2D splitterTex;

		public static void Init()
		{
			if (!ms_Loaded)
			{
				ms_Loaded = true;

				Box = "CN Box";
				Button = "Button";
				MiniButton = "ToolbarButton";
				MiniButtonLeft = "ToolbarButton";
				MiniButtonMiddle = "ToolbarButton";
				MiniButtonRight = "ToolbarButton";
				Toolbar = "Toolbar";
				EntryStyle = "CN EntryInfo";
				EntryIconStyle = "AssetLabel Icon";
				EvenBackground = "CN EntryBackEven";
				OddBackground = "CN EntryBackodd";
				MessageStyle = "CN Message";
				MessageButtonStyle = "ControlLabel";
				MessageButtonBoldStyle = "ControlLabel";
				SearchField = "ToolbarSeachTextField";
				SearchFieldCancelButton = "ToolbarSeachCancelButton";

				iconTag = EditorGUIUtility.IconContent("AssetLabelIcon").image;

				EntryStyle = new GUIStyle(EntryStyle);
				EntryStyle.normal.background = null;
				EntryStyle.onNormal.background = null;
				EntryStyle.padding = new RectOffset(32, 0, 8, 0);
				//EntryStyle.normal.background = (Texture2D)iconTag;
				//EntryStyle.onNormal.background = (Texture2D)iconTag;
				//EntryStyle.border = new RectOffset(32, 2, 0, 0);
				//EntryStyle.overflow = new RectOffset(-2, 0, -9, -8);

				EvenBackground = new GUIStyle(EvenBackground);
				EvenBackground.margin = new RectOffset(0, 0, 0, 0);
				EvenBackground.padding = new RectOffset(0, 0, 0, 0);
				OddBackground = new GUIStyle(OddBackground);
				OddBackground.margin = new RectOffset(0, 0, 0, 0);
				OddBackground.padding = new RectOffset(0, 0, 0, 0);

				MessageStyle = new GUIStyle(MessageStyle);
				MessageStyle.richText = true;
				MessageButtonStyle = new GUIStyle(MessageButtonStyle);
				MessageButtonStyle.normal.textColor = Color.white;
				MessageButtonBoldStyle = new GUIStyle(MessageButtonBoldStyle);
				MessageButtonBoldStyle.fontStyle = FontStyle.Bold;
				MessageButtonBoldStyle.normal.textColor = Color.white;

				splitterTex = new Texture2D(2, 2);
				splitterTex.hideFlags = HideFlags.DontSaveInEditor;
				Color[] colorArray = new Color[4];
				for (int i = 0; i < colorArray.Length; ++i)
					colorArray[i] = new Color(0.15f, 0.15f, 0.15f);
				splitterTex.SetPixels(colorArray);
				splitterTex.Apply();
			}
		}
	}

	private class ContentConstants
	{
		public static bool ms_Loaded;

		public static GUIContent scanButtonContent = null;

		public static GUIContent detailGUIContent = null;

		public static void Init()
		{
			if (!ms_Loaded)
			{
				ms_Loaded = true;

				scanButtonContent = new GUIContent("Scan TODO List", "扫描所有代码中标识为\"@TODO\"的注释文字");

				detailGUIContent = new GUIContent("");
			}
		}
	}

	private static readonly Vector2 windowSize = new Vector2(600, 300);
	private static readonly int defaultDisplayEntryCount = 16;
	private static readonly float defaultDisplayEntryHeight = 32f;

	[MenuItem("Window/TODO List %#t")]
	public static void CreateWindow()
	{
		ToDoListWindow window = GetWindow<ToDoListWindow>("TODO", true);
		window.minSize = windowSize;
	}

	private int matchCount = 0;

	private float entryAreaDefaultHeightRatio = 0.6f;
	private float entryAreaMinHeightRatio = 0.1f;
	private float entryAreaMaxHeightRatio = 0.8f;
	private float entryAreaHeightRatio = 0f;
	private Rect areaRect = Rect.zero;
	private Vector2 entriesScrollPosition = Vector2.zero;
	private Vector2 detailScrollPosition = Vector2.zero;
	private bool resizingEntryArea = false;

	private List<TODOEntry> currentEntries = null;
	private List<TODOEntry> currentDisplayEntries = null;
	private int currentSelectedEntryIndex = -1;
	private List<GUIContent> currentDisplayEntryGUIContentPool = null;
	private bool selectChanged = false;

	private Regex todoMatchPattern = null;
	private Regex searchPattern = null;
	private string searchKeywords;

	private bool needRepaint = false;

	private void OnEnable()
	{
		Init();
		EditorApplication.update += this.EditorUpdate;
	}

	private void OnDisable()
	{
		EditorApplication.update -= this.EditorUpdate;
	}

	private void OnGUI()
	{
		StyleConstants.Init();
		ContentConstants.Init();

		DrawToolbar();
		DrawTODOList();
		DrawResizeArea();
		DrawDetail();

		if (selectChanged)
		{
			selectChanged = false;
			needRepaint = true;
		}
	}

	private void EditorUpdate()
	{
		if (needRepaint)
		{
			needRepaint = false;
			Repaint();
		}
	}

	private void OnDestroy()
	{
	}

	private void Init()
	{
		if (currentEntries == null)
			currentEntries = new List<TODOEntry>(1024);
		if (currentDisplayEntries == null)
			currentDisplayEntries = new List<TODOEntry>(1024);
		if (currentDisplayEntryGUIContentPool == null)
		{
			currentDisplayEntryGUIContentPool = new List<GUIContent>();
			for (int i = 0; i < defaultDisplayEntryCount; ++i)
				currentDisplayEntryGUIContentPool.Add(new GUIContent(string.Empty, StyleConstants.iconTag));
		}

		entryAreaHeightRatio = entryAreaDefaultHeightRatio;

		if (todoMatchPattern == null)
			todoMatchPattern = new Regex(@"^\s*(//+.*|/\*+.*|\*+.*|\s*)@?TODO[:|：|\s]+\s*(.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	}

	private void DrawToolbar()
	{
		GUILayout.BeginHorizontal(StyleConstants.Toolbar);

		EditorGUI.BeginChangeCheck();
		searchKeywords = EditorGUILayout.TextField(searchKeywords, StyleConstants.SearchField, GUILayout.Width(150f));
		if (EditorGUI.EndChangeCheck())
		{
			try
			{
				searchPattern = string.IsNullOrEmpty(searchKeywords) ? null : new Regex(searchKeywords.Trim(), RegexOptions.Compiled);
			}
			catch { }
			ReFetchEntryDisplayList();
		}
		if (GUILayout.Button(string.Empty, StyleConstants.SearchFieldCancelButton))
		{
			// 清理搜索结果
			searchKeywords = string.Empty;
			searchPattern = null;
			ReFetchEntryDisplayList();
			needRepaint = true;
			GUIUtility.hotControl = 0;
			GUIUtility.keyboardControl = 0;
		}
		GUILayout.Label("Found:", StyleConstants.MessageStyle, GUILayout.Width(45f));
		GUILayout.Label(StyleConstants.iconTag, StyleConstants.MessageStyle);
		GUILayout.Label((matchCount > 999) ? "999+" : matchCount.ToString(), StyleConstants.MessageStyle);
		GUILayout.FlexibleSpace();
		if (GUILayout.Button(ContentConstants.scanButtonContent, StyleConstants.MiniButtonRight))
		{
			ClearAll();
			ScanTODOList();
		}

		GUILayout.EndHorizontal();
	}

	private void DrawTODOList()
	{
		Event current = Event.current;

		areaRect = EditorGUILayout.BeginVertical();
		float displayAreaHeight = areaRect.height;
		entriesScrollPosition = EditorGUILayout.BeginScrollView(entriesScrollPosition, StyleConstants.Box, GUILayout.Height(position.height * entryAreaHeightRatio));

		float currentDisplayStartY = entriesScrollPosition.y;
		float currentDisplayEndY = currentDisplayStartY + displayAreaHeight;
		// 为了显示的需要，下面显示日志条目的地方没有使用Layout，为了确保ScrollView能够正常工作，这里先将显示所有日志条目所需的空间预留出来
		GUILayoutUtility.GetRect(1f, currentDisplayEntries.Count * defaultDisplayEntryHeight);
		int currentDisplayCount = 0;
		Rect currentDisplayRect = new Rect(0f, 0f, areaRect.width, defaultDisplayEntryHeight);
		Rect currentDisplayIconRect = new Rect(6f, 8f, areaRect.width, defaultDisplayEntryHeight);
		for (int i = 0; i < currentDisplayEntries.Count; ++i)
		{
			float y = i * defaultDisplayEntryHeight;
			if (y > currentDisplayEndY || y + defaultDisplayEntryHeight < currentDisplayStartY)
				continue;
			currentDisplayRect.y = y;
			currentDisplayIconRect.y = y + 8f;
			if (current.type == EventType.MouseDown && current.button == 0 && currentDisplayRect.Contains(current.mousePosition))
			{
				// 检测到鼠标点击当前条目
				if (current.clickCount == 2)
				{
					// 是双击
					OpenEditorToEntry(currentDisplayEntries[i], -1);
				}
				if (currentSelectedEntryIndex != i)
				{
					currentSelectedEntryIndex = i;
					selectChanged = true;
					// 当选中一个条目时，要确保条目被完全显示在滚动区域中
					if (y < currentDisplayStartY)
						entriesScrollPosition -= new Vector2(0f, currentDisplayStartY - y);
					else if (y + defaultDisplayEntryHeight > currentDisplayEndY)
						entriesScrollPosition += new Vector2(0f, y + defaultDisplayEntryHeight - currentDisplayEndY);
				}
				// 当前点击事件不能再继续向后传递，这很重要！
				current.Use();
			}
			else if (current.type == EventType.Repaint)
			{
				// 绘制背景（奇偶数的日志背景颜色不同）
				GUIStyle backgroundStyle = (i % 2 != 0) ? StyleConstants.EvenBackground : StyleConstants.OddBackground;
				backgroundStyle.Draw(currentDisplayRect, false, false, currentSelectedEntryIndex == i, false);
				// 绘制图标
				StyleConstants.EntryIconStyle.Draw(currentDisplayIconRect, string.Empty, false, false, currentSelectedEntryIndex == i, false);
				// 绘制日志内容
				GUIContent content = GetGUIContentFormEntryDisplayPool(currentDisplayCount);
				content.text = currentDisplayEntries[i].text;
				StyleConstants.EntryStyle.Draw(currentDisplayRect, content, false, false, currentSelectedEntryIndex == i, false);
			}
			++currentDisplayCount;
		}
		if (current.type == EventType.KeyDown)
		{
			if (current.keyCode == KeyCode.UpArrow)
			{
				currentSelectedEntryIndex = Mathf.Max(0, currentSelectedEntryIndex - 1);
				selectChanged = true;
				current.Use();
			}
			else if (current.keyCode == KeyCode.DownArrow)
			{
				currentSelectedEntryIndex = Mathf.Min(currentDisplayEntries.Count - 1, currentSelectedEntryIndex + 1);
				selectChanged = true;
				current.Use();
			}
			else if(current.keyCode == KeyCode.Return)
			{
				if(currentSelectedEntryIndex >= 0 && currentDisplayEntries.Count > currentSelectedEntryIndex)
					OpenEditorToEntry(currentDisplayEntries[currentSelectedEntryIndex], -1);
				current.Use();
			}
			if (selectChanged)
			{
				float y = currentSelectedEntryIndex * defaultDisplayEntryHeight;
				if (y < currentDisplayStartY)
					entriesScrollPosition -= new Vector2(0f, currentDisplayStartY - y);
				else if (y + defaultDisplayEntryHeight > currentDisplayEndY)
					entriesScrollPosition += new Vector2(0f, y + defaultDisplayEntryHeight - currentDisplayEndY);
			}
		}

		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}

	private void DrawResizeArea()
	{
		Event current = Event.current;

		Rect resizeRect = new Rect(0f, areaRect.y + areaRect.height - 2.5f, position.width, 5f);
		EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeVertical);
		if (current.type != EventType.Repaint)
		{
			if (current.type == EventType.MouseDown && !resizingEntryArea)
			{
				if (resizeRect.Contains(current.mousePosition))
					resizingEntryArea = true;
			}
			else if (current.type == EventType.MouseDrag && resizingEntryArea)
			{
				entryAreaHeightRatio += Event.current.delta.y / position.height;
				entryAreaHeightRatio = Mathf.Min(entryAreaMaxHeightRatio, Mathf.Max(entryAreaMinHeightRatio, entryAreaHeightRatio));
				needRepaint = true;
			}
			else if (current.type == EventType.MouseUp && resizingEntryArea)
			{
				resizingEntryArea = false;
			}
		}
	}

	private void DrawDetail()
	{
		Event current = Event.current;

		GUILayout.Space(1f);
		detailScrollPosition = EditorGUILayout.BeginScrollView(detailScrollPosition);
		TODOEntry entry = (currentDisplayEntries == null || currentSelectedEntryIndex < 0 || currentDisplayEntries.Count <= currentSelectedEntryIndex) 
			? null : currentDisplayEntries[currentSelectedEntryIndex];
		ContentConstants.detailGUIContent.text = entry != null ? entry.whole : string.Empty;
		float minHeight = StyleConstants.MessageStyle.CalcHeight(ContentConstants.detailGUIContent, position.width);
		if (selectChanged)
		{
			// 当选中的条目发生变化的时候，要将当前的选择状态取消掉
			GUIUtility.hotControl = 0;
			GUIUtility.keyboardControl = 0;
		}
		EditorGUILayout.SelectableLabel(ContentConstants.detailGUIContent.text, StyleConstants.MessageStyle, GUILayout.ExpandWidth(true), GUILayout.Height(minHeight));

		EditorGUILayout.EndScrollView();

	}

	private void ScanTODOList()
	{
		try
		{
			DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "/");
			FileInfo[] fileInfos = dirInfo.GetFiles("*.cs", SearchOption.AllDirectories);
			if(fileInfos != null)
			{
				for(int i = 0; i < fileInfos.Length; ++i)
				{
					FileInfo fi = fileInfos[i];
					using (StreamReader reader = fi.OpenText())
					{
						int lineNum = 0;
						string line = null;
						while((line = reader.ReadLine()) != null)
						{
							++lineNum;
							Match result = todoMatchPattern.Match(line);
							if(result.Success)
							{
								string text = result.Groups[2].Value;
								string file = fi.FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets");
								TODOEntry entry = new TODOEntry {
									text = text,
									file = file,
									line = lineNum,
									whole = string.Format("{0}\n\n(at {1}:{2})", line.Trim(), file, lineNum),
								};
								currentEntries.Add(entry);
								// @TODO: 高亮被命中的字符串
								if (searchPattern == null)
									currentDisplayEntries.Add(entry);
								else if (IsTODOEntryMatch(searchPattern, line))
								{
									currentDisplayEntries.Add(entry);
									++matchCount;
								}
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	private bool IsTODOEntryMatch(Regex searchPattern, string todoEntry)
	{
		return searchPattern.IsMatch(todoEntry);
	}

	private void ClearAll()
	{
		currentEntries.Clear();
		currentDisplayEntries.Clear();
		currentSelectedEntryIndex = -1;
		selectChanged = true;
		matchCount = 0;
	}

	private GUIContent GetGUIContentFormEntryDisplayPool(int index)
	{
		if (index < currentDisplayEntryGUIContentPool.Count)
			return currentDisplayEntryGUIContentPool[index];
		else
		{
			while (index >= currentDisplayEntryGUIContentPool.Count)
				currentDisplayEntryGUIContentPool.Add(new GUIContent(string.Empty/*, StyleConstants.iconTag*/));
			return currentDisplayEntryGUIContentPool[index];
		}
	}

	private void OpenEditorToEntry(TODOEntry entry, int lineNum = -1)
	{
		UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(entry.file, typeof(TextAsset));
		if (obj != null)
			AssetDatabase.OpenAsset(obj, (lineNum != -1) ? lineNum : entry.line);
	}

	private void ReFetchEntryDisplayList()
	{
		currentDisplayEntries.Clear();
		matchCount = 0;
		for (int i = 0; i < currentEntries.Count; ++i)
		{
			TODOEntry entry = currentEntries[i];
			// @TODO: 高亮被命中的字符串
			if (searchPattern == null)
				currentDisplayEntries.Add(entry);
			else if (IsTODOEntryMatch(searchPattern, entry.whole))
			{
				currentDisplayEntries.Add(entry);
				++matchCount;
			}
		}
	}


}
