using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using FlyingWorm;

public class LogConsoleWindow : EditorWindow
{
	// Unity Internal LogEntry Mode
	[Flags]
	private enum EMode
	{
		Error = 1,                                          // bit 1
		Assert = 2,                                         // bit 2
		Log = 4,                                            // bit 3
															// ?? = 8
		Fatal = 16,                                         // bit 5
		DontPreprocessCondition = 32,                       // bit 6
		AssetImportError = 64,                              // bit 7
		AssetImportWarning = 128,                           // bit 8
		ScriptingError = 256,                               // bit 9
		ScriptingWarning = 512,                             // bit 10
		ScriptingLog = 1024,                                // bit 11
		ScriptCompileError = 2048,                          // bit 12
		ScriptCompileWarning = 4096,                        // bit 13
		StickyError = 8192,                                 // bit 14
		MayIgnoreLineNumber = 16384,                        // bit 15
		ReportBug = 32768,                                  // bit 16
		DisplayPreviousErrorInStatusBar = 65536,            // bit 17
		ScriptingException = 131072,                        // bit 18
		DontExtractStacktrace = 262144,                     // bit 19
		ShouldClearOnPlay = 524288,                         // bit 20
		GraphCompileError = 1048576,                        // bit 21
		ScriptingAssertion = 2097152                        // bit 22
	}

	// Unity Internal Console Flags
	[Flags]
	private enum EConsoleFlags
	{
		Collapse = 1,
		ClearOnPlay,
		ErrorPause = 4,
		Verbose = 8,
		StopForAssert = 16,
		StopForError = 32,
		Autoscroll = 64,
		LogLevelLog = 128,
		LogLevelWarning = 256,
		LogLevelError = 512
	}

	private class LogConsoleEntry
	{
		public LogType type;
		public string fistTwoLines;
		public string whole;
		public string file;
		public int line;
		public int mode;
		public int instanceID;

		public string text;
		public List<StackEntry> stackEntries;

		public override string ToString()
		{
			return string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", type, mode, file, line, instanceID, fistTwoLines, whole);
		}
	}

	public class StackEntry
	{
		public string fileName;
		public string className;
		public string methodName;
		public string namespaceName;
		public int lineNumber;
		public int charNumber;
		public string stackLabel;
		public string stackLabel2;
		public List<string> sourceCode;
	}

	private class UnityInternal
	{
		public static bool ms_Loaded;

		public static Type logEntriesType;        // UnityEditorInternal.LogEntries
		public static object logEntries;
		public static MethodInfo getEntryMethod;              // (static)UnityEditorInternal.LogEntries.GetEntryInternal
		public static MethodInfo startGettingEntriesMethod;   // (static)UnityEditorInternal.LogEntries.StartGettingEntries
		public static MethodInfo endGettingEntriesMethod;     // (static)UnityEditorInternal.LogEntries.EndGettingEntries
		public static MethodInfo clearEntriesMethod;          // (static)UnityEditorInternal.LogEntries.Clear
		public static MethodInfo setConsoleFlagMethod;        // (static)UnityEditorInternal.LogEntries.SetConsoleFlag
		public static MethodInfo getCountMethod;              // (static)UnityEditorInternal.LogEntries.GetCount
		public static MethodInfo getCountsByTypeMethod;       // (static)UnityEditorInternal.LogEntries.GetCountsByType
		public static MethodInfo getFirstTwoLinesMethod;      // (static)UnityEditorInternal.LogEntries.GetFirstTwoLinesEntryTextAndModeInternal
		public static MethodInfo rowGotDoubleClickedMethod;   // (static)UnityEditorInternal.LogEntries.RowGotDoubleClicked
		public static PropertyInfo logEntriesFlagField;       // (static)UnityEditorInternal.LogEntries.consoleFlags

		public static Type logEntryType;      // UnityEditorInternal.LogEntry
		public static object logEntry;
		public static FieldInfo logEntryConditionField;           // (instance)UnityEditorInternal.LogEntry.condition
		public static FieldInfo logEntryFileField;            // (instance)UnityEditorInternal.LogEntry.file
		public static FieldInfo logEntryLineField;            // (instance)UnityEditorInternal.LogEntry.line
		public static FieldInfo logEntryModeField;            // (instance)UnityEditorInternal.LogEntry.mode
		public static FieldInfo logEntryInstanceIDField;      // (instance)UnityEditorInternal.LogEntry.instanceID
		public static FieldInfo logEntryIdentifierField;      // (instance)UnityEditorInternal.LogEntry.identifier
		public static FieldInfo logEntryErrorNumField;        //  (instance)UnityEditorInternal.LogEntry.errorNum
		public static FieldInfo logEntryIsWorldPlayingField;  //  (instance)UnityEditorInternal.LogEntry.isWorldPlaying

