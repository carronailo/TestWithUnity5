using CinemaDirector;
using UnityEngine;

[CutsceneTrackGroupAttribute(typeof(CharacterTrackGroup))]
public class CharacterTrackGroupControl : ActorTrackGroupControl
{
    public override void Initialize()
    {
        base.Initialize();
        LabelPrefix = Resources.Load<Texture>("Character TrackGroup Icon");
    }
}