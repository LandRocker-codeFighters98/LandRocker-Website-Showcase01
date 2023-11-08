using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


namespace LandRocker.TimelineTrack
{
    public class ScreenFaderMixerBehaviour : PlayableBehaviour
    {
        Color defaultColor;
        Image imageBinding;
        bool processedFrame;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            imageBinding = playerData as Image;

            if (imageBinding == null)
                return;

            if (!processedFrame)
            {
                defaultColor = imageBinding.color;
                processedFrame = true;
            }

            int inputCount = playable.GetInputCount();

            Color blendedColor = Color.clear;
            float totalWeight = 0f;
            float greatestWeight = 0f;
            int currentInputs = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<ScreenFaderBehaviour> inputPlayable = (ScriptPlayable<ScreenFaderBehaviour>)playable.GetInput(i);
                ScreenFaderBehaviour input = inputPlayable.GetBehaviour();

                blendedColor += input.color * inputWeight;
                totalWeight += inputWeight;

                if (inputWeight > greatestWeight)
                {
                    greatestWeight = inputWeight;
                }

                if (!Mathf.Approximately(inputWeight, 0f))
                    currentInputs++;
            }

            imageBinding.color = blendedColor + defaultColor * (1f - totalWeight);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            processedFrame = false;

            if (imageBinding == null)
                return;

            imageBinding.color = defaultColor;
        }
    }
}