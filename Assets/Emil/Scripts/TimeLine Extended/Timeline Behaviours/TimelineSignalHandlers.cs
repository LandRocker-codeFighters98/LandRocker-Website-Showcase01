using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace LandRocker.Timeline
{
    public class TimelineSignalHandlers : MonoBehaviour
    {
        public void EnablePlayableDirector(PlayableDirector playable)
        {
            if (!playable.enabled)
                playable.enabled = true;
        }

        public void DisablePlayableDirector(PlayableDirector playable)
        {
            if (playable.enabled)
                playable.enabled = false;
        }

        public void PlayPlayableDirector(PlayableDirector playable)
        {
            playable.Play();
        }

        public void StopPlayableDirector(PlayableDirector playable)
        {
            playable.Stop();
        }
        public void PausePlayableDirector(PlayableDirector playable)
        {
            playable.Pause();
        }
    }
}
