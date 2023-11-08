using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine;

namespace LandRocker.TimelineTrack
{
    public class PSControlClip02 : PlayableAsset 
    {
        [SerializeField] private PSControlBehaviour02 psControlBehavior = new PSControlBehaviour02();
        public ClipCaps clipCaps => ClipCaps.Blending;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<PSControlBehaviour02>.Create(graph, psControlBehavior);
        }
    }
}