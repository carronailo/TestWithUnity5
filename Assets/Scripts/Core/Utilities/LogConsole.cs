using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UnityDebug = UnityEngine.Debug;

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

	public static bool RaiseAssertException
	{
		get
		{
			return UnityEngine.Assertions.Assert.raiseExceptions;
		}
		set
		{
			UnityEngine.Assertions.Assert.raiseExceptions = value;
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

	public static void Log(object message)
	{
		_Log(LogType.Log, string.Empty, message.ToString(), null);
	}

	public static void Log(string tag, string message)
	{
		_Log(LogType.Log, tag, message, null);
	}

	public static void Log(string tag, object message)
	{
		_Log(LogType.Log, tag, message.ToString(), null);
	}

	public static void Log(string message, UnityObject context)
	{
		_Log(LogType.Log, string.Empty, message, context);
	}

	public static void Log(object message, UnityObject context)
	{
		_Log(LogType.Log, string.Empty, message.ToString(), context);
	}

	public static void Log(string tag, string message, UnityObject context)
	{
		_Log(LogType.Log, tag, message, context);
	}

	public static void Log(string tag, object message, UnityObject context)
	{
		_Log(LogType.Log, tag, message.ToString(), context);
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

	public static void LogWarning(object message)
	{
		_Log(LogType.Warning, string.Empty, message.ToString(), null);
	}

	public static void LogWarning(string tag, string message)
	{
		_Log(LogType.Warning, tag, message, null);
	}

	public static void LogWarning(string tag, object message)
	{
		_Log(LogType.Warning, tag, message.ToString(), null);
	}

	public static void LogWarning(string message, UnityObject context)
	{
		_Log(LogType.Warning, string.Empty, message, context);
	}

	public static void LogWarning(object message, UnityObject context)
	{
		_Log(LogType.Warning, string.Empty, message.ToString(), context);
	}

	public static void LogWarning(string tag, string message, UnityObject context)
	{
		_Log(LogType.Warning, tag, message, context);
	}

	public static void LogWarning(string tag, object message, UnityObject context)
	{
		_Log(LogType.Warning, tag, message.ToString(), context);
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

	public static void LogError(object message)
	{
		_Log(LogType.Error, string.Empty, message.ToString(), null);
	}

	public static void LogError(string tag, string message)
	{
		_Log(LogType.Error, tag, message, null);
	}

	public static void LogError(string tag, object message)
	{
		_Log(LogType.Error, tag, message.ToString(), null);
	}

	public static void LogError(string message, UnityObject context)
	{
		_Log(LogType.Error, string.Empty, message, context);
	}

	public static void LogError(object message, UnityObject context)
	{
		_Log(LogType.Error, string.Empty, message.ToString(), context);
	}

	public static void LogError(string tag, string message, UnityObject context)
	{
		_Log(LogType.Error, tag, message, context);
	}

	public static void LogError(string tag, object message, UnityObject context)
	{
		_Log(LogType.Error, tag, message.ToString(), context);
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

	#endregion

	private static void _Assert(bool condition, string message)
	{
		UnityEngine.Assertions.Assert.IsTrue(condition, message);
	}

	private static void _Log(LogType logType, string tag, string message, UnityObject context)
	{
		if (Instance != null)
		{
			if (string.IsNullOrEmpty(tag))
			{
				Instance.logger.Log(logType, (object)message, context);
			}
			else
			{
				Instance.logger.Log(logType, tag, message, context);
			}
		}
	}

	private static void _LogFormat(LogType logType, UnityObject context, string format, object[] args)
	{
		if (Instance != null)
		{
			Instance.logger.LogFormat(LogType.Log, context, format, args);
		}
	}

	private static void _LogException(Exception exception, UnityObject context)
	{
		if (Instance != null)
		{
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

	// 为了能够区别于Debug，单独设置仅限于使用LogConsole时才生效的日志过滤和日志开关，还是需要单独定义一个Logger的
	// 但是这个Logger在接收到日志的时候一定会转发给Debug，为了能够获取到日志的堆栈信息
	private Logger logger = null;


	private LogConsole()
	{
		Init();
		//Application.logMessageReceived += OnLogMessageReceived;
	}

	~LogConsole()
	{
		//Application.logMessageReceived -= OnLogMessageReceived;
	}

	void Init()
	{
		logger = new Logger(new LogConsoleLoggerHandler());
	}

	void OnLogMessageReceived(string message, string stackTrace, LogType logType)
	{
		// @TODO: 如果是部署环境运行，那么需要将日志转发到自制的控制台UI上
	}
}
