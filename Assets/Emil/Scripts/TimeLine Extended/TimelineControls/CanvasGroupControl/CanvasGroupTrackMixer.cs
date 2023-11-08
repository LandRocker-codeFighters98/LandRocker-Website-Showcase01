using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.UI;

namespace LandRocker.TimelineTrack
{
    public class CanvasGroupTrackMixer : PlayableBehaviour
    {
        private CanvasGroup canvasGroup;
        private bool processedFrame;
        private float defaultAlpha;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerData is CanvasGroup)
                canvasGroup = playerData as CanvasGroup;

            if (!canvasGroup)
                return;

            if (!processedFrame)
            {
                processedFrame = true;
                defaultAlpha = canvasGroup.alpha;
            }

            int inputCount = playable.GetInputCount();
            float currentAlpha = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);

                if (inputWeight > 0)
                {
                    ScriptPlayable<CanvasGroupControlBehaviour> inputPlayable = (ScriptPlayable<CanvasGroupControlBehaviour>)playable.GetInput(i);
                    CanvasGroupControlBehaviour behaviour = inputPlayable.GetBehaviour();
                    
                    currentAlpha = inputWeight;

                    if(behaviour.isUsingBehaviourAlpha && !(inputWeight < 1))
                    {
                        currentAlpha = behaviour.alpha;
                    }
                }

                canvasGroup.alpha = currentAlpha;
            }
        }
        public override void OnPlayableDestroy(Playable playable)
        {
            processedFrame = false;

            if (!canvasGroup) return;

            canvasGroup.alpha = defaultAlpha;
        }
    }
}