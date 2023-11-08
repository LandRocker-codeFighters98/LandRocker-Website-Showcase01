using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LandRocker.TimelineTrack
{
    [TrackColor(154f / 255f, 8f / 255f, 65f / 255f)]
    [TrackBindingType(typeof(Animator))]
    [TrackClipType(typeof(AnimatorControlClip))]
    public class AnimatorControlTrack : TrackAsset
    {
       public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) => ScriptPlayable<AnimatorControlMixer>.Create(graph, inputCount);
    }
}