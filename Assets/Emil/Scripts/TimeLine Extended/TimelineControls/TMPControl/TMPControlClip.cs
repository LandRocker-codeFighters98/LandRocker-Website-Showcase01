using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    public class TMPControlClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private TMPBehaviour template = new TMPBehaviour();
        public ClipCaps clipCaps => ClipCaps.Blending;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) => ScriptPlayable<TMPBehaviour>.Create(graph,template);
    }
}
