using System;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A sample behaviour for triggering Cutscenes.
    /// </summary>
    public class CutsceneTrigger : MonoBehaviour
    {
        public StartMethod StartMethod;
        public Cutscene Cutscene;
        public GameObject TriggerObject;
		public string TriggerObjectTag;
        public string SkipButtonName = "Jump";

		[NonSerialized]
		public Action onTriggeredCallback = null;

        private bool hasTriggered = false;

        /// <summary>
        /// When the trigger is loaded, optimize the Cutscene.
        /// </summary>
        void Awake()
        {
            if (Cutscene != null)
            {
                Cutscene.Optimize();
            }
        }

		private void OnEnable()
		{
			if(StartMethod == StartMethod.OnEnable && Cutscene != null)
			{
				hasTriggered = true;
				if (onTriggeredCallback != null)
					onTriggeredCallback();
				Cutscene.Play();
			}
		}

		private void OnDisable()
		{
			if (StartMethod == StartMethod.OnEnable)
			{
				hasTriggered = false;
				Cutscene.Stop();
			}
		}

		// When the scene starts trigger the Cutscene if necessary.
		void Start()
        {
            if (StartMethod == StartMethod.OnStart && Cutscene != null)
            {
                hasTriggered = true;
				if (onTriggeredCallback != null)
					onTriggeredCallback();
                Cutscene.Play();
            }
        }

        void Update()
        {
            if (SkipButtonName != null || SkipButtonName != string.Empty)
            {
                // Check if the user wants to skip.
                if (Input.GetButtonDown(SkipButtonName))
                {
                    if (Cutscene != null && Cutscene.State == CinemaDirector.Cutscene.CutsceneState.Playing)
                    {
                        Cutscene.Skip();
                    }
                }
            }
        }


        /// <summary>
        /// If Cutscene is setup to play on trigger, watch for the trigger event.
        /// </summary>
        /// <param name="other">The other collider.</param>
        void OnTriggerEnter(Collider other)
        {
            if (!hasTriggered && (other.gameObject == TriggerObject || other.CompareTag(TriggerObjectTag)))
            {
                hasTriggered = true;
				if (onTriggeredCallback != null)
					onTriggeredCallback();
				Cutscene.Play();
            }
        }
    }

    public enum StartMethod
    {
        OnStart,
        OnTrigger,
        None,
		OnEnable,
    }
}