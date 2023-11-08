using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LandRocker.TimelineTrack
{
    public class ImageControlClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private ImageControlBehaviour template = new ImageControlBehaviour();
        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) => ScriptPlayable<ImageControlBehaviour>.Create(graph,template);
    }
}