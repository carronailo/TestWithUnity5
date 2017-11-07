using UnityEngine;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Transform", "Set Parent", CutsceneItemGenre.ActorItem)]
    public class SetParent : CinemaActorEvent
    {
		[SerializeField]
		private string parentTag;

		public GameObject parent;

		public override void Trigger(GameObject actor)
        {
			if (parent == null)
			{
				parent = GameObject.FindWithTag(parentTag);
			}
			if (actor != null && parent != null)
			{
				actor.transform.SetParent(parent.transform);
			}
			else if(actor != null)
			{
				actor.transform.SetParent(null);
			}
		}

		public override void Reverse(GameObject actor)
        {
        }
    }
}