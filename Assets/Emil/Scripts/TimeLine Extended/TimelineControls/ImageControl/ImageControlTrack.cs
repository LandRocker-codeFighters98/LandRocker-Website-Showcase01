using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace LandRocker.TimelineTrack
{
    [TrackColor(255f / 255f, 126f / 255f, 0f / 255f)]
    [TrackBindingType(typeof(Image))]
    [TrackClipType(typeof(ImageControlClip))]
    public class ImageControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) => ScriptPlayable<ImageTrackMixer>.Create(graph,inputCount);

         public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            Image trackBinding = director.GetGenericBinding(this) as Image;
            if (trackBinding == null)
                return;

            var serializedObject = new UnityEditor.SerializedObject(trackBinding);
            var iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                if (iterator.hasVisibleChildren)
                    continue;

                driver.AddFromName<Image>(trackBinding.gameObject, iterator.propertyPath);
            }
#endif
            base.GatherProperties(director, driver);
        }
    }
}