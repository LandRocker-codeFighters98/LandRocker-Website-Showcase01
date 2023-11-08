using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace LandRocker.TimelineTrack
{
    public class TimeDilationClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private TimeDilationBehaviour template = new TimeDilationBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Extrapolation | ClipCaps.Blending; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) =>
             ScriptPlayable<TimeDilationBehaviour>.Create(graph, template);
        
    }
}