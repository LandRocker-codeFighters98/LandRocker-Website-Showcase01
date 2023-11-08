using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    [TrackColor(127f / 255f, 185f / 255f, 170f / 255f)]
    [TrackBindingType(typeof(CanvasGroup))]
    [TrackClipType(typeof(CanvasGroupControlClip))]
    public class CanvasGroupControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) => ScriptPlayable<CanvasGroupTrackMixer>.Create(graph,inputCount);
    }
}