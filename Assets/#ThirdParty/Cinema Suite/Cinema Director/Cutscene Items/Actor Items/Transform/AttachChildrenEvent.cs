using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Detaches all children in hierarchy from this Parent.
    /// </summary>
    [CutsceneItemAttribute("Transform", "Attach Children", CutsceneItemGenre.ActorItem)]
    public class AttachChildrenEvent : CinemaActorEvent
    {
        public GameObject[] Children;
		public string[] ChildrenTag;
        public override void Trigger(GameObject actor)
        {
            if (actor != null && Children != null)
            {
				for(int i = 0; i < Children.Length; ++i)
				{
					GameObject child = Children[i];
					if(child == null && ChildrenTag != null && ChildrenTag.Length > i)
						child = GameObject.FindGameObjectWithTag(ChildrenTag[i]);
					if (child != null)
						child.transform.SetParent(actor.transform);
				}
            }
        }

        public override void Reverse(GameObject actor)
        {
        }
    }
}