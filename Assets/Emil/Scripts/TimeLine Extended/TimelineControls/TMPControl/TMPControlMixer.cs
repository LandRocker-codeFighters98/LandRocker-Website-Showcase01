using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace LandRocker.TimelineTrack
{
    public class TMPControlMixer : PlayableBehaviour
    {
        private TextMeshProUGUI TMPro;
        private bool processedFrame;
        private Color defaultColor;
        private string defaultText;
        private float defaultLineSpacing = 0;
        private float defaultParagraphSpacing = 0;
        private float defaultWordSpacing = 0;
        private float defaultCharacterSpacing = 0;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerData is TextMeshProUGUI)
                TMPro = playerData as TextMeshProUGUI;

            if (!TMPro)
                return;

            if (!processedFrame)
            {
                processedFrame = true;

                defaultColor = TMPro.color;
                defaultText = TMPro.text;
                defaultCharacterSpacing = TMPro.characterSpacing;
                defaultParagraphSpacing = TMPro.paragraphSpacing;
                defaultLineSpacing = TMPro.lineSpacing;
                defaultWordSpacing = TMPro.wordSpacing;
            }

            string currentText = string.Empty;
            float currentAlpha = 1;

            int inputCount = playable.GetInputCount();

            Color blendedColor = Color.clear;
            float blendedCharacterSpacing = 0.0f;
            float blendedLineSpacing = 0.0f;
            float blendedParagraphSpacing = 0.0f;
            float blendedWordSpacing = 0.0f;
            float totalWeight = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<TMPBehaviour> inputPlayable = (ScriptPlayable<TMPBehaviour>)playable.GetInput(i);
                TMPBehaviour inputBehaviour = inputPlayable.GetBehaviour();

                if (inputWeight > 0)
                {
                    if (inputWeight < 0.5)
                    {
                        currentAlpha = 1 - inputWeight;
                    }
                    else
                    {
                        currentText = inputBehaviour.text;
                    }
                }


                blendedColor += inputBehaviour.color * inputWeight;

                if (inputBehaviour.isModifyingCharacterSpacing)
                {
                    blendedCharacterSpacing += inputBehaviour.characterSpacing * inputWeight;
                    blendedLineSpacing = inputBehaviour.lineSpacing * inputWeight;
                    blendedParagraphSpacing = inputBehaviour.paragraphSpacing * inputWeight;
                    blendedWordSpacing = inputBehaviour.wordSpacing * inputWeight;
                }

                totalWeight += inputWeight;
            }

            float remainingWeight = 1 - totalWeight;
            TMPro.text = currentText;
            TMPro.lineSpacing = blendedLineSpacing + defaultLineSpacing * remainingWeight;
            TMPro.characterSpacing = blendedCharacterSpacing + defaultCharacterSpacing * remainingWeight;
            TMPro.paragraphSpacing = blendedParagraphSpacing + defaultParagraphSpacing * remainingWeight;
            TMPro.wordSpacing = blendedWordSpacing + defaultWordSpacing * remainingWeight;

            Color color = blendedColor + defaultColor * remainingWeight;
            TMPro.color = new Color(color.r, color.g, color.b, currentAlpha);
        }
    }
}