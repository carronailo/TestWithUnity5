using UnityEngine;
using System.Collections;
using CinemaDirector;

namespace ProjectARPG.PluginExtensions.CinemaSuite.ActorItem
{
	public enum EAttackActionType
	{
		None = 0x0,
	}

	[CutsceneItemAttribute("Combat", "Cast Skill", CutsceneItemGenre.ActorItem)]
	public class CastSkill : CinemaActorEvent
	{
		public EAttackActionType skillType = EAttackActionType.None;
		public SendMessageOptions SendMessageOptions = SendMessageOptions.DontRequireReceiver;

		/// <summary>
		/// Trigger this event and send the message.
		/// </summary>
		/// <param name="actor">the actor to send the message to.</param>
		public override void Trigger(GameObject actor)
		{
			if (actor != null)
			{
				actor.SendMessage("ForceSkill", skillType, SendMessageOptions);
			}
		}
	}
}
