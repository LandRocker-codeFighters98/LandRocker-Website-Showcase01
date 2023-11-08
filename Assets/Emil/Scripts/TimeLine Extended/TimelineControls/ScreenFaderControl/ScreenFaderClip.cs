using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace LandRocker.TimelineTrack
{
    public class ScreenFaderClip : PlayableAsset, ITimelineClipAsset
    {
        public ScreenFaderBehaviour template = new ScreenFaderBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<ScreenFaderBehaviour>.Create(graph, template); 
        }
    }
}