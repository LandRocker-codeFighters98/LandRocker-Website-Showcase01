using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LandRocker.TimelineTrack
{
    public class LightControlClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private LightControlBehaviour template = new LightControlBehaviour();
        public ClipCaps clipCaps => ClipCaps.Blending;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) => ScriptPlayable<LightControlBehaviour>.Create(graph, template);
    }
}