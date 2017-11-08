using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoDestructParticleSystem))]
[CanEditMultipleObjects]
public class AutoDestructParticleSystem_Editor : Editor
{
	SerializedProperty useFixedDurationProp = null;
	SerializedProperty fixedDurationProp = null;
	SerializedProperty disableInsteadOfDestroyProp = null;
	SerializedProperty naturallyEndProp = null;

	void OnEnable()
	{
		useFixedDurationProp = serializedObject.FindProperty("useFixedDuration");
		fixedDurationProp = serializedObject.FindProperty("fixedDuration");
		disableInsteadOfDestroyProp = serializedObject.FindProperty("disableInsteadOfDestroy");
		naturallyEndProp = serializedObject.FindProperty("naturallyEnd");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(useFixedDurationProp, new GUIContent("Use fixed duration?"));
		if (!useFixedDurationProp.hasMultipleDifferentValues && useFixedDurationProp.boolValue)
			EditorGUILayout.Slider(fixedDurationProp, 0f, 10f, new GUIContent("Duration in second(s):"));
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.PropertyField(disableInsteadOfDestroyProp, new GUIContent("Use disable instead of destroy."));
		EditorGUILayout.PropertyField(naturallyEndProp, new GUIContent("Let particle system stop naturally."));

		serializedObject.ApplyModifiedProperties();
	}
}
