using UnityEngine;
using System.Collections;
using CinemaDirector;

namespace ProjectARPG.PluginExtensions.CinemaSuite.ActorItem
{
	[CutsceneItemAttribute("Transform", "Set Rotation", CutsceneItemGenre.ActorItem)]
	public class SetRotationEvent : CinemaActorEvent
	{
		public Space space;
		public Vector3 Rotation;

		public override void Trigger(GameObject actor)
		{
			if (actor != null)
			{
				if (space == Space.World)
					actor.transform.eulerAngles = Rotation;
				else if (space == Space.Self)
					actor.transform.localEulerAngles = Rotation;
			}
		}

		public override void Reverse(GameObject actor)
		{
		}
	}
}
