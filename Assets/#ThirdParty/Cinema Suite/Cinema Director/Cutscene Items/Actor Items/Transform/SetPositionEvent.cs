using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Detaches all children in hierarchy from this Parent.
    /// </summary>
    [CutsceneItemAttribute("Transform", "Set Position", CutsceneItemGenre.ActorItem)]
    public class SetPositionEvent : CinemaActorEvent
    {
		public Space space;
        public Vector3 Position;

        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
				if(space == Space.World)
					actor.transform.position = Position;
				else if(space == Space.Self)
					actor.transform.localPosition = Position;
			}
		}

        public override void Reverse(GameObject actor)
        {
        }
    }
}