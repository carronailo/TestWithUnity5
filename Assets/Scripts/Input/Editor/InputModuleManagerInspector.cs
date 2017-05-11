using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InputModuleManager))]
public class InputModuleManagerInspector : CustomInspector
{
	SerializedProperty usedInputModules = null;

	public override void OnEnable()
	{
		base.OnEnable();
		usedInputModules = serializedObject.FindProperty("usedInputModules");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		DisplayScriptField();

		EditorGUI.BeginChangeCheck();
		EInputModule usedInputModulesValue = (EInputModule)usedInputModules.intValue;
		usedInputModulesValue = (EInputModule)EditorGUILayout.EnumMaskField("启用的输入模块：", usedInputModulesValue);
		usedInputModules.intValue = (int)usedInputModulesValue;

		serializedObject.ApplyModifiedProperties();
	}
}
