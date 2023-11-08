using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    public class AnimatorControlClip : PlayableAsset
    {
        [SerializeField] private AnimatorControlBehaviour animatorControlBehaviour = new AnimatorControlBehaviour();
        public ClipCaps clipCaps => ClipCaps.None;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<AnimatorControlBehaviour>.Create(graph, animatorControlBehaviour);
        }
    }
}