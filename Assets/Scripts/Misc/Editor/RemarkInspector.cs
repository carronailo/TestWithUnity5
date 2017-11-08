using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Remark), true)]
public class RemarkInspector : Editor
{
	private SerializedProperty remark;
	private Vector2 scrollPos;

	[MenuItem("Component/添加备注")]
	private static void AddRemark()
	{
		if (Selection.activeGameObject != null)
			Selection.activeGameObject.AddComponent<Remark>();
	}

	private void OnEnable()
	{
		remark = serializedObject.FindProperty("remark");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.BeginVertical(GUILayout.Height(200f));
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(100f));
		remark.stringValue = EditorGUILayout.TextArea(remark.stringValue, GUILayout.Height(400));
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties();
	}
}
