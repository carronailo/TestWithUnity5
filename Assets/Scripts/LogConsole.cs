using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityObject = UnityEngine.Object;
using UnityDebug = UnityEngine.Debug;
using System.Text.RegularExpressions;

/// <summary>
/// (对应Unity5.6)
/// 主要为了实现在不变更使用Unity引擎Debug.Log系列日志输入函数习惯的基础上，将日志输出功能扩展至运行期的自制的控制台（包括移动平台）
/// 1、为了不变更对Debug.Log系列函数的使用习惯，需要全面接管Debug.Log的系列函数，而且提供类似的调用接口（增加显示赋值的Tag参数）
/// 2、为了能够将日志输出转定位到自制的控制台（可能是存储文件），需要暴露对日志转储的回调注册和反注册
/// </summary>
public sealed class LogConsole
{
	private static LogConsole Instance
	{
		get
		{
			if (_instance == null)
				_instance = new LogConsole();
			return _instance;
		}
	}
	private static LogConsole _instance = null;

	#region Static Public APIs

	public static bool LogEnabled
	{
		get
		{
			return Instance.logger.logEnabled;
		}
		set
		{
			Instance.logger.logEnabled = value;
		}
	}

	public static LogType LogLevel
	{
		get
		{
			return Instance.logger.filterLogType;
		}
		set
		{
			Instance.logger.filterLogType = value;
		}
	}

	public static bool SendToUnityEditorConsole
	{
		get
		{
			return Instance.sendToUnityEditorConsole;
		}
		set
		{
			Instance.sendToUnityEditorConsole = value;
		}
	}


	/// <summary>
	/// 注意，修改日志队列长度上限会导致当前已经保存在队列中的日志被清空
	/// </summary>
	public static int LogQueueLimit
	{
		get
		{
			return Instance.logQueueLimit;
		}
		set
		{
			Instance.ChangeLogQueueLimit(value);
		}
	}

	public static void Assert(bool condition)
	{
		Assert(condition, null);
	}

	public static void Assert(bool condition, string message)
	{
		_Assert(condition, message);
	}

	public static void AssertFormat(bool condition, string format, params object[] args)
	{
		_Assert(condition, string.Format(format, args));
	}

	public static void Log(string message)
	{
		_Log(LogType.Log, string.Empty, message, null);
	}

	public static void Log(string tag, string message)
	{
		_Log(LogType.Log, tag, message, null);
	}

	public static void Log(string message, UnityObject context)
	{
		_Log(LogType.Log, string.Empty, message, context);
	}

	public static void Log(string tag, string message, UnityObject context)
	{
		_Log(LogType.Log, tag, message, context);
	}

	public static void LogFormat(string format, params object[] args)
	{
		_LogFormat(LogType.Log, null, format, args);
	}

	public static void LogFormat(UnityObject context, string format, params object[] args)
	{
		_LogFormat(LogType.Log, context, format, args);
	}

	public static void LogWarning(string message)
	{
		_Log(LogType.Warning, string.Empty, message, null);
	}

	public static void LogWarning(string tag, string message)
	{
		_Log(LogType.Warning, tag, message, null);
	}

	public static void LogWarning(string message, UnityObject context)
	{
		_Log(LogType.Warning, string.Empty, message, context);
	}

	public static void LogWarning(string tag, string message, UnityObject context)
	{
		_Log(LogType.Warning, tag, message, context);
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		_LogFormat(LogType.Warning, null, format, args);
	}

	public static void LogWarningFormat(UnityObject context, string format, params object[] args)
	{
		_LogFormat(LogType.Warning, context, format, args);
	}

	public static void LogError(string message)
	{
		_Log(LogType.Error, string.Empty, message, null);
	}

	public static void LogError(string tag, string message)
	{
		_Log(LogType.Error, tag, message, null);
	}

	public static void LogError(string message, UnityObject context)
	{
		_Log(LogType.Error, string.Empty, message, context);
	}

	public static void LogError(string tag, string message, UnityObject context)
	{
		_Log(LogType.Error, tag, message, context);
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		_LogFormat(LogType.Error, null, format, args);
	}

	public static void LogErrorFormat(UnityObject context, string format, params object[] args)
	{
		_LogFormat(LogType.Error, context, format, args);
	}

	public static void LogException(Exception exception)
	{
		_LogException(exception, null);
	}

	public static void LogException(Exception exception, UnityObject context)
	{
		_LogException(exception, context);
	}

	public static void GetAllLog(ref List<string> output)
	{
		if(Instance != null)
		{
			output.Clear();
			foreach (LogContent log in Instance.logQueue)
			{
				output.Add(log.ToString());
			}
		}
	}

	#endregion

	private static void _Assert(bool condition, string message)
	{
		if (!condition)
		{
			throw new AssertionException(message, null);
		}
	}

