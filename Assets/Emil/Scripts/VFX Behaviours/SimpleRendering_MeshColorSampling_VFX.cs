using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace LandRocker.Behaviours
{
    public class SimpleRendering_MeshColorSampling_VFX : MonoBehaviour
    {
        [SerializeField] protected float lifeTimeMin = 0.5f;
        [SerializeField] protected float lifeTimeMax = 1f;
        [SerializeField] protected Vector3 particleScale = new Vector3(0.5f, 0.5f, 0.5f);
        [Tooltip("Only Alpha Is Effected")] [SerializeField] protected Gradient particleAlpha;

        [SerializeField] private VisualEffectAsset visualEffectAsset;
        [SerializeField] private ExposedProperty textureToSampleProperty;
        [SerializeField] private ExposedProperty lifeTimeMinProperty;
        [SerializeField] private ExposedProperty lifeTimeMaxProperty;
        [SerializeField] private ExposedProperty particleScaleProperty;
        [SerializeField] private ExposedProperty particleGradientProperty;
        [SerializeField] private ExposedProperty meshProperty;

        public float LifeTimeMin { get { return lifeTimeMin; } set { lifeTimeMin = value; } }
        public float LifeTimeMax { get { return lifeTimeMax; } set { lifeTimeMax = value; } }
        public Vector3 ParticleScale { get { return particleScale; } set { particleScale = value; } }
        public Gradient ParticleAlpha { get { return particleAlpha; } set { particleAlpha = value; } }

        private void OnEnable()
        {
            foreach (MeshFilter meshFilter in GetComponentsInChildren<MeshFilter>())
            {
                Mesh mesh = meshFilter.mesh;
                Transform child = meshFilter.transform;
                MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
                VisualEffect VFX;

                if (child.gameObject.GetComponent<VisualEffect>() != null)
                {
                    VFX = child.gameObject.GetComponent<VisualEffect>();
                }
                else
                {
                    VFX = child.gameObject.AddComponent<VisualEffect>();
                }

                VFX.visualEffectAsset = visualEffectAsset;

                VFX.SetFloat(lifeTimeMinProperty, lifeTimeMin);
                VFX.SetFloat(lifeTimeMaxProperty, lifeTimeMax);
                VFX.SetVector3(particleScaleProperty, particleScale);
                VFX.SetGradient(particleGradientProperty, particleAlpha);
                if (meshRenderer.material.mainTexture != null)
                    VFX.SetTexture(textureToSampleProperty, meshRenderer.material.mainTexture);
                VFX.SetMesh(meshProperty, mesh);
                meshRenderer.enabled = false;
            }
        }

    }
}