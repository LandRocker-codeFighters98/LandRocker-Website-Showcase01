using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;


namespace LandRocker.TimelineTrack
{
    [TrackColor(150f / 255f, 43f / 255f, 145f / 255f)]
    [TrackBindingType(typeof(SpriteRenderer))]
    [TrackClipType(typeof(SpriteRendererControlClip))]
    public class SpriteRendererControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) => ScriptPlayable<SpriteRendererTrackMixer>.Create(graph,inputCount);
    }
}