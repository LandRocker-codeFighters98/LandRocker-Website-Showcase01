using UnityEngine.Playables;
using UnityEngine;

namespace LandRocker.TimelineTrack
{
    public class LightControlMixer : PlayableBehaviour
    {
        private Light light;
        private Color defaultColor;
        private float defaultIntensity;
        private float defaultBounceIntensity;
        private float defaultRange;
        private bool processedFrame;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerData is Light)
                light = playerData as Light;

            if (!light)
                return;

            if (!processedFrame)
            {
                processedFrame = true;

                defaultIntensity = light.intensity;
                defaultBounceIntensity = light.bounceIntensity;
                defaultColor = light.color;
                defaultRange = light.range;
            }

            int inputCount = playable.GetInputCount();

            Color blendedColor = Color.clear;
            float blendedIntensity = 0;
            float blendedBounceIntensity = 0;
            float blendedRange = 0;
            float totalWeight = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<LightControlBehaviour> inputPlayable = (ScriptPlayable<LightControlBehaviour>)playable.GetInput(i);
                LightControlBehaviour behaviour = inputPlayable.GetBehaviour();

                blendedColor += behaviour.color * inputWeight;
                blendedIntensity += behaviour.intensity * inputWeight;
                blendedBounceIntensity += behaviour.bounceIntensity * inputWeight;
                blendedRange += behaviour.range * inputWeight;
                totalWeight += inputWeight;
            }

            float remainingWeight = 1 - totalWeight;
            light.color = blendedColor + defaultColor * remainingWeight;
            light.intensity = blendedIntensity + defaultIntensity * remainingWeight;
            light.bounceIntensity = blendedBounceIntensity + defaultBounceIntensity * remainingWeight;
            light.range = blendedRange + defaultRange * remainingWeight;

        }

        public override void OnPlayableDestroy(Playable playable)
        {
            processedFrame = false;

            if (!light) return;

            light.intensity = defaultIntensity;
            light.bounceIntensity = defaultBounceIntensity;
            light.color = defaultColor;
            light.range = defaultRange;
        }
    }
}