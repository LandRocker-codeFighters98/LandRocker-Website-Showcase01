using UnityEngine;
using System.Collections.Generic;

namespace LandRocker.Behaviours
{
    public class MeshGeneration_ParticleSystem : MonoBehaviour
    {
        [Header("ParticleSystem Main Props")]
        [SerializeField] protected float duration = 5f;
        [SerializeField] protected bool looping = true;
        [SerializeField] protected bool prewarm = false;
        [SerializeField] protected float startDelay = 0f;
        [SerializeField] protected float startLifeTime = 50f;
        [SerializeField] protected float startSpeed = 0f;
        [SerializeField] protected float startSize = 0.01f;
        [SerializeField] protected Color startColor = Color.white;
        [SerializeField] protected float gravityModifier = 0f;
        [SerializeField] protected ParticleSystemSimulationSpace simulationSpace = ParticleSystemSimulationSpace.Local;
        [SerializeField] protected float simulationSpeed = 1f;
        [SerializeField] protected ParticleSystemScalingMode scalingMode = ParticleSystemScalingMode.Local;
        [SerializeField] protected uint maxParticles = 5000;
        [SerializeField] protected ParticleSystemCullingMode cullingMode = ParticleSystemCullingMode.Automatic;

        [Header("ParticleSystem Emission Props")]
        [SerializeField] protected uint rateOverTime = 3000;

        [Header("ParticleSystem Shape Props")]
        [SerializeField] protected ParticleSystemShapeType shapeType = ParticleSystemShapeType.Mesh;
        [SerializeField] protected ParticleSystemMeshShapeType positionType = ParticleSystemMeshShapeType.Vertex;
        [SerializeField] protected ParticleSystemShapeMultiModeValue meshSpawnMode = ParticleSystemShapeMultiModeValue.Loop;
        [SerializeField] protected bool useMeshColor = true;
        [SerializeField] protected Texture2D texture2D;
        [SerializeField] protected bool colorEffectsParticles = true;
        [SerializeField] protected bool alphaEffectsParticles = true;

        [Header("ParticleSystem Renderer Props")]
        [SerializeField] protected ParticleSystemRenderMode renderMode = ParticleSystemRenderMode.Billboard;
        [SerializeField] protected Material material;
        [SerializeField] protected float minParticleSize = 0f;
        [SerializeField] protected float maxParticleSize = 0.5f;
        [SerializeField] protected ParticleSystemRenderSpace renderAlignment = ParticleSystemRenderSpace.Facing;

        private MeshFilter[] meshFilters;
        private List<ParticleSystem> particles = new List<ParticleSystem>();
        private bool durationIsSet = false;

        private void Awake()
        {
            meshFilters = GetComponentsInChildren<MeshFilter>();
        }
        void OnEnable()
        {
            for (int i = 0; i < meshFilters.Length; ++i)
            {
                Mesh mesh = meshFilters[i].mesh;
                MeshRenderer meshRenderer = meshFilters[i].GetComponent<MeshRenderer>();
                //meshRenderer.material = material; // Very Important to be able to animate a material //Can Animate a material
                GameObject meshFilterGameObject = meshFilters[i].gameObject;

                ParticleSystem particle;
                if (meshFilterGameObject.GetComponent<ParticleSystem>())
                    particle = meshFilterGameObject.GetComponent<ParticleSystem>();
                else
                    particle = meshFilterGameObject.AddComponent<ParticleSystem>();

                if (particle)
                {
                    particle.Stop();

                    SetParticleVariables(particle,mesh,meshRenderer);

                    particles.Add(particle);

                    particle.Play();
                }
            }
            durationIsSet = true;
        }

        protected void SetParticleVariables(ParticleSystem particle,Mesh mesh,MeshRenderer meshRenderer)
        {
            ParticleSystem.MainModule main = particle.main;

            if (!durationIsSet)
                main.duration = duration;

            main.loop = looping;
            main.prewarm = prewarm;
            main.startDelay = startDelay;
            main.startLifetime = startLifeTime;
            main.startSpeed = startSpeed;
            main.startSize = startSize;
            main.startColor = startColor;
            main.gravityModifier = gravityModifier;
            main.simulationSpace = simulationSpace;
            main.simulationSpeed = simulationSpeed;
            main.scalingMode = scalingMode;
            main.playOnAwake = false;
            main.maxParticles = (int)maxParticles;
            main.cullingMode = cullingMode;

            ParticleSystem.EmissionModule emission = particle.emission;
            emission.rateOverTime = rateOverTime;

            ParticleSystem.ShapeModule shape = particle.shape;
            shape.shapeType = shapeType;
            shape.meshShapeType = positionType;
            shape.meshSpawnMode = meshSpawnMode;
            shape.mesh = mesh;
            shape.useMeshColors = useMeshColor;
            shape.texture = texture2D;
            shape.textureColorAffectsParticles = colorEffectsParticles;
            shape.textureAlphaAffectsParticles = alphaEffectsParticles;

            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = renderMode;
            renderer.material = material;
            renderer.minParticleSize = minParticleSize;
            renderer.maxParticleSize = maxParticleSize;
            renderer.alignment = renderAlignment;

            meshRenderer.enabled = false;
        }
        void OnDisable()
        {
            for (int i = 0; i < particles.Count; ++i)
            {
                particles[i].Stop();
            }

            particles.Clear();
        }
    }
}