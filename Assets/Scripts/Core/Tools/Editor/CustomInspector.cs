using UnityEditor;
using UnityEngine;

public class CustomInspector : Editor
{
	protected SerializedProperty script;

	private GUIContent scriptLabelContent;

	public virtual void OnEnable()
	{
		script = serializedObject.FindProperty("m_Script");
		scriptLabelContent = new GUIContent("Script");
	}

	public void DisplayScriptField()
	{
		EditorGUILayout.BeginHorizontal();
		{
			bool guiEnabled = GUI.enabled;
			GUI.enabled = false;
			EditorGUILayout.PropertyField(script, scriptLabelContent);
			GUI.enabled = guiEnabled;
		}
		EditorGUILayout.EndHorizontal();
	}
}
