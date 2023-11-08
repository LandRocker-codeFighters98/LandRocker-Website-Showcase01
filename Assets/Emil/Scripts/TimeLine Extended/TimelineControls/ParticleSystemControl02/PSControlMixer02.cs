using UnityEngine;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    public class PSControlMixer02 : PlayableBehaviour
    {
        private ParticleSystem particleSystem;
        private Color defaultColor;
        private bool processedFrame;
        private bool playOnAwake;
        private bool valueSet;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerData is ParticleSystem)
                particleSystem = playerData as ParticleSystem;

            if (!particleSystem)
                return;

             ParticleSystem.MainModule main = particleSystem.main;

            if (!processedFrame)
            {
                processedFrame = true;

                defaultColor = main.startColor.color;
                playOnAwake = main.playOnAwake;

                if (!playOnAwake)
                    particleSystem.Stop();
            }

            int inputCount = playable.GetInputCount();
            float sumWeight = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                if (inputWeight > 0)
                {
                    sumWeight += inputWeight;
                    ScriptPlayable<PSControlBehaviour02> inputPlayable = (ScriptPlayable<PSControlBehaviour02>)playable.GetInput(i);
                    PSControlBehaviour02 behaviour = inputPlayable.GetBehaviour();

                    if (!valueSet)
                    {
                        valueSet = true;
                        particleSystem.Play();
                    }

                    Color colorWithoutWeight = behaviour.color;
                    Color colorWithWeight = new Color(colorWithoutWeight.r, colorWithoutWeight.g, colorWithoutWeight.b, inputWeight);
                    main.startColor = colorWithWeight;
                }
            }

            if (!(sumWeight > 0))
            {
                sumWeight = 0;

                if (processedFrame && valueSet)
                {
                    main.startColor = defaultColor;
                    main.playOnAwake = playOnAwake;

                    processedFrame = false;
                    valueSet = false;

                    if (!playOnAwake)
                        particleSystem.Stop();
                }
            }
        }
    }
}