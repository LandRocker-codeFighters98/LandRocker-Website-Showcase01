using UnityEngine;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    [System.Serializable]
    public class ImageControlBehaviour : PlayableBehaviour
    {
        [Range(0f, 1f)]public float alpha = 1;
        public bool isUsingBehaviourAlpha = true;
    }
}