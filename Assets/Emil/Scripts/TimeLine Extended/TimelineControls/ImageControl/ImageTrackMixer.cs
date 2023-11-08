using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.UI;

namespace LandRocker.TimelineTrack
{
    public class ImageTrackMixer : PlayableBehaviour
    {
        private Image image;
        private bool processedFrame;
        private float defaultAlpha;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerData is Image)
                image = playerData as Image;

            if (!image)
                return;

            if (!processedFrame)
            {
                processedFrame = true;
                defaultAlpha = image.color.a;
            }

            int inputCount = playable.GetInputCount();
            float currentAlpha = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);

                if (inputWeight > 0)
                {
                    ScriptPlayable<ImageControlBehaviour> inputPlayable = (ScriptPlayable<ImageControlBehaviour>)playable.GetInput(i);
                    ImageControlBehaviour behaviour = inputPlayable.GetBehaviour();
                    
                    currentAlpha = inputWeight;

                    if(behaviour.isUsingBehaviourAlpha && !(inputWeight < 1))
                    {
                        currentAlpha = behaviour.alpha;
                    }
                }

                image.color = new Color(image.color.r, image.color.g, image.color.b, currentAlpha);
            }
        }
        public override void OnPlayableDestroy(Playable playable)
        {
            processedFrame = false;

            if (!image) return;

            image.color = new Color(image.color.r, image.color.g, image.color.b, defaultAlpha);
        }
    }
}