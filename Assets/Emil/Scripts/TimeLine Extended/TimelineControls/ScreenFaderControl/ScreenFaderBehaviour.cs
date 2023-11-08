using System;
using UnityEngine;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    [Serializable]
    public class ScreenFaderBehaviour : PlayableBehaviour
    {
        public Color color = Color.black;
    }
}