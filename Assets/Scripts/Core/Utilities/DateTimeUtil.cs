using System;

public static class DateTimeUtil
{
	public static long dayToMilli = 24 * 60 * 60 * 1000;
	public static long hourToMilli = 60 * 60 * 1000;
	public static long minuteToMilli = 60 * 1000;

	public static long NowMillisecond
	{
		get
		{
			return DateTime.Now.Ticks / 10000;
		}
	}

	/// <summary>
	/// 将毫秒数转为日期字符串，一般用来将Java服务器端用System.currentMilliSeconds获得的毫秒数转为本地时区的日期字符串
	/// </summary>
	/// <param name="milli">如果此参数不是Java端System.currentMilliSeconds获得的数值，请确保它是从格林威治时间1970年1月1日0点0分0秒开始计时的毫秒数</param>
	/// <returns></returns>
	public static string ConvertMilliToDateString_UTC19700101(long milli)
	{
		DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milli);
		return date.ToLocalTime().ToString("yyyy/MM/dd");
	}

	/// <summary>
	/// 将毫秒数转为日期时间字符串，一般用来将Java服务器端用System.currentMilliSeconds获得的毫秒数转为本地时区的日期时间字符串
	/// </summary>
	/// <param name="milli">如果此参数不是Java端System.currentMilliSeconds获得的数值，请确保它是从格林威治时间1970年1月1日0点0分0秒开始计时的毫秒数</param>
	/// <returns></returns>
	public static string ConvertMilliToDateTimeString_UTC19700101(long milli)
	{
		DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milli);
		return date.ToLocalTime().ToString("yyyy/MM/dd hh:mm:ss");
	}

	/// <summary>
	/// 计算从毫秒数代表的日期时间到函数调用时，已经过去了多少毫秒，一般用来处理Java服务器端用System.currentMilliSeconds获得的毫秒数
	/// </summary>
	/// <param name="milli">如果此参数不是Java端System.currentMilliSeconds获得的数值，请确保它是从格林威治时间1970年1月1日0点0分0秒开始计时的毫秒数</param>
	/// <returns>注意，鉴于服务器与客户端时间不统一，返回值有可能为负数</returns>
	public static long MilliPastSinceThen_UTC19700101(long milli)
	{
		DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milli);
		return (DateTime.UtcNow.Ticks - date.Ticks) / 10000;
	}

	public static int CalcDays(long milli)
	{
		int day = (int)(milli / dayToMilli);
		long extra = milli % dayToMilli;
		return extra > 0 ? ++day : day;
	}
}
