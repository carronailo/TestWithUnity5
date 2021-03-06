using CinemaDirector.Helpers;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An Event for disabling any behaviour that has an "enabled" property.
    /// </summary>
    [CutsceneItemAttribute("Game Object", "Disable Behaviour", CutsceneItemGenre.ActorItem)]
	public class DisableBehaviour : CinemaActorEvent, IRevertable
    {
        public Component Behaviour;

		[SerializeField]
		private string BehaviourTypeName;

        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

		/// <summary>
		/// Cache the state of all actors related to this event.
		/// </summary>
		/// <returns>All the revert info related to this event.</returns>
		public RevertInfo[] CacheState()
		{
			List<Transform> actors = new List<Transform>(GetActors());
			List<RevertInfo> reverts = new List<RevertInfo>();
			foreach (Transform go in actors)
			{
				if (go != null)
				{
					Component b = Behaviour == null ? go.GetComponent(BehaviourTypeName) as Component : go.GetComponent(Behaviour.GetType()) as Component;
					if (b != null)
					{
						if (Behaviour == null)
							Behaviour = b;

						PropertyInfo pi = Behaviour.GetType().GetProperty("enabled");
						bool value = (bool)pi.GetValue(b, null);

						reverts.Add(new RevertInfo(this, Behaviour, "enabled", value));
					}
				}
			}

			return reverts.ToArray();
		}

		/// <summary>
        /// Trigger this event and Disable the chosen Behaviour.
        /// </summary>
        /// <param name="actor">The actor to perform the behaviour disable on.</param>
        public override void Trigger(GameObject actor)
        {
			Component b = Behaviour == null ? actor.GetComponent(BehaviourTypeName) as Component : actor.GetComponent(Behaviour.GetType()) as Component;
            if (b != null)
            {
				if (Behaviour == null)
					Behaviour = b;

				Behaviour.GetType().GetMember("enabled");

                PropertyInfo fieldInfo = Behaviour.GetType().GetProperty("enabled");
                fieldInfo.SetValue(Behaviour, false, null);
            }
        }

        /// <summary>
        /// Reverse trigger this event and Enable the chosen Behaviour.
        /// </summary>
        /// <param name="actor">The actor to perform the behaviour enable on.</param>
        public override void Reverse(GameObject actor)
        {
			Component b = Behaviour == null ? actor.GetComponent(BehaviourTypeName) as Component : actor.GetComponent(Behaviour.GetType()) as Component;
			if (b != null)
            {
				if (Behaviour == null)
					Behaviour = b;

				PropertyInfo fieldInfo = Behaviour.GetType().GetProperty("enabled");
                fieldInfo.SetValue(b, true, null);
            }
        }

        /// <summary>
        /// Option for choosing when this Event will Revert to initial state in Editor.
        /// </summary>
        public RevertMode EditorRevertMode
        {
            get { return editorRevertMode; }
            set { editorRevertMode = value; }
        }

        /// <summary>
        /// Option for choosing when this Event will Revert to initial state in Runtime.
        /// </summary>
        public RevertMode RuntimeRevertMode
        {
            get { return runtimeRevertMode; }
            set { runtimeRevertMode = value; }
        }
    }
}