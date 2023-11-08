using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LandRocker.TimelineTrack
{
    [TrackColor(241f / 255f, 249f / 255f, 99f / 255f)]
    [TrackClipType(typeof(LightControlClip))]
    [TrackBindingType(typeof(Light))]
    public class LightControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) => ScriptPlayable<LightControlMixer>.Create(graph, inputCount);

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            Light trackBinding = director.GetGenericBinding(this) as Light;
            if (trackBinding == null)
                return;
            driver.AddFromName<Light>(trackBinding.gameObject, "m_Color");
            driver.AddFromName<Light>(trackBinding.gameObject, "m_Intensity");
            driver.AddFromName<Light>(trackBinding.gameObject, "m_Range");
            driver.AddFromName<Light>(trackBinding.gameObject, "m_BounceIntensity");
#endif
            base.GatherProperties(director, driver);
        }
    }
}