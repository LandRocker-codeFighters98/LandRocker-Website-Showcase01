using UnityEngine;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    public class AnimatorControlMixer : PlayableBehaviour
    {
        private Animator animator;
        private bool firstFrameProcessed;
        private float weightSum;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerData is Animator)
                animator = playerData as Animator;

            if (!animator)
                return;
            
            if(!firstFrameProcessed)
            {
                firstFrameProcessed = true;
                animator.speed = 0;
            }

            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                if (inputWeight > 0)
                {
                    weightSum += inputWeight;
                    ScriptPlayable<AnimatorControlBehaviour> inputPlayable = (ScriptPlayable<AnimatorControlBehaviour>)playable.GetInput(i);
                    AnimatorControlBehaviour behaviour = inputPlayable.GetBehaviour();

                    //double playHeadTotalTime = inputPlayable.GetGraph().GetRootPlayable(0).GetTime(); //PlayHead Time in secs In TotalTime Timeline 
                    //double duration = inputPlayable.GetDuration(); //Duration of clip where playhead is at
                    //double playHeadClipTime =inputPlayable.GetTime(); // PlayHead Time realtive to clip
                    double normalized =  inputPlayable.GetTime() / inputPlayable.GetDuration(); // Normalized duration of clip in timeline
                    animator.Play(behaviour.animationName, -1, (float)normalized);
                }
            }

            if (weightSum > 0)
            {
                weightSum = 0;
                animator.speed = 0;
            }
        }
    }
}