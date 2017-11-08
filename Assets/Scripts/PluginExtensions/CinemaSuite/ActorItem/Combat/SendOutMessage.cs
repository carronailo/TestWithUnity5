using UnityEngine;
using System.Collections;
using CinemaDirector;

namespace ProjectARPG.PluginExtensions.CinemaSuite.ActorItem
{
	public enum ESendMessageType
	{
		None = 0,
		ChangeWingsTemporarylyDuringBattleMessage,
		ChangeArtifactTemporarylyDuringBattleMessage,
		Messages_TriggerGamePlotMessage,
		SkillCaromCombatMessage,
	}

	[CutsceneItemAttribute("Combat", "Send Out Message", CutsceneItemGenre.ActorItem)]
	public class SendOutMessage : CinemaActorEvent
	{
		public ESendMessageType sendMessageType;
		//public ChangeWingsTemporarylyDuringBattleMessage changeWingsTemporarylyDuringBattleMessage;
		//public ChangeArtifactTemporarylyDuringBattleMessage changeArtifactTemporarylyDuringBattleMessage;
		//public Messages_TriggerGamePlotMessage messages_TriggerGamePlotMessage;
		//public SkillCaromCombatMessage skillCaromCombatMessage;

		/// <summary>
		/// Trigger this event and send the message.
		/// </summary>
		/// <param name="actor">the actor to send the message to.</param>
		public override void Trigger(GameObject actor)
		{
			switch (sendMessageType)
			{
				case ESendMessageType.None:
					break;
				//case ESendMessageType.ChangeArtifactTemporarylyDuringBattleMessage:
				//	Message.Send(changeArtifactTemporarylyDuringBattleMessage).Broadcast();
				//	break;
				//case ESendMessageType.ChangeWingsTemporarylyDuringBattleMessage:
				//	Message.Send(changeWingsTemporarylyDuringBattleMessage).Broadcast();
				//	break;
				//case ESendMessageType.Messages_TriggerGamePlotMessage:
				//	Message.Send(messages_TriggerGamePlotMessage).Broadcast();
				//	break;
				//case ESendMessageType.SkillCaromCombatMessage:
				//	Message.Send(skillCaromCombatMessage).Broadcast();
				//	break;
			}
		}
	}
}
