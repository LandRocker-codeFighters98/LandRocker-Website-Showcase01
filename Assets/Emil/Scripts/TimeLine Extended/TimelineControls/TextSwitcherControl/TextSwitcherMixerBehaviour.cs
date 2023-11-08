using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace LandRocker.TimelineTrack
{
    public class TextSwitcherMixerBehaviour : PlayableBehaviour
    {
        Color defaultColor;
        int defaultFontSize;
        string defaultText;

        Text trackBinding;
        bool processedFrame;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (trackBinding is Text)
                trackBinding = playerData as Text;

            if (!trackBinding)
                return;

            if (!processedFrame)
            {
                defaultColor = trackBinding.color;
                defaultFontSize = trackBinding.fontSize;
                defaultText = trackBinding.text;
                processedFrame = true;
            }

            int inputCount = playable.GetInputCount();

            Color blendedColor = Color.clear;
            float blendedFontSize = 0f;
            float totalWeight = 0f;
            float greatestWeight = 0f;
            int currentInputs = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<TextSwitcherBehaviour> inputPlayable = (ScriptPlayable<TextSwitcherBehaviour>)playable.GetInput(i);
                TextSwitcherBehaviour input = inputPlayable.GetBehaviour();

                blendedColor += input.color * inputWeight;
                blendedFontSize += input.fontSize * inputWeight;
                totalWeight += inputWeight;

                if (inputWeight > greatestWeight)
                {
                    trackBinding.text = input.text;
                    greatestWeight = inputWeight;
                }

                if (!Mathf.Approximately(inputWeight, 0f))
                    currentInputs++;
            }

            trackBinding.color = blendedColor + defaultColor * (1f - totalWeight);
            trackBinding.fontSize = Mathf.RoundToInt(blendedFontSize + defaultFontSize * (1f - totalWeight));
            if (currentInputs != 1 && 1f - totalWeight > greatestWeight)
            {
                trackBinding.text = defaultText;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            processedFrame = false;

            if (trackBinding == null)
                return;

            trackBinding.color = defaultColor;
            trackBinding.fontSize = defaultFontSize;
            trackBinding.text = defaultText;
        }
    }
}