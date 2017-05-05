using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InputModuleManager))]
public class InputModuleManagerInspector : Editor
{
	SerializedProperty usedInputModules = null;

	private void OnEnable()
	{
		usedInputModules = serializedObject.FindProperty("usedInputModules");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUI.BeginChangeCheck();
		EInputModule usedInputModulesValue = (EInputModule)usedInputModules.intValue;
		usedInputModulesValue = (EInputModule)EditorGUILayout.EnumMaskField("启用的输入模块：", usedInputModulesValue);
		usedInputModules.intValue = (int)usedInputModulesValue;

		serializedObject.ApplyModifiedProperties();
	}
}
