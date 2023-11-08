using UnityEngine.Timeline;
using TMPro;
using UnityEngine.Playables;
using UnityEngine;

namespace LandRocker.TimelineTrack
{
    [TrackBindingType(typeof(TextMeshProUGUI))]
    [TrackClipType(typeof(TMPControlClip))]
    [TrackColor(0f / 255f, 162f / 255f, 255f / 255f)]
    public class TMPControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) => ScriptPlayable<TMPControlMixer>.Create(graph, inputCount);
    }
}