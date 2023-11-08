using UnityEngine;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    [System.Serializable]
    public class AnimatorControlBehaviour : PlayableBehaviour
    {
        #region [Animation]
        public int layerID = 0; 
        public string animationName = "Animation State Name" ;
        #endregion
    }
}