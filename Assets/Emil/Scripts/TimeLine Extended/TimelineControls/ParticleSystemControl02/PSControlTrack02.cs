using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LandRocker.TimelineTrack
{
    [TrackColor(127f / 255f, 185f / 255f, 255f / 255f)]
    [TrackBindingType(typeof(ParticleSystem))]
    [TrackClipType(typeof(PSControlClip02))]
    public class PSControlTrack02 : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) => ScriptPlayable<PSControlMixer02>.Create(graph, inputCount);
    }
}