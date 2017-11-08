using UnityEditor;
using UnityEngine;

public class MyCustomInspector : Editor
{
	protected SerializedProperty script;
	
	public virtual void OnEnable()
	{
		script = serializedObject.FindProperty("m_Script");
	}

	public void DisplayScriptField()
	{
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.LabelField("Script", GUILayout.Width(50f));
			EditorGUILayout.PropertyField(script, GUIContent.none);
		}
		EditorGUILayout.EndHorizontal();
	}
}
