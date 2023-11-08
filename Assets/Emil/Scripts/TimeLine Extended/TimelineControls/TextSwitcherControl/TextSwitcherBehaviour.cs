using UnityEngine;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    [System.Serializable]
    public class TextSwitcherBehaviour : PlayableBehaviour
    {
        public Color color = Color.white;
        public int fontSize = 14;
        public string text;
    }
}