using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LandRocker.TimelineTrack
{
    public class CanvasGroupControlClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private CanvasGroupControlBehaviour template = new CanvasGroupControlBehaviour();
        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) => ScriptPlayable<CanvasGroupControlBehaviour>.Create(graph,template);
    }
}