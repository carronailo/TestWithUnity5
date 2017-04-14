using System;

public static class LogUtil
{
	public static Action<Exception> ShowException = null;
	public static Action<string> ShowError = null;
	public static Action<string> ShowWarning = null;
	public static Action<string> ShowInfo = null;
	public static Action<string> ShowDebug = null;
}
