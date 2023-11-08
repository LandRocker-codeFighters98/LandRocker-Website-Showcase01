using UnityEngine;
using UnityEngine.Timeline;

namespace LandRocker.TimelineTrack
{
    [TrackColor(127f / 255f, 185f / 255f, 244f / 255f)]
    [TrackBindingType(typeof(ParticleSystem))]
    [TrackClipType(typeof(PSControlClip01))]
    public class PSControlTrack01 : TrackAsset
    {
       // public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) => ScriptPlayable<PSControlMixer>.Create(graph, inputCount);
    }
    
}
