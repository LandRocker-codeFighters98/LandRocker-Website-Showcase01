using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LandRocker.TimelineTrack
{
    public class PSControlClip01 : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private PSControlBehaviour01 psControlBehavior = new PSControlBehaviour01();
        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<PSControlBehaviour01>.Create(graph, psControlBehavior);
        }
    }
}