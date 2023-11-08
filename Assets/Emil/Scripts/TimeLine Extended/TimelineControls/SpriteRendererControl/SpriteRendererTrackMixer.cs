using UnityEngine.Playables;
using UnityEngine;

namespace LandRocker.TimelineTrack
{
    public class SpriteRendererTrackMixer : PlayableBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private bool processedFrame;
        private float defaultAlpha;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerData is SpriteRenderer)
                spriteRenderer = playerData as SpriteRenderer;

            if (!spriteRenderer)
                return;

            if (!processedFrame)
            {
                processedFrame = true;
                defaultAlpha = spriteRenderer.color.a;
            }

            int inputCount = playable.GetInputCount();
            float currentAlpha = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);

                if (inputWeight > 0)
                {
                    ScriptPlayable<SpriteRendererControlBehaviour> inputPlayable = (ScriptPlayable<SpriteRendererControlBehaviour>)playable.GetInput(i);
                    SpriteRendererControlBehaviour behaviour = inputPlayable.GetBehaviour();
                    
                    currentAlpha = inputWeight;

                    if(behaviour.isUsingBehaviourAlpha && !(inputWeight < 1))
                    {
                        currentAlpha = behaviour.alpha;
                    }
                }

                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, currentAlpha);
            }
        }
        public override void OnPlayableDestroy(Playable playable)
        {
            processedFrame = false;

            if (!spriteRenderer) return;

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, defaultAlpha);
        }
    }
}