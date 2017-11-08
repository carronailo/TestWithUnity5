using UnityEngine;
using System.Collections;
using CinemaDirector;

namespace ProjectARPG.PluginExtensions.CinemaSuite.ActorItem
{
	[CutsceneItemAttribute("Combat", "Cast Attack", CutsceneItemGenre.ActorItem)]
	public class CastAttack : CinemaActorEvent
	{
		public SendMessageOptions SendMessageOptions = SendMessageOptions.DontRequireReceiver;

		/// <summary>
		/// Trigger this event and send the message.
		/// </summary>
		/// <param name="actor">the actor to send the message to.</param>
		public override void Trigger(GameObject actor)
		{
			if (actor != null)
			{
				actor.SendMessage("ForceAttack", SendMessageOptions);
			}
		}
	}
}