		public static void Init()
		{
			if (!ms_Loaded)
			{
				ms_Loaded = true;

				logEntriesType = typeof(EditorWindow).Assembly.GetType("UnityEditorInternal.LogEntries");
				logEntries = Activator.CreateInstance(logEntriesType);
				getEntryMethod = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				startGettingEntriesMethod = logEntriesType.GetMethod("StartGettingEntries", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				endGettingEntriesMethod = logEntriesType.GetMethod("EndGettingEntries", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				clearEntriesMethod = logEntriesType.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				setConsoleFlagMethod = logEntriesType.GetMethod("SetConsoleFlag", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				getCountMethod = logEntriesType.GetMethod("GetCount", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				getCountsByTypeMethod = logEntriesType.GetMethod("GetCountsByType", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				getFirstTwoLinesMethod = logEntriesType.GetMethod("GetFirstTwoLinesEntryTextAndModeInternal", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				rowGotDoubleClickedMethod = logEntriesType.GetMethod("RowGotDoubleClicked", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				logEntriesFlagField = logEntriesType.GetProperty("consoleFlags", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

				logEntryType = typeof(EditorWindow).Assembly.GetType("UnityEditorInternal.LogEntry");
				logEntry = Activator.CreateInstance(logEntryType);
				logEntryConditionField = logEntryType.GetField("condition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				logEntryFileField = logEntryType.GetField("file", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				logEntryLineField = logEntryType.GetField("line", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				logEntryModeField = logEntryType.GetField("mode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				logEntryInstanceIDField = logEntryType.GetField("instanceID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				logEntryIdentifierField = logEntryType.GetField("identifier", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				logEntryErrorNumField = logEntryType.GetField("errorNum", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				logEntryIsWorldPlayingField = logEntryType.GetField("isWorldPlaying", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
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
		public static GUIStyle LogStyle;
		public static GUIStyle WarningStyle;
		public static GUIStyle ErrorStyle;
		public static GUIStyle AssertionStyle;
		public static GUIStyle ExceptionStyle;
		public static GUIStyle EvenBackground;
		public static GUIStyle OddBackground;
		public static GUIStyle MessageStyle;
		public static GUIStyle MessageButtonStyle;
		public static GUIStyle MessageButtonBoldStyle;
		public static GUIStyle StatusError;
		public static GUIStyle StatusWarn;
		public static GUIStyle StatusLog;
		public static GUIStyle Toolbar;
		public static GUIStyle CountBadge;

		public static Texture iconInfo;
		public static Texture iconWarn;
		public static Texture iconError;
		public static Texture iconAssertion;
		public static Texture iconException;
		public static Texture iconInfoSmall;
		public static Texture iconWarnSmall;
		public static Texture iconErrorSmall;
		public static Texture iconAssertionSmall;
		public static Texture iconExceptionSmall;
		public static Texture iconInfoMono;
		public static Texture iconWarnMono;
		public static Texture iconErrorMono;
		public static Texture iconAssertionMono;
		public static Texture iconExceptionMono;

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
				LogStyle = "CN EntryInfo";
				WarningStyle = "CN EntryWarn";
				ErrorStyle = "CN EntryError";
				AssertionStyle = "CN EntryError";
				ExceptionStyle = "CN EntryError";
				ErrorStyle = "CN EntryError";
				EvenBackground = "CN EntryBackEven";
				OddBackground = "CN EntryBackodd";
				MessageStyle = "CN Message";
				MessageButtonStyle = "ControlLabel";
				MessageButtonBoldStyle = "ControlLabel";
				StatusError = "CN StatusError";
				StatusWarn = "CN StatusWarn";
				StatusLog = "CN StatusInfo";
				CountBadge = "CN CountBadge";

				iconInfo = EditorGUIUtility.IconContent("console.infoicon").image;
				iconWarn = Resources.Load<Texture>("iconpng_huang");
				iconError = Resources.Load<Texture>("iconpng_hong");
				iconAssertion = Resources.Load<Texture>("iconpng_cheng");
				iconException = Resources.Load<Texture>("iconpng_zi");
				iconInfoSmall = EditorGUIUtility.IconContent("console.infoicon.sml").image;
				iconWarnSmall = Resources.Load<Texture>("iconpng_huang_sml");
				iconErrorSmall = Resources.Load<Texture>("iconpng_hong_sml");
				iconAssertionSmall = Resources.Load<Texture>("iconpng_cheng_sml");
				iconExceptionSmall = Resources.Load<Texture>("iconpng_zi_sml");
				iconInfoMono = EditorGUIUtility.IconContent("console.infoicon.sml").image;
				iconWarnMono = Resources.Load<Texture>("iconpng_hui_sml");
				iconErrorMono = iconWarnMono;
				iconAssertionMono = iconWarnMono;
				iconExceptionMono = iconWarnMono;

				WarningStyle = new GUIStyle(WarningStyle);
				WarningStyle.normal.background = (Texture2D)iconWarn;
				WarningStyle.onNormal.background = (Texture2D)iconWarn;
				WarningStyle.border = new RectOffset(32, 0, 0, 0);
				WarningStyle.overflow = new RectOffset(-4, -4, -4, -4);
				ErrorStyle = new GUIStyle(ErrorStyle);
				ErrorStyle.normal.background = (Texture2D)iconError;
				ErrorStyle.onNormal.background = (Texture2D)iconError;
				ErrorStyle.border = new RectOffset(32, 0, 0, 0);
				ErrorStyle.overflow = new RectOffset(-4, -4, -4, -4);
				AssertionStyle = new GUIStyle(AssertionStyle);
				AssertionStyle.normal.background = (Texture2D)iconAssertion;
				AssertionStyle.onNormal.background = (Texture2D)iconAssertion;
				AssertionStyle.border = new RectOffset(32, 0, 0, 0);
				AssertionStyle.overflow = new RectOffset(-4, -4, -4, -4);
				ExceptionStyle = new GUIStyle(ExceptionStyle);
				ExceptionStyle.normal.background = (Texture2D)iconException;
				ExceptionStyle.onNormal.background = (Texture2D)iconException;
				ExceptionStyle.border = new RectOffset(32, 0, 0, 0);
				ExceptionStyle.overflow = new RectOffset(-4, -4, -4, -4);

				MessageStyle = new GUIStyle(MessageStyle);
				MessageStyle.richText = true;
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

		public static GUIContent clearGUIContent = null;
		public static GUIContent collapseEntriesGUIContent = null;
		public static GUIContent clearOnPlayGUIContent = null;
		public static GUIContent errorPauseGUIContent = null;
		public static GUIContent stopForAssertGUIContent = null;
		public static GUIContent stopForErrorGUIContent = null;

		public static GUIContent displayInfoGUIContent = null;
		public static GUIContent displayWarningGUIContent = null;
		public static GUIContent displayAssertionGUIContent = null;
		public static GUIContent displayErrorGUIContent = null;
		public static GUIContent displayExceptionGUIContent = null;

		public static GUIContent stackTraceGUIContent = null;

		public static void Init()
		{
			if (!ms_Loaded)
			{
				ms_Loaded = true;

				clearGUIContent = new GUIContent("Clear", "清理所有日志");
				collapseEntriesGUIContent = new GUIContent("Collapse", "合并相同日志");
				clearOnPlayGUIContent = new GUIContent("Clear on Play", "运行游戏前清理日志");
				errorPauseGUIContent = new GUIContent("Error Pause", "接收到错误时暂停游戏");
				stopForAssertGUIContent = new GUIContent("Stop for Assert");
				stopForErrorGUIContent = new GUIContent("Stop for Error");

				displayInfoGUIContent = new GUIContent("0", "是否显示普通日志（白色）的开关");
				displayWarningGUIContent = new GUIContent("0", "是否显示警告日志（黄色）的开关");
				displayAssertionGUIContent = new GUIContent("0", "是否显示断言日志（橙色）的开关");
				displayErrorGUIContent = new GUIContent("0", "是否显示错误日志（红色）的开关");
				displayExceptionGUIContent = new GUIContent("0", "是否显示异常日志（紫色）的开关");

				stackTraceGUIContent = new GUIContent("");
			}
		}
	}

	private static readonly Vector2 windowSize = new Vector2(600, 300);
	private static readonly int defaultDisplayLogCount = 10;
	private static readonly float defaultDisplayLogHeight = 32f;

	[MenuItem("Window/My Console %#x")]
	public static void CreateWindow()
	{
		LogConsoleWindow window = GetWindow<LogConsoleWindow>("MyConsole", true);
		window.minSize = windowSize;
	}

	private static bool HasMode(int mode, EMode modeToCheck)
	{
		return (mode & (int)modeToCheck) != 0;
	}

	private static bool HasFlag(int consoleFlags, EConsoleFlags flagsToCheck)
	{
		return (consoleFlags & (int)flagsToCheck) != 0;
	}

	private static GUIStyle GetStyleForErrorMode(int mode)
	{
		GUIStyle result;
		if (HasMode(mode, (EMode)131072))       // ScriptingException
			result = StyleConstants.ExceptionStyle;
		else if (HasMode(mode, (EMode)2097154))     // Assert | ScriptingAssertion
			result = StyleConstants.AssertionStyle;
		else if (HasMode(mode, (EMode)1050961))     // Error | Fatal | AssetImportError | ScriptingError | ScriptCompileError | GraphCompileError
			result = StyleConstants.ErrorStyle;
		else if (HasMode(mode, (EMode)4736))        // AssetImportWarning | ScriptingWarning | ScriptCompileWarning
			result = StyleConstants.WarningStyle;
		else        // Log | ScriptingLog
			result = StyleConstants.LogStyle;
		return result;
	}

	private static Texture GetIconForErrorMode(int mode, bool large)
	{
		Texture result;
		if (HasMode(mode, (EMode)131072))       // ScriptingException
			result = ((!large) ? StyleConstants.iconExceptionSmall : StyleConstants.iconException);
		else if (HasMode(mode, (EMode)2097154))     // Assert | ScriptingAssertion
			result = ((!large) ? StyleConstants.iconAssertionSmall : StyleConstants.iconAssertion);
		else if (HasMode(mode, (EMode)1050961))     // Error | Fatal | AssetImportError | ScriptingError | ScriptCompileError | GraphCompileError
			result = ((!large) ? StyleConstants.iconErrorSmall : StyleConstants.iconError);
		else if (HasMode(mode, (EMode)4736))        // AssetImportWarning | ScriptingWarning | ScriptCompileWarning
			result = ((!large) ? StyleConstants.iconWarnSmall : StyleConstants.iconWarn);
		else if (HasMode(mode, (EMode)1028))        // Log | ScriptingLog
			result = ((!large) ? StyleConstants.iconInfoSmall : StyleConstants.iconInfo);
		else
			result = null;
		return result;
	}

	private static LogType GetLogTypeForErrorMode(int mode)
	{
		LogType result = LogType.Log;
		if (HasMode(mode, (EMode)131072))       // ScriptingException
			result = LogType.Exception;
		else if (HasMode(mode, (EMode)2097154))     // Assert | ScriptingAssertion
			result = LogType.Assert;
		else if (HasMode(mode, (EMode)1050961))     // Error | Fatal | AssetImportError | ScriptingError | ScriptCompileError | GraphCompileError
			result = LogType.Error;
		else if (HasMode(mode, (EMode)4736))        // AssetImportWarning | ScriptingWarning | ScriptCompileWarning
			result = LogType.Warning;
		else        // Log | ScriptingLog
			result = LogType.Log;
		return result;
	}

	private bool isDevelopmentBuild = false;

	private bool collapseEntries = false;
	private bool clearOnPlay = false;
	private bool errorPause = false;
	private bool stopForAssert = false;
	private bool stopForError = false;

	private bool displayInfo = true;
	private bool displayWarning = true;
	private bool displayAssertion = true;
	private bool displayError = true;
	private bool displayException = true;

	private int logCount = 0;
	private int warningCount = 0;
	private int errorCount = 0;
	private int assertionCount = 0;
	private int exceptionCount = 0;

	private float logAreaDefaultHeightRatio = 0.6f;
	private float logAreaMinHeightRatio = 0.1f;
	private float logAreaMaxHeightRatio = 0.8f;
	private float logAreaHeightRatio = 0f;
	private Rect logAreaRect = Rect.zero;
	private Vector2 logEntriesScrollPosition = Vector2.zero;
	private Vector2 stackTraceScrollPosition = Vector2.zero;
	private bool resizingLogArea = false;

	private List<LogConsoleEntry> currentEntries = null;
	private List<LogConsoleEntry> currentDisplayEntries = null;
	private LogConsoleEntry currentSelectedEntry = null;
	private bool selectChanged = false;

	private List<GUIContent> currentDisplayLogGUIContentPool = null;

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
		// 为确保在切换Edit模式和Play模式的时候初始化数据不丢失，每一次OnGUI都尝试调用初始化，至于是否真的执行初始化流程则由初始化标志位决定
		UnityInternal.Init();
		StyleConstants.Init();
		ContentConstants.Init();

		//if (Event.current.type != EventType.Repaint)
		//{
		//	FillLogEntries();
		//}
		DrawToolbar1();     // 提供原生Console窗口相关功能按钮的工具栏
		DrawToolbar2();     // 提供扩展功能按钮的工具栏
		DrawLogList();      // 日志内容显示区
		DrawResizeArea();
		//DrawSplitter();     // 日志内容显示区和堆栈显示区的分割线
		DrawStackTrace();   // 堆栈显示区

		if (selectChanged)
		{
			selectChanged = false;
			needRepaint = true;
		}
	}

	private void EditorUpdate()
	{
		//if(!EditorApplication.isCompiling)
		{
			if (CheckFillLogEntries())
				needRepaint = true;

			int unityConsoleFlags = (int)UnityInternal.logEntriesFlagField.GetValue(null, null);
			if (CheckUnityConsoleToggleStatesChanged(unityConsoleFlags))
				needRepaint = true;
			if (CheckUnityConsoleDisplayToggleStateChanged(unityConsoleFlags))
			{
				ReFetchLogEntryDisplayList();
				needRepaint = true;
			}
		}
		if (needRepaint)
		{
			needRepaint = false;
			Repaint();
		}
	}

	private void OnDestroy()
	{
	}

	#region 初始化函数

	private void Init()
	{
		UnityInternal.Init();

		isDevelopmentBuild = Unsupported.IsDeveloperBuild();
		if (currentEntries == null)
			currentEntries = new List<LogConsoleEntry>(1024);
		if (currentDisplayEntries == null)
			currentDisplayEntries = new List<LogConsoleEntry>(1024);
		if (currentDisplayLogGUIContentPool == null)
		{
			currentDisplayLogGUIContentPool = new List<GUIContent>();
			for (int i = 0; i < defaultDisplayLogCount; ++i)
				currentDisplayLogGUIContentPool.Add(new GUIContent(string.Empty, StyleConstants.iconInfo));
		}

		// 初始化时将窗口上相关复选框的状态设置为与Unity编辑器原生Console窗口相同
		int unityConsoleFlags = (int)UnityInternal.logEntriesFlagField.GetValue(null, null);
		collapseEntries = HasFlag(unityConsoleFlags, EConsoleFlags.Collapse);
		clearOnPlay = HasFlag(unityConsoleFlags, EConsoleFlags.ClearOnPlay);
		errorPause = HasFlag(unityConsoleFlags, EConsoleFlags.ErrorPause);
		stopForAssert = HasFlag(unityConsoleFlags, EConsoleFlags.StopForAssert);
		stopForError = HasFlag(unityConsoleFlags, EConsoleFlags.StopForError);
		displayInfo = HasFlag(unityConsoleFlags, EConsoleFlags.LogLevelLog);
		displayWarning = HasFlag(unityConsoleFlags, EConsoleFlags.LogLevelWarning);
		displayAssertion = HasFlag(unityConsoleFlags, EConsoleFlags.LogLevelError);
		displayError = displayAssertion;
		displayException = displayAssertion;

		logAreaHeightRatio = logAreaDefaultHeightRatio;
		// @TODO: 其他扩展功能的相关参数，从EditorPrefs里读取
	}

	#endregion

	#region GUI绘制函数

	private float DrawToolbar1()
	{
		GUILayout.BeginHorizontal(StyleConstants.Toolbar);

		if (GUILayout.Button(ContentConstants.clearGUIContent, StyleConstants.MiniButton, GUILayout.Width(40f)))
		{
			UnityInternal.clearEntriesMethod.Invoke(null, null);
			ClearAllLog();
		}

		EditorGUILayout.Space();

		collapseEntries = GUILayout.Toggle(collapseEntries, ContentConstants.collapseEntriesGUIContent, StyleConstants.MiniButtonLeft);
		SetUnityConsoleFlag(EConsoleFlags.Collapse, collapseEntries);
		clearOnPlay = GUILayout.Toggle(clearOnPlay, ContentConstants.clearOnPlayGUIContent, StyleConstants.MiniButtonMiddle);
		SetUnityConsoleFlag(EConsoleFlags.ClearOnPlay, clearOnPlay);
		errorPause = GUILayout.Toggle(errorPause, ContentConstants.errorPauseGUIContent, StyleConstants.MiniButtonRight);
		SetUnityConsoleFlag(EConsoleFlags.ErrorPause, errorPause);
		if (isDevelopmentBuild)
		{
			GUILayout.FlexibleSpace();
			stopForAssert = GUILayout.Toggle(stopForAssert, ContentConstants.stopForAssertGUIContent, StyleConstants.MiniButtonLeft);
			SetUnityConsoleFlag(EConsoleFlags.StopForAssert, stopForAssert);
			stopForError = GUILayout.Toggle(stopForError, ContentConstants.stopForErrorGUIContent, StyleConstants.MiniButtonRight);
			SetUnityConsoleFlag(EConsoleFlags.StopForError, stopForError);
		}
		GUILayout.FlexibleSpace();

		ContentConstants.displayInfoGUIContent.text = (logCount > 9999) ? "9999+" : logCount.ToString();
		ContentConstants.displayInfoGUIContent.image = (logCount <= 0) ? StyleConstants.iconInfoMono : StyleConstants.iconInfoSmall;
		EditorGUI.BeginChangeCheck();
		displayInfo = GUILayout.Toggle(displayInfo, ContentConstants.displayInfoGUIContent, StyleConstants.MiniButtonRight);
		if (EditorGUI.EndChangeCheck())
		{
			ReFetchLogEntryDisplayList();
			SetUnityConsoleFlag(EConsoleFlags.LogLevelLog, displayInfo);
		}

		ContentConstants.displayWarningGUIContent.text = (warningCount > 9999) ? "9999+" : warningCount.ToString();
		ContentConstants.displayWarningGUIContent.image = (warningCount <= 0) ? StyleConstants.iconWarnMono : StyleConstants.iconWarnSmall;
		EditorGUI.BeginChangeCheck();
		displayWarning = GUILayout.Toggle(displayWarning, ContentConstants.displayWarningGUIContent, StyleConstants.MiniButtonRight);
		if (EditorGUI.EndChangeCheck())
		{
			ReFetchLogEntryDisplayList();
			SetUnityConsoleFlag(EConsoleFlags.LogLevelWarning, displayWarning);
		}

		ContentConstants.displayAssertionGUIContent.text = (assertionCount > 9999) ? "9999+" : assertionCount.ToString();
		ContentConstants.displayAssertionGUIContent.image = (assertionCount <= 0) ? StyleConstants.iconAssertionMono : StyleConstants.iconAssertionSmall;
		ContentConstants.displayErrorGUIContent.text = (errorCount > 9999) ? "9999+" : errorCount.ToString();
		ContentConstants.displayErrorGUIContent.image = (errorCount <= 0) ? StyleConstants.iconErrorMono : StyleConstants.iconErrorSmall;
		ContentConstants.displayExceptionGUIContent.text = (exceptionCount > 9999) ? "9999+" : exceptionCount.ToString();
		ContentConstants.displayExceptionGUIContent.image = (exceptionCount <= 0) ? StyleConstants.iconExceptionMono : StyleConstants.iconExceptionSmall;
		EditorGUI.BeginChangeCheck();
		displayAssertion = GUILayout.Toggle(displayAssertion, ContentConstants.displayAssertionGUIContent, StyleConstants.MiniButtonRight);
		displayError = GUILayout.Toggle(displayError, ContentConstants.displayErrorGUIContent, StyleConstants.MiniButtonRight);
		displayException = GUILayout.Toggle(displayException, ContentConstants.displayExceptionGUIContent, StyleConstants.MiniButtonRight);
		if (EditorGUI.EndChangeCheck())
		{
			ReFetchLogEntryDisplayList();
			SetUnityConsoleFlag(EConsoleFlags.LogLevelError, displayAssertion || displayError || displayException);
		}

		GUILayout.EndHorizontal();

		if (GUI.changed)
			RepaintUnityConsole();

		return StyleConstants.MiniButton.fixedHeight;      // 工具栏高度就是ToolbarButton风格的固定高度
	}

	private float DrawToolbar2()
	{
		GUILayout.BeginHorizontal(StyleConstants.Toolbar);
		EditorGUILayout.Space();
		GUILayout.EndHorizontal();
		return StyleConstants.MiniButton.fixedHeight;      // 工具栏高度就是ToolbarButton风格的固定高度
	}

	private void DrawLogList()
	{
		Event current = Event.current;

		logAreaRect = EditorGUILayout.BeginVertical();
		float logDisplayAreaHeight = logAreaRect.height;
		logEntriesScrollPosition = EditorGUILayout.BeginScrollView(logEntriesScrollPosition, StyleConstants.Box, GUILayout.Height(position.height * logAreaHeightRatio));
		float currentDisplayStartY = logEntriesScrollPosition.y;
		float currentDisolayEndY = currentDisplayStartY + logDisplayAreaHeight;
		// 为了显示的需要，下面显示日志条目的地方没有使用Layout，为了确保ScrollView能够正常工作，这里先将显示所有日志条目所需的空间预留出来
		GUILayoutUtility.GetRect(1f, currentDisplayEntries.Count * defaultDisplayLogHeight);
		int currentDisplayCount = 0;
		Rect currentDisplayRect = new Rect(0f, 0f, logAreaRect.width, defaultDisplayLogHeight);
		//int controlID = GUIUtility.GetControlID(FocusType.Passive);
		for (int i = 0; i < currentDisplayEntries.Count; ++i)
		{
			// 判断当前条目是否会进入ScrollView的可视区域
			float y = i * defaultDisplayLogHeight;
			if (y > currentDisolayEndY || y + defaultDisplayLogHeight < currentDisplayStartY)
				continue;
			currentDisplayRect.y = y;
			if (current.type == EventType.MouseDown && current.button == 0 && currentDisplayRect.Contains(current.mousePosition))
			{
				// 检测到鼠标点击当前条目
				if (current.clickCount == 2)
				{
					// 是双击
					UnityInternal.rowGotDoubleClickedMethod.Invoke(null, new object[] { i });
				}
				if (currentSelectedEntry != currentDisplayEntries[i])
				{
					currentSelectedEntry = currentDisplayEntries[i];
					selectChanged = true;
				}
				// 当前点击事件不能再继续向后传递，这很重要！
				current.Use();
			}
			if (Event.current.type == EventType.Repaint)
			{
				// 绘制背景（奇偶数的日志背景颜色不同）
				GUIStyle backgroundStyle = (i % 2 != 0) ? StyleConstants.EvenBackground : StyleConstants.OddBackground;
				backgroundStyle.Draw(currentDisplayRect, false, false, currentSelectedEntry == currentDisplayEntries[i], false);
				// 绘制日志内容
				GUIContent content = GetGUIContentFormLogDisplayPool(currentDisplayCount);
				content.text = currentDisplayEntries[i].fistTwoLines;
				//content.text = currentDisplayEntries[i].ToString();
				GUIStyle textStyle = GetStyleForErrorMode(currentDisplayEntries[i].mode);
				//textStyle.Draw(currentDisplayRect, content, controlID, currentSelectedEntry == currentDisplayEntries[i]);
				textStyle.Draw(currentDisplayRect, content, false, false, currentSelectedEntry == currentDisplayEntries[i], false);
				//EditorGUI.LabelField(currentDisplayRect, content, GetStyleForErrorMode(currentDisplayEntries[i].mode));
			}
			// @TODO: 当选中一个条目时，要确保条目被完全显示在滚动区域中
			// 当选中一条条目时，如果条目有对应的context，需要在Hierachy窗口高亮对应物体
			if (selectChanged && currentSelectedEntry != null)
			{
				SetActiveEntry(currentSelectedEntry);
			}
			++currentDisplayCount;
		}
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		//EditorGUILayout.LabelField(string.Format("Area[{0}], Scroll[{1}], LogCount[{2}], DisplayCount[{3}/{4}]", areaRect, logEntriesScrollPosition, currentDisplayEntries.Count, currentDisplayCount, currentDisplayLogGUIContentPool.Count));
	}

	private void DrawResizeArea()
	{
		Event current = Event.current;

		Rect resizeRect = new Rect(0f, logAreaRect.y + logAreaRect.height - 2.5f, position.width, 5f);
		EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeVertical);
		if (current.type != EventType.Repaint)
		{
			if (current.type == EventType.MouseDown && !resizingLogArea)
			{
				if (resizeRect.Contains(current.mousePosition))
					resizingLogArea = true;
			}
			else if (current.type == EventType.MouseDrag && resizingLogArea)
			{
				logAreaHeightRatio += Event.current.delta.y / position.height;
				logAreaHeightRatio = Mathf.Min(logAreaMaxHeightRatio, Mathf.Max(logAreaMinHeightRatio, logAreaHeightRatio));
				needRepaint = true;
			}
			else if (current.type == EventType.MouseUp && resizingLogArea)
			{
				resizingLogArea = false;
			}
		}
	}

	private void DrawStackTrace()
	{
		Event current = Event.current;
		stackTraceScrollPosition = EditorGUILayout.BeginScrollView(stackTraceScrollPosition, StyleConstants.Box);
		{
			EditorGUILayout.Space();
			// 显示在StackTrace之前的日志内容
			ContentConstants.stackTraceGUIContent.text = currentSelectedEntry != null ? currentSelectedEntry.text : string.Empty;
			float minHeight = StyleConstants.MessageStyle.CalcHeight(ContentConstants.stackTraceGUIContent, position.width);
			if (selectChanged)
			{
				// 当选中的条目发生变化的时候，要将当前的选择状态取消掉
				GUIUtility.hotControl = 0;
				GUIUtility.keyboardControl = 0;
			}
			EditorGUILayout.SelectableLabel(ContentConstants.stackTraceGUIContent.text, StyleConstants.MessageStyle,
				GUILayout.ExpandWidth(true), GUILayout.Height(minHeight)/*GUILayout.ExpandHeight(true), GUILayout.MinHeight(minHeight)*/);
			if (currentSelectedEntry != null && currentSelectedEntry.stackEntries.Count > 0)
			{
				for (int i = 0; i < currentSelectedEntry.stackEntries.Count; ++i)
				{
					StackEntry stackEntry = currentSelectedEntry.stackEntries[i];
					if (GUILayout.Button(stackEntry.stackLabel, StyleConstants.MessageButtonBoldStyle) && Event.current.button == 0)
					{
						OpenEditorToStackEntry(stackEntry, -1);
					}
					if (GUILayout.Button(stackEntry.stackLabel2, StyleConstants.MessageButtonStyle) && Event.current.button == 0)
					{
						OpenEditorToStackEntry(stackEntry, -1);
					}
				}
			}
		}
		EditorGUILayout.EndScrollView();
	}

	private void DrawSplitter(bool vertical = false)
	{
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.border = new RectOffset(1, 1, 1, 1);
		if (!vertical)
			style.stretchWidth = true;
		else
			style.stretchHeight = true;
		style.margin = new RectOffset(0, 0, 0, 0);
		style.normal.background = StyleConstants.splitterTex;
		if (!vertical)
			GUILayout.Box("", style, GUILayout.Height(1f));
		else
			GUILayout.Box("", style, GUILayout.Width(1f));
	}

	#endregion

	/// <summary>
	/// 检查并填充日志列表
	/// </summary>
	/// <returns>true 日志列表内容发生了变化，false 日志列表内容没有发生变化</returns>
	private bool CheckFillLogEntries()
	{
		bool result = false;
		// 先获取到Unity编辑器原生Console窗口里各个复选按钮的状态位
		int unityConsoleFlags = (int)UnityInternal.logEntriesFlagField.GetValue(null, null);
		// 为了能够获取到完整的日志列表，需要先将Unity编辑器原生Console窗口的几个复选按钮的状态设置一下
		SetUnityConsoleFlag(EConsoleFlags.Collapse, false);         // 取消Collapse
		SetUnityConsoleFlag(EConsoleFlags.LogLevelLog, true);       // 显示普通日志
		SetUnityConsoleFlag(EConsoleFlags.LogLevelWarning, true);   // 显示警告日志
		SetUnityConsoleFlag(EConsoleFlags.LogLevelError, true);     // 显示错误日志

		// 对比检查本地日志列表和编辑器Console窗口日志列表是否相同
		int fillLogStartIndex = CheckLogEntriesChanged();
		if (fillLogStartIndex >= 0)
		{
			if (fillLogStartIndex == 0)
				ClearAllLog();
			FetchLogEntriesFromUnityConsole(fillLogStartIndex);
			result = true;
		}

		// 日志获取完之后，再将Unity编辑器原生Console窗口的复选按钮还原
		UnityInternal.logEntriesFlagField.SetValue(null, unityConsoleFlags, null);

		return result;
	}

	private void SetUnityConsoleFlag(EConsoleFlags flags, bool value)
	{
		UnityInternal.setConsoleFlagMethod.Invoke(null, new object[] { (int)flags, value });
	}

	/// <summary>
	/// 对比判断原生Console窗口的日志列表跟本地记录的日志列表是否相同
	/// 日志列表通常情况下是稳定增长的，只要原生窗口的日志列表数量增加了，就说明日志发生了变化
	/// 两种特殊情况下日志列表会被清空，就是原生窗口的Clear按钮和我们自己的窗口的Clear按钮被按下（其实也会调用到原生窗口的Clear），这时日志列表会清零，但反映到日志数量上也是数量发生了变化
	/// 唯一一种数量相同但日志也发生了变化的可能，就是日志列表被清空的瞬间又被填上了N条日志，而N恰好等于Clear之前的日志数量（在存在大量瞬发日志的情况下还是有可能出现的）
	/// 这个时候只需要两边各取第一条日志，对比一下日志的唯一ID看是不是相等就可以知道了
	/// </summary>
	/// <returns>-1 本地日志和编辑器Console窗口日志相同，n 日志不相同时需要更新本地日志的操作起始下标</returns>
	private int CheckLogEntriesChanged()
	{
		int unityConsoleLogCount = (int)UnityInternal.getCountMethod.Invoke(null, null);
		object[] parameter = new object[] { 0, 0, 0 };
		UnityInternal.getCountsByTypeMethod.Invoke(null, parameter);
		int unityConsoleInfoCount = (int)parameter[2];
		int unityConsoleWarningCount = (int)parameter[1];
		int unityConsoleErrorCount = (int)parameter[0];
		int currentLogEntriesCount = currentEntries.Count;

		if (unityConsoleLogCount == currentLogEntriesCount && unityConsoleInfoCount == logCount && unityConsoleWarningCount == warningCount
			 && unityConsoleErrorCount == (assertionCount + errorCount + exceptionCount))
		{
			// 两边日志数量相同时
			if (unityConsoleLogCount == 0)
			{
				// 如果日志数量为0，不用更新日志列表
				return -1;
			}
			else
			{
				return -1;
				/*
				// 日志数量不为0，需要对比检查双方第一条日志是否是同一条
				if ((bool)getEntryMethod.Invoke(null, new object[] { 0, logEntry }))
				{
					int instanceID = (int)logEntryInstanceIDField.GetValue(logEntry);
					if (currentEntries[0].instanceID == instanceID)
						return -1;	// 第一条日志是同一条，那么不用更新日志列表
					else
						return 0;	// 第一条日志不相同，说明编辑器Console窗口日志已经清空过一次了，需要重新获取全部日志列表
				}
				else
					return 0;	// 从编辑器Console窗口取首条日志出错了，直接清空重新获取全部日志列表
					*/
			}
		}
		else if (unityConsoleLogCount > currentLogEntriesCount)
		{
			// 编辑器Console窗口日志数量更多
			if (currentLogEntriesCount == 0)
			{
				// 如果本地记录的日志列表是空的，那么直接获取全部日志就行
				return 0;
			}
			else
			{
				return currentLogEntriesCount - 1;
				/*
				// 对比检查双方首条日志是否同一条
				if ((bool)getEntryMethod.Invoke(null, new object[] { 0, logEntry }))
				{
					int instanceID = (int)logEntryInstanceIDField.GetValue(logEntry);
					if (currentEntries[0].instanceID == instanceID)
						return currentLogEntriesCount - 1;  // 第一条日志是同一条，那么只需要获取编辑器Console窗口多出来的那些日志
					else
						return 0;   // 第一条日志不相同，说明编辑器Console窗口日志已经清空过一次了，需要重新获取全部日志列表
				}
				else
					return 0;   // 从编辑器Console窗口取首条日志出错了，直接清空重新获取全部日志列表
					*/
			}
		}
		else
		{
			// 编辑器Console窗口日志数量更少，需要重新获取全部日志列表
			return 0;
		}
	}

	private void FetchLogEntriesFromUnityConsole(int startIndex)
	{
		int unityConsoleLogCount = (int)UnityInternal.startGettingEntriesMethod.Invoke(null, null);
		object[] parameter = new object[] { 0, 0, "" };
		for (int i = startIndex; i < unityConsoleLogCount; ++i)
		{
			parameter[0] = i;
			UnityInternal.getFirstTwoLinesMethod.Invoke(null, parameter);
			string firstTwoLines = (string)parameter[2];
			if ((bool)UnityInternal.getEntryMethod.Invoke(null, new object[] { i, UnityInternal.logEntry }))
			{
				string wholeText = (string)UnityInternal.logEntryConditionField.GetValue(UnityInternal.logEntry);
				string file = (string)UnityInternal.logEntryFileField.GetValue(UnityInternal.logEntry);
				int line = (int)UnityInternal.logEntryLineField.GetValue(UnityInternal.logEntry);
				int mode = (int)UnityInternal.logEntryModeField.GetValue(UnityInternal.logEntry);
				int instanceID = (int)UnityInternal.logEntryInstanceIDField.GetValue(UnityInternal.logEntry);
				//int identifier = (int)UnityInternal.logEntryIdentifierField.GetValue(UnityInternal.logEntry);
				//int errorNum = (int)UnityInternal.logEntryErrorNumField.GetValue(UnityInternal.logEntry);
				//int isWorldPlaying = (int)UnityInternal.logEntryIsWorldPlayingField.GetValue(UnityInternal.logEntry);
				LogConsoleEntry entry = new LogConsoleEntry
				{
					fistTwoLines = firstTwoLines,
					whole = wholeText,
					file = file,
					line = line,
					mode = mode,
					instanceID = instanceID,
				};
				entry.type = GetLogTypeForErrorMode(mode);
				currentEntries.Add(entry);
				if (ShouldDisplayLogEntry(mode))
					currentDisplayEntries.Add(entry);
				AccumulateLogCount(entry.type, 1);

				PostProcessLogEntry(entry);
			}
		}
		UnityInternal.endGettingEntriesMethod.Invoke(null, null);
	}

	private void PostProcessLogEntry(LogConsoleEntry entry)
	{
		// 日志详情内容按照不同日志类型可以划分为几个不同的模板：
		// 编译错误和编译警告
		// （ScriptCompileError，ScriptCompileWarning）
		// 模板形式：[文件相对路径]([行号],[列号]): [错误/警告代码]: [具体错误/警告描述]
		// Shader导入错误和导入警告
		// （AssetImportError，AssetImportWarning）
		// 模板形式：Shader ... in '[Shader路径]': [具体错误/警告描述] at line [行号] ...
		// 模板形式：[可能多行的描述]\n([最后一行为Shader文件路径])
		// Debug日志输出
		// （ScriptingLog，ScriptingWarning，ScriptingError，ScriptingException）
		// 模板形式：[可能多行的日志内容]\n[命名空间].[类名].[函数名]([函数参数类型列表])(at [文件路径]:[行号])\n...
		// 其他杂项
		// 模板形式：没有固定样式，比如换行符不一致的警告，自动升级代码的通知，Unity内置消息弃用的警告等
		if (entry.stackEntries == null)
			entry.stackEntries = new List<StackEntry>();
		else
			entry.stackEntries.Clear();
		if(HasMode(entry.mode, EMode.ScriptCompileWarning | EMode.ScriptCompileError))
		{
			string[] temp = entry.whole.Split(':');
			if(temp.Length >= 3)
			{

				entry.text = string.Format("<color={0}>[{1}]</color> {2}", (entry.type == LogType.Warning) ? "yellow" : "#dd2222ff", temp[1].Trim(), temp[2].Trim());
				temp = temp[0].Split(new char[] { '(', ',', ')' });
				if(temp.Length >= 3)
				{
					string file = temp[0];
					int lineNum = 0;
					int.TryParse(temp[1], out lineNum);
					int charNum = 0;
					int.TryParse(temp[2], out charNum);
					StackEntry stack = new StackEntry();
					stack.fileName = file;
					stack.lineNumber = lineNum;
					stack.charNumber = charNum;
					stack.stackLabel = "Compile " + entry.type.ToString();
					if(charNum > 0)
						stack.stackLabel2 = string.Format("    {0}: line {1} column {2}", file.Replace("Assets/", ""), lineNum, charNum);
					else
						stack.stackLabel2 = string.Format("    {0}: line {1}", file.Replace("Assets/", ""), lineNum);
					entry.stackEntries.Add(stack);
				}
			}
			else
			{
				entry.text = entry.whole;
			}
		}
		else if(HasMode(entry.mode, EMode.AssetImportWarning | EMode.AssetImportError))
		{
			entry.text = entry.whole;
		}
		else if(HasMode(entry.mode, EMode.ScriptingLog | EMode.ScriptingWarning | EMode.ScriptingException))
		{
			entry.text = entry.whole;
		}
		else
		{
			entry.text = entry.whole;
		}
	}

	private GUIContent GetGUIContentFormLogDisplayPool(int index)
	{
		if (index < currentDisplayLogGUIContentPool.Count)
			return currentDisplayLogGUIContentPool[index];
		else
		{
			while (index >= currentDisplayLogGUIContentPool.Count)
				currentDisplayLogGUIContentPool.Add(new GUIContent(string.Empty, StyleConstants.iconInfo));
			return currentDisplayLogGUIContentPool[index];
		}
	}

	private void ClearAllLog()
	{
		currentEntries.Clear();
		currentDisplayEntries.Clear();
		currentSelectedEntry = null;
		selectChanged = true;
		logCount = 0;
		warningCount = 0;
		errorCount = 0;
		assertionCount = 0;
		exceptionCount = 0;
	}

	private void AccumulateLogCount(LogType type, int num)
	{
		switch (type)
		{
			case LogType.Log:
				logCount += num;
				break;
			case LogType.Warning:
				warningCount += num;
				break;
			case LogType.Error:
				errorCount += num;
				break;
			case LogType.Assert:
				assertionCount += num;
				break;
			case LogType.Exception:
				exceptionCount += num;
				break;
			default:
				break;
		}
	}

	private void SetActiveEntry(LogConsoleEntry entry)
	{
		if (entry != null)
		{
			if (entry.instanceID != 0)
				EditorGUIUtility.PingObject(entry.instanceID);
		}
		else
		{
			//this.m_ActiveText = string.Empty;
			//this.m_ActiveInstanceID = 0;
			//this.m_ListView.row = -1;
		}
	}

	private void RepaintUnityConsole()
	{
		Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(type);
		EditorWindow editorWindow = (array.Length > 0) ? (array[0] as EditorWindow) : null;
		if (editorWindow != null)
		{
			editorWindow.Repaint();
		}
	}

	private bool ShouldDisplayLogEntry(int entryMode)
	{
		// @TODO: 应用查找过滤器
		LogType type = GetLogTypeForErrorMode(entryMode);
		switch (type)
		{
			case LogType.Log:
				return displayInfo;
			case LogType.Warning:
				return displayWarning;
			case LogType.Assert:
				return displayAssertion;
			case LogType.Error:
				return displayError;
			case LogType.Exception:
				return displayException;
			default:
				return false;
		}
	}

	private void ReFetchLogEntryDisplayList()
	{
		currentDisplayEntries.Clear();
		for (int i = 0; i < currentEntries.Count; ++i)
		{
			LogConsoleEntry entry = currentEntries[i];
			if (ShouldDisplayLogEntry(entry.mode))
				currentDisplayEntries.Add(entry);
		}
	}

	/// <summary>
	/// 检查Unity原生Console窗口上的复选按钮状态跟自己窗口上的对应复选按钮状态是否相同
	/// </summary>
	/// <returns>true 复选按钮状态不一样，false 复选按钮状态一样</returns>
	private bool CheckUnityConsoleToggleStatesChanged(int unityConsoleFlags)
	{
		bool result = false;

		bool unityConsole_ClearOnPlay = HasFlag(unityConsoleFlags, EConsoleFlags.ClearOnPlay);
		if (clearOnPlay != unityConsole_ClearOnPlay)
		{
			clearOnPlay = unityConsole_ClearOnPlay;
			result = true;
		}
		bool unityConsole_ErrorPause = HasFlag(unityConsoleFlags, EConsoleFlags.ErrorPause);
		if (errorPause != unityConsole_ErrorPause)
		{
			errorPause = unityConsole_ErrorPause;
			result = true;
		}
		bool unityConsole_StopForAssert = HasFlag(unityConsoleFlags, EConsoleFlags.StopForAssert);
		if (stopForAssert != unityConsole_StopForAssert)
		{
			stopForAssert = unityConsole_StopForAssert;
			result = true;
		}
		bool unityConsole_StopForError = HasFlag(unityConsoleFlags, EConsoleFlags.StopForError);
		if (stopForError != unityConsole_StopForError)
		{
			stopForError = unityConsole_StopForError;
			result = true;
		}

		return result;
	}

	bool CheckUnityConsoleDisplayToggleStateChanged(int unityConsoleFlags)
	{
		bool result = false;

		bool unityConsole_CollapseEntries = HasFlag(unityConsoleFlags, EConsoleFlags.Collapse);
		if (collapseEntries != unityConsole_CollapseEntries)
		{
			collapseEntries = unityConsole_CollapseEntries;
			result = true;
		}
		bool unityConsole_DisplayInfo = HasFlag(unityConsoleFlags, EConsoleFlags.LogLevelLog);
		if (displayInfo != unityConsole_DisplayInfo)
		{
			displayInfo = unityConsole_DisplayInfo;
			result = true;
		}
		bool unityConsole_DisplayWarning = HasFlag(unityConsoleFlags, EConsoleFlags.LogLevelWarning);
		if (displayWarning != unityConsole_DisplayWarning)
		{
			displayWarning = unityConsole_DisplayWarning;
			result = true;
		}
		bool unityConsole_DisplayError = HasFlag(unityConsoleFlags, EConsoleFlags.LogLevelError);
		if ((displayAssertion || displayError || displayException) != unityConsole_DisplayError)
		{
			displayAssertion = unityConsole_DisplayError;
			displayError = unityConsole_DisplayError;
			displayException = unityConsole_DisplayError;
			result = true;
		}

		return result;
	}

	private void OpenEditorToStackEntry(StackEntry stackEntry, int lineNum = -1)
	{
		UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(stackEntry.fileName, typeof(TextAsset));
		if (obj != null)
		{
			AssetDatabase.OpenAsset(obj, (lineNum != -1) ? lineNum : stackEntry.lineNumber);
		}
	}


}
