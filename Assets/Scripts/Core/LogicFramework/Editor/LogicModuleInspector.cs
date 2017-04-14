#if UNITY_EDITOR
using LogicFramework;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LogicModule), true)]
public class LogicModuleInspector : Editor
{
	protected SerializedProperty disableMessageProcessor;
	protected SerializedProperty disableNetworkMessageProcessor;
	protected SerializedProperty disableDataProvider;
	protected SerializedProperty disableConfigData;

	protected virtual void OnEnable()
	{
		disableMessageProcessor = serializedObject.FindProperty("disableMessageProcessor");
		disableNetworkMessageProcessor = serializedObject.FindProperty("disableNetworkMessageProcessor");
		disableDataProvider = serializedObject.FindProperty("disableDataProvider");
		disableConfigData = serializedObject.FindProperty("disableConfigData");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("禁用消息", GUILayout.Width(50f));
		disableMessageProcessor.boolValue = EditorGUILayout.Toggle(disableMessageProcessor.boolValue);
		EditorGUILayout.LabelField("禁用网络", GUILayout.Width(50f));
		disableNetworkMessageProcessor.boolValue = EditorGUILayout.Toggle(disableNetworkMessageProcessor.boolValue);
		EditorGUILayout.LabelField("禁用数据", GUILayout.Width(50f));
		disableDataProvider.boolValue = EditorGUILayout.Toggle(disableDataProvider.boolValue);
		EditorGUILayout.LabelField("禁用配置", GUILayout.Width(50f));
		disableConfigData.boolValue = EditorGUILayout.Toggle(disableConfigData.boolValue);
		EditorGUILayout.EndHorizontal();
		serializedObject.ApplyModifiedProperties();
		DrawDefaultInspector();
	}
}
#endif
