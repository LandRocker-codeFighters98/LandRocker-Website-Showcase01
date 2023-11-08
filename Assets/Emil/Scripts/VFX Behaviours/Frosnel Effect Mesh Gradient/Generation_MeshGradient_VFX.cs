using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace LandRocker.Behaviours
{
    public class Generation_MeshGradient_VFX : MonoBehaviour
    {
        [Header("Particles Are Generated Onto Mesh With Point Not Using Texture")]
        [Space]
        [Space]

        [SerializeField] protected bool isMeshRendering = true;
        [SerializeField] protected float generationDelay = 0;
        [Range(1, float.PositiveInfinity)] [SerializeField] protected float staticPosScalar = 1.1f;
        [SerializeField] [GradientUsage(true)] protected Gradient particleGradient;

        [Header("MeshGeneration_Static_VFX")]
        [SerializeField] protected VisualEffectAsset visualEffectAsset;

        [SerializeField] private ExposedProperty generationDelayProperty = "GenerationDelay";
        [SerializeField] private ExposedProperty meshProperty = "Mesh";
        [SerializeField] private ExposedProperty staticPosScalarProperty = "StaticPosScalar";
        [SerializeField] private ExposedProperty particleGradientProperty = "ParticleGradient";

        protected MeshFilter[] meshFilters;

        public float GenerationDelay { get { return generationDelay; } set { generationDelay = value; } }
        public Gradient ParticleGradient { get { return particleGradient; } set { particleGradient = value; } }


        void Awake()
        {
            meshFilters = GetComponentsInChildren<MeshFilter>();
        }

        void OnEnable()
        {
            foreach (MeshFilter meshFilter in meshFilters)
            {
                Mesh mesh = meshFilter.mesh;
                Transform selfTransform = meshFilter.transform;
                MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

                VisualEffect VFX;

                if (selfTransform.gameObject.GetComponent<VisualEffect>() != null)
                {
                    VFX = selfTransform.gameObject.GetComponent<VisualEffect>();
                }
                else
                {
                    VFX = selfTransform.gameObject.AddComponent<VisualEffect>();
                }

                VFX.visualEffectAsset = visualEffectAsset;

                VFX.SetFloat(generationDelayProperty, generationDelay);
                VFX.SetFloat(staticPosScalarProperty, staticPosScalar);
                VFX.SetMesh(meshProperty, mesh);
                VFX.SetGradient(particleGradientProperty, particleGradient);

                if (!isMeshRendering)
                    meshRenderer.enabled = false;
            }
        }
    }
}

