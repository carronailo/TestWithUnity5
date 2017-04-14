using System;
using System.Text;

public static class Delegate_ToString_Extension
{
	public static string ToStringEx(this Delegate d)
	{
		return string.Format("实例[{0}]上的方法[{1}]", d.Target.ToString(), d.Method.ToString());
	}
}

public static class StringUtility
{
	public static string ToString(object p)
	{
		if (p == null)
			return "null";
		else if (p is Delegate)
			return ((Delegate)p).ToStringEx();
		else
			return p.ToString();
	}

	public static string ParseKiloInt(int i)
	{
		return ParseKiloInt(i, 9999);
	}

	public static string ParseKiloInt(int i, int threshold)
	{
		if (i > threshold)
			return (i / 1000).ToString() + "K";
		else if (i >= 0)
			return i.ToString();
		else if (i > -10000)
			return i.ToString();
		else
			return (i / 1000).ToString() + "K";
	}

	public static string ParseSegmentInt(int i)
	{
		bool isNeg = i < 0;
		int abs = isNeg ? -i : i;
		int million = abs / 1000000;
		string strMillion = million == 0 ? "" : million.ToString() + ",";
		int thousand = (abs % 1000000) / 1000;
		string strThousand = million == 0 ? (thousand == 0 ? "" : thousand.ToString() + ",") : string.Format("{0:d3},", thousand);
		int unit = abs % 1000;
		string strUnit = (million == 0 && thousand == 0) ? unit.ToString() : string.Format("{0:d3}", unit);
		return (isNeg ? "-" : "") + strMillion + strThousand + strUnit;
	}

	public static string ToUrlEncode(string strCode)
	{
		StringBuilder sb = new StringBuilder();
		byte[] byStr = Encoding.UTF8.GetBytes(strCode); //默认是System.Text.Encoding.Default.GetBytes(str) 
		System.Text.RegularExpressions.Regex regKey = new System.Text.RegularExpressions.Regex("^[A-Za-z0-9]+$");
		for (int i = 0; i < byStr.Length; i++)
		{
			string strBy = Convert.ToChar(byStr[i]).ToString();
			if (regKey.IsMatch(strBy))
			{
				//是字母或者数字则不进行转换  
				sb.Append(strBy);
			}
			else
			{
				sb.Append(@"%" + Convert.ToString(byStr[i], 16));
			}
		}
		return (sb.ToString());
	}
}
