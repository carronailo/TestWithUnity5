using UnityEngine;
using System.Collections;
using UnityEditor;

namespace CinemaDirector
{
	[CustomEditor(typeof(AttachChildrenEvent))]
	public class AttachChildrenEventInspector : MyCustomInspector
	{
		SerializedProperty Children;
		SerializedProperty ChildrenTag;
		SerializedProperty firetime;

		public override void OnEnable()
		{
			base.OnEnable();
			Children = serializedObject.FindProperty("Children");
			ChildrenTag = serializedObject.FindProperty("ChildrenTag");
			firetime = serializedObject.FindProperty("firetime");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			// script
			DisplayScriptField();
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(firetime);
			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			{
				EditorGUILayout.PropertyField(Children, new GUIContent("子物体："), true);
			}
			if(EditorGUI.EndChangeCheck())
			{
				ChildrenTag.arraySize = Children.arraySize;
				for(int i = 0; i < Children.arraySize; ++i)
				{
					SerializedProperty prop = ChildrenTag.GetArrayElementAtIndex(i);
					prop.stringValue = (Children.GetArrayElementAtIndex(i).objectReferenceValue as GameObject).tag;
				}
			}
			GUI.enabled = false;
			EditorGUILayout.PropertyField(ChildrenTag, new GUIContent("子物体Tag："), true);
			GUI.enabled = true;

			serializedObject.ApplyModifiedProperties();
		}
	}
}
