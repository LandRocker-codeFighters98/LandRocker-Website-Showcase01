using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace LandRocker.Behaviours
{
    public class SimpleGeneration_MeshColorSampling_VFX : MonoBehaviour
    {
        [SerializeField] public float TimeToStopVFXAfter;
        [SerializeField] protected float eachParticleIntervalTime = 0f;
        [SerializeField] protected uint particleGenerationPerDelay = 40;
        [SerializeField] protected Vector3 particleScale = new Vector3(0.12f, 0.12f, 0.12f);
        [SerializeField][GradientUsage(true)] protected Gradient particleGradient;
        protected float eachParticleLifeTime;
        [SerializeField] private VisualEffectAsset visualEffectAsset;
        [SerializeField] private ExposedProperty eachParticleIntervalTimeProperty;
        [SerializeField] private ExposedProperty particleGenerationPerDelayProperty;
        [SerializeField] private ExposedProperty eachParticleLifeTimeProperty;
        [SerializeField] private ExposedProperty meshProperty;
        [SerializeField] private ExposedProperty particleGradientProperty;
        [SerializeField] private ExposedProperty particleScaleProperty;
        [SerializeField] private ExposedProperty textureSamplingProperty;
        [SerializeField] private ExposedProperty stopEvent;

        public float EachParticleIntervalTime { get { return eachParticleIntervalTime; } set { eachParticleIntervalTime = value; } }
        public float EachParticleLifeTime { get { return eachParticleLifeTime; } set { eachParticleLifeTime = value; } }
        public uint ParticleGenrationPerDelay { get { return particleGenerationPerDelay; } set { particleGenerationPerDelay = value; } }
        public Vector3 ParticleScale { get { return particleScale; } set { particleScale = value; } }
        public Gradient ParticleGradient { get { return particleGradient; } set { particleGradient = value; } }


        void OnEnable()
        {
            StartCoroutine(StopingVFX());
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

                eachParticleLifeTime = TimeToStopVFXAfter;
                VFX.visualEffectAsset = visualEffectAsset;
                VFX.SetFloat(eachParticleIntervalTimeProperty, eachParticleIntervalTime);
                VFX.SetFloat(eachParticleLifeTimeProperty, eachParticleLifeTime);
                VFX.SetUInt(particleGenerationPerDelayProperty, particleGenerationPerDelay);
                VFX.SetGradient(particleGradientProperty, particleGradient);
                VFX.SetVector3(particleScaleProperty, particleScale);
                VFX.SetMesh(meshProperty, mesh);
                if (meshRenderer.material.mainTexture != null)
                    VFX.SetTexture(textureSamplingProperty, meshRenderer.material.mainTexture);
                meshRenderer.enabled = false;
            }
        }

        private IEnumerator StopingVFX()
        {
            yield return new WaitForSeconds(TimeToStopVFXAfter);
            foreach (VisualEffect vf in GetComponentsInChildren<VisualEffect>())
            {
                vf.SendEvent(stopEvent);
            }
        }

        private void OnDisable()
        {
            foreach (VisualEffect vf in GetComponentsInChildren<VisualEffect>())
            {
                vf.SendEvent(stopEvent);
            }
            StopCoroutine(StopingVFX());
        }
    }
}