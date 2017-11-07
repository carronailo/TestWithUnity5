using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(SetParent), true)]
public class SetParentInspector : Editor
{
	private SerializedProperty parent;
	private SerializedProperty parentTag;
	private SerializedProperty firetime;

	private void OnEnable()
	{
		parent = serializedObject.FindProperty("parent");
		parentTag = serializedObject.FindProperty("parentTag");
		firetime = serializedObject.FindProperty("firetime");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(firetime);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(parent);
		if(EditorGUI.EndChangeCheck())
		{
			if (parent.objectReferenceValue == null)
				parentTag.stringValue = string.Empty;
			else
			{
				GameObject temp = (parent.objectReferenceValue as GameObject);
				if (temp == null)
					parentTag.stringValue = string.Empty;
				else
					parentTag.stringValue = temp.tag;
			}
		}
		GUI.enabled = false;
		EditorGUILayout.PropertyField(parentTag);
		GUI.enabled = true;

		serializedObject.ApplyModifiedProperties();
	}
}
