using UnityEngine;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    [System.Serializable]
    public class LightControlBehaviour : PlayableBehaviour
    {
        public Color color = Color.white;
        public float intensity = 1f;
        public float bounceIntensity = 1f;
        public float range = 10f;

        #region [Used If Not Blending And Working With single playables]
        // //****************************************************************
        // protected bool processedFrame = false;
        // //****************************************************************
        // private Light light;
        // private Color defaultColor;
        // private float defaultIntensity;
        // private float defaultBounceIntensity;
        // private float defaultRange;

        // public override void OnPlayableCreate(Playable playable)
        // {
        //     var playableTemp = (ScriptPlayable<LightControlBehaviour>)playable;
        //     LightControlBehaviour lightControlBehaviour = playableTemp.GetBehaviour();
        // //   light lightControlBehaviour.
        // }

        // public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        // {
        //     if (playerData is Light)
        //         light = playerData as Light;

        //     if (!light) return;

        //     if (!processedFrame)
        //     {
        //         processedFrame = true;

        //         defaultColor = light.color;
        //         defaultIntensity = light.intensity;
        //         defaultBounceIntensity = light.bounceIntensity;

        //         if (light.type == LightType.Point || light.type == LightType.Spot)
        //             defaultRange = light.range;
        //     }

        //     light.color = color;
        //     light.intensity = intensity;
        //     light.bounceIntensity = bounceIntensity;

        //     if (light.type == LightType.Point || light.type == LightType.Spot)
        //         light.range = range;
        // }

        // public override void OnBehaviourPause(Playable playable, FrameData info)
        // {
        //     if (!light)
        //         return;

        //     if (processedFrame)
        //     {
        //         processedFrame = false;

        //         light.color = defaultColor;
        //         light.intensity = defaultIntensity;
        //         light.bounceIntensity = defaultBounceIntensity;

        //         if (light.type == LightType.Point || light.type == LightType.Spot)
        //             light.range = defaultRange;
        //     }
        // }
        #endregion
    }
}