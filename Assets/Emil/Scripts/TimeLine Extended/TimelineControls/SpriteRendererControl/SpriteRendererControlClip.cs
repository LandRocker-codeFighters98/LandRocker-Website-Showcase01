using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LandRocker.TimelineTrack
{
    public class SpriteRendererControlClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private SpriteRendererControlBehaviour template = new SpriteRendererControlBehaviour();
        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) => ScriptPlayable<SpriteRendererControlBehaviour>.Create(graph,template);
    }
}