	private static void _Log(LogType logType, string tag, string message, UnityObject context)
	{
		if (Instance != null)
		{
			if (string.IsNullOrEmpty(tag))
			{
				long id = Instance.RecordLog(logType, string.Empty, context);
				Instance.logger.Log(logType, (object)(string.Format("{0}[#{1}]", message, id)), context);
			}
			else
			{
				long id = Instance.RecordLog(logType, tag, context);
				Instance.logger.Log(logType, tag, string.Format("{0}[#{1}]", message, id), context);
			}
		}
	}

	private static void _LogFormat(LogType logType, UnityObject context, string format, object[] args)
	{
		if (Instance != null)
		{
			long id = Instance.RecordLog(logType, string.Empty, context);
			Instance.logger.LogFormat(LogType.Log, context, string.Format("{0}[#{1}]", format, id), args);
		}
	}

	private static void _LogException(Exception exception, UnityObject context)
	{
		if (Instance != null)
		{
			Instance.RecordLog(LogType.Exception, string.Empty, exception.Message, exception.StackTrace, context);
			Instance.logger.LogException(exception, context);
		}
	}

	private class LogConsoleLoggerHandler : ILogHandler
	{
		public void LogException(Exception exception, UnityObject context)
		{
			UnityDebug.logger.LogException(exception, context);
		}

		public void LogFormat(LogType logType, UnityObject context, string format, params object[] args)
		{
			UnityDebug.logger.LogFormat(logType, context, format, args);
		}
	}

	private class LogContent
	{
		public LogType logType;
		public string logTag;
		public string logContent;
		public string stackTrace;
		public UnityObject context;
		public long uniqueID;

		public string ToString()
		{
			return string.Format("{0}-{1}:{2}[#{3}]{4}{5}", logType, logTag, logContent, uniqueID, Environment.NewLine, stackTrace);
		}
	}

	private bool sendToUnityEditorConsole = true;

	private int logQueueLimit = 3000;

	private Queue<LogContent> logQueue = null;
	private Dictionary<long, LogContent> logTable = null;
	private Dictionary<LogType, Dictionary<string, LogContent>> typeTagContentTable = null;
	private Dictionary<string, Dictionary<LogType, LogContent>> tagTypeContentTable = null;

	// 为了能够区别于Debug，单独设置仅限于使用LogConsole时才生效的日志过滤和日志开关，还是需要单独定义一个Logger的
	// 但是这个Logger在接收到日志的时候一定会转发给Debug，为了能够获取到日志的堆栈信息
	private Logger logger = null;

	private LogConsole()
	{
		Init();
		Application.logMessageReceived += OnLogMessageReceived;
	}

	~LogConsole()
	{
		Application.logMessageReceived -= OnLogMessageReceived;
	}

	void Init()
	{
		logger = new Logger(new LogConsoleLoggerHandler());
		logQueue = new Queue<LogContent>(logQueueLimit);
		logTable = new Dictionary<long, LogContent>();
		typeTagContentTable = new Dictionary<LogType, Dictionary<string, LogContent>>();
		tagTypeContentTable = new Dictionary<string, Dictionary<LogType, LogContent>>();
	}

	void ChangeLogQueueLimit(int value)
	{
		logQueue.Clear();
		logTable.Clear();
		logQueueLimit = value;
		logQueue = new Queue<LogContent>(logQueueLimit);
		typeTagContentTable.Clear();
		tagTypeContentTable.Clear();
	}

	long RecordLog(LogType logType, string tag, UnityObject context)
	{
		return RecordLog(logType, tag, null, null, context);
	}

	long RecordLog(LogType logType, string tag, string message, string stackTrace, UnityObject context)
	{
		long uniqueID = UniqueIDGenerator.GetID();
		LogContent log = new LogContent() { logType = logType, logTag = tag, context = context, uniqueID = uniqueID };
		if (logQueue.Count >= logQueueLimit)
		{
			LogContent oldLog = logQueue.Dequeue();
			logTable.Remove(oldLog.uniqueID);
			CollectionUtil.RemoveFromTable(oldLog.logType, oldLog.logTag, typeTagContentTable);
			CollectionUtil.RemoveFromTable(oldLog.logTag, oldLog.logType, tagTypeContentTable);
		}
		logQueue.Enqueue(log);
		logTable.Add(log.uniqueID, log);
		CollectionUtil.AddIntoTable(log.logType, log.logTag, typeTagContentTable);
		CollectionUtil.AddIntoTable(log.logTag, log.logType, tagTypeContentTable);
		if (message != null)
			log.logContent = message;
		if (stackTrace != null)
			log.stackTrace = stackTrace;
		return uniqueID;
	}

	// [#1234567890000]
	const string idPattern = "\\[#([0-9]+)\\]";
	void OnLogMessageReceived(string message, string stackTrace, LogType logType)
	{
		Match match = Regex.Match(message, idPattern);
		if(match.Success)
		{
			string idStr = match.Groups[1].Value;
			long id = 0L;
			LogContent log = null;
			if (long.TryParse(idStr, out id) && logTable.TryGetValue(id, out log))
			{
				log.logContent = message.Replace(match.Groups[0].Value, string.Empty);
				log.stackTrace = stackTrace;
			}
		}
	}
}
