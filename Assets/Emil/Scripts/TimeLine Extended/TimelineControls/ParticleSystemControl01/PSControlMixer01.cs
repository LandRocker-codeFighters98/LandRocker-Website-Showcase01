using UnityEngine;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    public class PSControlMixer01 // : PlayableBehaviour
    {
        // private ParticleSystem particleSystem;
        // private bool frameProcessed;

        // #region [ParticleSystem.main]
        // private bool defaultIsLooping;
        // private bool defaultPlayOnAwake;
        // private float defaultStartDelay;
        // private float defaultStartSpeed;
        // private float defaultStartLifetime;
        // private float defaultStartSize;
        // private float defaultGravityModifier;
        // private float defaultStartRotation;
        // private Color defaultStartColor;
        // private bool defaultUseUnscaledTime;
        // private float defaultSimulationSpeed;
        // #endregion

        // #region [ParticleSystem.emission]
        // private float defaultRateOverTime;
        // private float defaultRateOverDistance;
        // #endregion
        // public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        // {
        //     if (playerData is ParticleSystem)
        //         particleSystem = playerData as ParticleSystem;

        //     if (!particleSystem)
        //         return;

        //     ParticleSystem.MainModule main = particleSystem.main;
        //     ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
        //     ParticleSystem.ShapeModule shapeModule = particleSystem.shape;

        //     if (!frameProcessed)
        //     {
        //         frameProcessed = true;

        //         defaultIsLooping = main.loop;
        //         defaultPlayOnAwake = main.playOnAwake;
        //         defaultStartDelay = main.startDelay.constant;
        //         defaultStartSpeed = main.startSpeed.constant;
        //         defaultStartLifetime = main.startLifetime.constant;
        //         defaultStartSize = main.startSize.constant;
        //         defaultGravityModifier = main.gravityModifier.constant;
        //         defaultStartRotation = main.startRotation.constant;
        //         defaultStartColor = main.startColor.color;
        //         defaultUseUnscaledTime = main.useUnscaledTime;
        //         defaultSimulationSpeed = main.simulationSpeed;

        //         defaultRateOverTime = emissionModule.rateOverTime.constant;
        //         defaultRateOverDistance = emissionModule.rateOverDistance.constant;
        //     }

        //     int inputCount = playable.GetInputCount();

        //     for (int i = 0; i < inputCount; i++)
        //     {

        //         float inputWeight = playable.GetInputWeight(i);

        //         if (inputWeight > 0)
        //         {

        //             ScriptPlayable<PSControlBehaviour> inputPlayable = (ScriptPlayable<PSControlBehaviour>)playable.GetInput(i);
        //             PSControlBehaviour behaviour = inputPlayable.GetBehaviour();

        //             main.loop = behaviour.isLooping;
        //             main.startDelay = behaviour.startDelay;
        //             main.startSpeed = behaviour.startSpeed;
        //             main.startLifetime = behaviour.startLifetime;
        //             main.startSize = behaviour.startSize;
        //             main.gravityModifier = behaviour.gravityModifier;
        //             main.startRotation = behaviour.startRotation;
        //             main.startColor = behaviour.startColor;
        //             main.useUnscaledTime = behaviour.useUnscaledTime;
        //             main.simulationSpeed = behaviour.simulationSpeed;

        //             emissionModule.rateOverTime = behaviour.rateOverTime;
        //             emissionModule.rateOverDistance = behaviour.rateOverDistance;

        //                 particleSystem.Play();
        //         }
        //     }
        // }
        // public override void OnPlayableDestroy(Playable playable)
        // {
        //     if (!particleSystem)
        //         return;

        //     ParticleSystem.MainModule main = particleSystem.main;
        //     ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
        //     ParticleSystem.ShapeModule shapeModule = particleSystem.shape;

        //     if (frameProcessed)
        //     {
        //         frameProcessed = false;

        //         main.loop = defaultIsLooping;
        //         main.startDelay = defaultStartDelay;
        //         main.startSpeed = defaultStartSpeed;
        //         main.playOnAwake = defaultPlayOnAwake;
        //         main.startLifetime = defaultStartLifetime;
        //         main.startSize = defaultStartSize;
        //         main.gravityModifier = defaultGravityModifier;
        //         main.startRotation = defaultStartRotation;
        //         main.startColor = defaultStartColor;
        //         main.useUnscaledTime = defaultUseUnscaledTime;
        //         main.simulationSpeed = defaultSimulationSpeed;

        //         emissionModule.rateOverTime = defaultRateOverTime;
        //         emissionModule.rateOverDistance = defaultRateOverDistance;

        //         if (defaultPlayOnAwake)
        //             particleSystem.Play();

        //     }
        // }
    }
}