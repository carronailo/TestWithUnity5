using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ProjectARPG.PluginExtensions.CinemaSuite.ActorItem
{
	[CustomEditor(typeof(SendOutMessage))]
	public class SendOutMessageEditor : Editor
	{
		SerializedProperty firetimeProp;
		SerializedProperty sendMessageTypeProp;
		SerializedProperty changeWingsTemporarylyDuringBattleMessageProp;
		SerializedProperty changeArtifactTemporarylyDuringBattleMessageProp;
		SerializedProperty messages_TriggerGamePlotMessageProp;
        SerializedProperty skillCaromCombatMessage;

		void OnEnable()
		{
			firetimeProp = serializedObject.FindProperty("firetime");
			sendMessageTypeProp = serializedObject.FindProperty("sendMessageType");
			changeWingsTemporarylyDuringBattleMessageProp = serializedObject.FindProperty("changeWingsTemporarylyDuringBattleMessage");
			changeArtifactTemporarylyDuringBattleMessageProp = serializedObject.FindProperty("changeArtifactTemporarylyDuringBattleMessage");
			messages_TriggerGamePlotMessageProp = serializedObject.FindProperty("messages_TriggerGamePlotMessage");

            skillCaromCombatMessage = serializedObject.FindProperty("skillCaromCombatMessage");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(firetimeProp);
			EditorGUILayout.PropertyField(sendMessageTypeProp, new GUIContent("发送消息："));
			ESendMessageType currentType = (ESendMessageType)sendMessageTypeProp.enumValueIndex;
			switch (currentType)
			{
				case ESendMessageType.None:
					break;
				case ESendMessageType.ChangeArtifactTemporarylyDuringBattleMessage:
					EditorGUILayout.PropertyField(changeArtifactTemporarylyDuringBattleMessageProp, new GUIContent("消息参数："), true);
					break;
				case ESendMessageType.ChangeWingsTemporarylyDuringBattleMessage:
					EditorGUILayout.PropertyField(changeWingsTemporarylyDuringBattleMessageProp, new GUIContent("消息参数："), true);
					break;
				case ESendMessageType.Messages_TriggerGamePlotMessage:
					EditorGUILayout.PropertyField(messages_TriggerGamePlotMessageProp, new GUIContent("消息参数："), true);
					break;
                case ESendMessageType.SkillCaromCombatMessage:
                    EditorGUILayout.PropertyField(skillCaromCombatMessage, new GUIContent("消息参数："), true);
                    break;
			}

			serializedObject.ApplyModifiedProperties();
		}

	}
}
