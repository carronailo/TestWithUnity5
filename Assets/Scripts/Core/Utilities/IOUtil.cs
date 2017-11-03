using UnityEngine;
using System.Collections;
using System.IO;
using System;

public static class IOUtil
{
	public static void MakeSurePath(string path)
	{
		if (!Directory.Exists(path))
		{
			LogConsole.Log("正在创建目录…… " + path);
			try
			{
				Directory.CreateDirectory(path);
			}
			catch(Exception ex)
			{
				LogConsole.LogException(ex);
				LogConsole.Log("创建目录失败！ " + path);
			}
		}
	}
}
