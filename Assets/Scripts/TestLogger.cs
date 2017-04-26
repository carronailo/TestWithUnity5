using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Test
{
	public class TestLogger : MonoBehaviour
	{
		public bool enable;
		public LogType type;

		public GUIStyle s;
		public GUIStyle s1;

		public Texture2D t;
		public Texture2D t2d;

		public GUIContent content;
		// Use this for initialization
		void Start()
		{
			t.anisoLevel = 1;
			Debug.logger.logEnabled = true;
			Debug.logger.filterLogType = LogType.Log;
		}

		// Update is called once per frame
		void Update()
		{
			LogConsole.LogEnabled = enable;
			LogConsole.LogLevel = type;
		}

		private void OnGUI()
		{
			string tb = "ToolbarButton";
			s = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("SearchTextField");
			s1 = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("CN Message");
			if (GUI.Button(new Rect(10, 10, 400, 200), "Print Log"))
			{
				Debug.Log("Test Unity Log");
				Assert.IsTrue(false);
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
				catch (System.Exception ex)
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

				Debug.LogAssertion("Unity assertion");

				LogConsole.Assert(false, "Test if this assert can be printed in console");
			}
		}
	}
}

