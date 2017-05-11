using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(StateMachineTemplate))]
public class StateMachineTemplateInspector : CustomInspector
{
	SerializedProperty allStates;

	StateMachineTemplate target;

	public override void OnEnable()
	{
		base.OnEnable();
		allStates = serializedObject.FindProperty("allStates");
		target = serializedObject.targetObject as StateMachineTemplate;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		DisplayScriptField();

		if(GUILayout.Button("ceshi"))
		{
			LogConsole.Log(allStates.arraySize);
			for(int i = 0; i < allStates.arraySize; ++i)
			{
				SerializedProperty elem = allStates.GetArrayElementAtIndex(i);
				LogConsole.Log(elem.name + " " + elem.type);
				while (elem.Next(true))
				{
					LogConsole.Log(elem.name + " " + elem.type);
				}
			}
		}

		EditorGUILayout.LabelField("allStates");
		int count = allStates.arraySize;
		for (int i = 0; i < count; ++i)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10f);
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("element " + i);
			SerializedProperty elem = allStates.GetArrayElementAtIndex(i);
			if (target.allStates[i] is BaseState)
			{
				BaseState state = target.allStates[i] as BaseState;
				state.s = EditorGUILayout.TextField("s = ", state.s);
			}
			if (target.allStates[i] is AdvancedState)
			{
				AdvancedState state = target.allStates[i] as AdvancedState;
				state.i = EditorGUILayout.IntField("i = ", state.i);
			}
			if (target.allStates[i] is AnotherState)
			{
				AnotherState state = target.allStates[i] as AnotherState;
				state.f = EditorGUILayout.FloatField("f = ", state.f);
			}
			if (target.allStates[i] is AnotherAdvancedState)
			{
				AnotherAdvancedState state = target.allStates[i] as AnotherAdvancedState;
				state.b = EditorGUILayout.Toggle("b = ", state.b);
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		serializedObject.ApplyModifiedProperties();
	}
}
