using UnityEngine;
using UnityEditor;

public class CustomQueueMatInspector : MaterialEditor
{
	public override void OnEnable()
	{
		base.OnEnable();
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		Material material = target as Material;
		bool flag = false;
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("-100"))
		{
			material.renderQueue -= 100;
			flag = true;
		}
		if (GUILayout.Button("-10"))
		{
			material.renderQueue -= 10;
			flag = true;
		}
		if (GUILayout.Button("-1"))
		{
			material.renderQueue -= 1;
			flag = true;
		}
		string rdrQueueStr = material.renderQueue.ToString();
		rdrQueueStr = EditorGUILayout.TextField(rdrQueueStr);
		if (GUILayout.Button("+1"))
		{
			material.renderQueue += 1;
			flag = true;
		}
		if (GUILayout.Button("+10"))
		{
			material.renderQueue += 10;
			flag = true;
		}
		if (GUILayout.Button("+100"))
		{
			material.renderQueue += 100;
			flag = true;
		}
		EditorGUILayout.EndHorizontal();
		if (flag)
			EditorUtility.SetDirty(target);
	}
}
