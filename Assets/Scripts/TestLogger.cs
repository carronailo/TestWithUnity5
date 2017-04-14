using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TestLogger : MonoBehaviour
{
	public bool enable;
	public LogType type;

	public List<string> allLogs;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		LogConsole.LogEnabled = enable;
		LogConsole.LogLevel = type;

		LogConsole.GetAllLog(ref allLogs);
	}

	private void OnGUI()
	{
		if(GUI.Button(new Rect(10, 10, 400, 200), "Print Log"))
		{
			LogConsole.Log("Test Log");
			LogConsole.Log("MyTag", "Test Log With Tag");
			LogConsole.Log("Test Log With Context", this);
			LogConsole.Log("MyTag", "Test Log With Tag And Context", this);
			LogConsole.LogFormat("Test Log {0} {1}({2},{3})", "With", "Format", 1, 2.9f);
			LogConsole.LogFormat(this, "Test Log {0} {1}({2},{3})", "With", "Format And Context", 1, 2.9f);
			LogConsole.LogWarning("Test Warning");
			LogConsole.LogError("Test Error");
			try
			{
				throw new System.Exception("Test Exception");
			}
			catch(System.Exception ex)
			{
				LogConsole.LogException(ex);
			}
			try
			{
				throw new System.Exception("Test Exception With Context");
			}
			catch (System.Exception ex)
			{
				LogConsole.LogException(ex, this);
			}

			LogConsole.Assert(false, "Test if this assert can be printed in console");
		}
	}
}
