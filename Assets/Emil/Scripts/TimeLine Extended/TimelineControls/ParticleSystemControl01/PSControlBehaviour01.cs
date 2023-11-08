using UnityEngine;
using UnityEngine.Playables;


namespace LandRocker.TimelineTrack
{
    [System.Serializable]
    public class PSControlBehaviour01 : PlayableBehaviour
    {
        #region [ParticleSystem.main]
        public float startDelay = 0.0f;
        public float startSpeed = 5.0f;
        public float startLifetime = 5.0f;
        public float startSize = 1f;
        public float gravityModifier = 0.0f;
        public float startRotation = 0.0f;
        public Color startColor = Color.white;
        public bool useUnscaledTime = false; // ParticleSystem.main.useUnscaledTime 
        [Range(0f, 100f)] public float simulationSpeed = 1f;
        #endregion

        #region [ParticleSystem.emission]
        [Range(0f, float.PositiveInfinity)] public float rateOverTime = 10.0f; // ParticleSystem.emission.rateOverTime
        [Range(0f, float.PositiveInfinity)] public float rateOverDistance = 0.0f; // ParticleSystem.emission.rateDistance
        #endregion

        #region [ParticleSystem.shape] //Only Donut Cone Sphere HemiSphere
        public ParticleSystemShapeType shape; // ParticleSystem.ShapeModule.shape
        [Range(0f, 90f)] public float angle = 10f; // Cone
        [Range(0.0001f, float.PositiveInfinity)] public float donutRadius = 10f; // Donut
        [Range(0.0001f, float.PositiveInfinity)] public float radius = 10.0f;
        [Range(0.0f, 1.0f)] public float radiusThickness = 0.0f;
        [Range(0.0f, 360.0f)] public float arc = 360.0f;
        [Range(0.0f, 1.0f)] public float spread = 1.0f;
        public Texture2D texture;
        public Vector3 postion = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 scale = Vector3.one;
        public bool alignToDirection;
        [Range(0f, 1f)] public float randomizeDirection;
        [Range(0f, 1f)] public float spheizeDirection;
        [Range(0f, float.PositiveInfinity)] public float randomizePosition;
        #endregion

        private ParticleSystem particleSystem;
        private bool frameProcessed;
        private bool isValueSet;
        #region [ParticleSystem.main]
        private bool defaultPlayOnAwake;
        private float defaultStartDelay;
        private float defaultStartSpeed;
        private float defaultStartLifetime;
        private float defaultStartSize;
        private float defaultGravityModifier;
        private float defaultStartRotation;
        private Color defaultStartColor;
        private bool defaultUseUnscaledTime;
        private float defaultSimulationSpeed;
        #endregion

        #region [ParticleSystem.emission]
        private float defaultRateOverTime;
        private float defaultRateOverDistance;
        #endregion

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerData is ParticleSystem)
                particleSystem = playerData as ParticleSystem;

            if (!particleSystem)
                return;

            ParticleSystem.MainModule main = particleSystem.main;
            ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
            ParticleSystem.ShapeModule shapeModule = particleSystem.shape;

            ScriptPlayable<PSControlBehaviour01> inputPlayable = (ScriptPlayable<PSControlBehaviour01>)playable;
            PSControlBehaviour01 behaviour = inputPlayable.GetBehaviour();

            if (!frameProcessed)
            {
                frameProcessed = true;

                defaultPlayOnAwake = main.playOnAwake;
                defaultStartDelay = main.startDelay.constant;
                defaultStartSpeed = main.startSpeed.constant;
                defaultStartLifetime = main.startLifetime.constant;
                defaultStartSize = main.startSize.constant;
                defaultGravityModifier = main.gravityModifier.constant;
                defaultStartRotation = main.startRotation.constant;
                defaultStartColor = main.startColor.color;
                defaultUseUnscaledTime = main.useUnscaledTime;
                defaultSimulationSpeed = main.simulationSpeed;

                defaultRateOverTime = emissionModule.rateOverTime.constant;
                defaultRateOverDistance = emissionModule.rateOverDistance.constant;
            }

            if (!isValueSet)
            {
                isValueSet = true;
                main.startDelay = behaviour.startDelay;
                main.startSpeed = behaviour.startSpeed;
                main.startLifetime = behaviour.startLifetime;
                main.startSize = behaviour.startSize;
                main.gravityModifier = behaviour.gravityModifier;
                main.startRotation = behaviour.startRotation;
                main.startColor = behaviour.startColor;
                main.useUnscaledTime = behaviour.useUnscaledTime;
                main.simulationSpeed = behaviour.simulationSpeed;

                emissionModule.rateOverTime = behaviour.rateOverTime;
                emissionModule.rateOverDistance = behaviour.rateOverDistance;

                particleSystem.Play();
            }
        }
        
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!particleSystem)
                return;

            particleSystem.Stop();

            isValueSet = false;

            ParticleSystem.MainModule main = particleSystem.main;
            ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
            ParticleSystem.ShapeModule shapeModule = particleSystem.shape;

            if (frameProcessed)
            {
                frameProcessed = false;

                main.startDelay = defaultStartDelay;
                main.startSpeed = defaultStartSpeed;
                main.startLifetime = defaultStartLifetime;
                main.startSize = defaultStartSize;
                main.gravityModifier = defaultGravityModifier;
                main.startRotation = defaultStartRotation;
                main.startColor = defaultStartColor;
                main.useUnscaledTime = defaultUseUnscaledTime;
                main.simulationSpeed = defaultSimulationSpeed;

                emissionModule.rateOverTime = defaultRateOverTime;
                emissionModule.rateOverDistance = defaultRateOverDistance;

                if (defaultPlayOnAwake)
                    particleSystem.Play();
            }
        }
    }
}

