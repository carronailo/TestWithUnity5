using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// The ActorTrackGroup maintains an Actor and a set of tracks that affect 
    /// that actor over the course of the Cutscene.
    /// </summary>
    [TrackGroupAttribute("Actor Track Group", TimelineTrackGenre.ActorTrack)]
    public class ActorTrackGroup : TrackGroup
    {
		[SerializeField]
		private string actorTag;

        [SerializeField]
        private Transform actor;

        /// <summary>
        /// The Actor that this TrackGroup is focused on.
        /// </summary>
        public Transform Actor
        {
            get 
			{
				if (actor == null)
				{
					GameObject actorGO = GameObject.FindWithTag(actorTag);
					actor = actorGO != null ? actorGO.transform : null;
					//actor = CutsceneActorManager.Instance.GetCutsceneActorReferenceByGUID<Transform>(actorGUID);
					//GameObject[] actorGOs = GameObject.FindGameObjectsWithTag(actorTag);
					//if (actorGOs.Length == 1)
					//	actor = actorGOs[0].transform;
					//else if (actorGOs.Length > 1)
					//{
					//	foreach (GameObject actorGO in actorGOs)
					//	{
					//		CutsceneActor actorComp = actorGO.GetComponent<CutsceneActor>();
					//		if (actorComp == null)
					//			continue;
					//		else if (actorComp.CompareGUID(actorGUID))
					//		{
					//			actor = actorGO.transform;
					//			break;
					//		}
					//	}
					//}
				}
				return actor; 
			}
			set 
			{
				actor = value; 
				//CutsceneActor actorComp = actor.GetComponent<CutsceneActor>();
				//if(actorComp == null)
				//	actorComp = actor.gameObject.AddComponent<CutsceneActor>();
				//actorGUID = actorComp.ActorGUID;
				// 设置tag要放在获取GUID之后，因为如果当前Actor没有tag的话，在添加CutSceneActor脚本的时候会自动设置一个tag
				actorTag = actor.tag;
			}
        }
    }
}