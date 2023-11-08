using System;
using UnityEngine;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    [Serializable]
    public class NavMeshAgentControlBehaviour : PlayableBehaviour
    {
        public Transform destination;
        public bool destinationSet;

        public override void OnPlayableCreate(Playable playable)
        {
            destinationSet = false;
        }
    }
}