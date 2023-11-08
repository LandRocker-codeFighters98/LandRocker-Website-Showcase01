using UnityEngine.Playables;
using UnityEngine.AI;

namespace LandRocker.TimelineTrack
{
    public class NavMeshAgentControlMixerBehaviour : PlayableBehaviour
    {
        private NavMeshAgent navMentAgent;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (navMentAgent is NavMeshAgent)
                navMentAgent = playerData as NavMeshAgent;

            if (!navMentAgent)
                return;

            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<NavMeshAgentControlBehaviour> inputPlayable = (ScriptPlayable<NavMeshAgentControlBehaviour>)playable.GetInput(i);
                NavMeshAgentControlBehaviour input = inputPlayable.GetBehaviour();

                if (inputWeight > 0.5f && !input.destinationSet && input.destination)
                {
                    if (!navMentAgent.isOnNavMesh)
                        continue;

                    navMentAgent.SetDestination(input.destination.position);
                    input.destinationSet = true;
                }
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            navMentAgent = null;
        }
    }
}