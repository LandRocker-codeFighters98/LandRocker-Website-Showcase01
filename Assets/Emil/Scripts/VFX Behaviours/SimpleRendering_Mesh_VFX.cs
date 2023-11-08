using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace LandRocker.Behaviours
{
    public class SimpleRendering_Mesh_VFX : MonoBehaviour
    {
        [SerializeField] protected float delay = 0.01f;
        [SerializeField] protected float lifetime = 0.05f;
        [SerializeField] protected Vector3 particleScale = new Vector3(0.15f, 0.15f, 0.15f);
        [SerializeField][ColorUsage(true,true)] protected Color particleColor = new Color(0, 75, 191, 100);

        [SerializeField] private VisualEffectAsset visualEffectAsset;

        [SerializeField] private ExposedProperty delayProperty;
        [SerializeField] private ExposedProperty lifeTimeProperty;
        [SerializeField] private ExposedProperty meshProperty;
        [SerializeField] private ExposedProperty particleScaleProperty;
        [SerializeField] private ExposedProperty particleColorProperty;

        public float Delay { get { return delay; } set { delay = value; } }
        public float Lifetime { get { return lifetime; } set { lifetime = value; } }
        public Vector3 ParticleScale { get { return particleScale; } set { particleScale = value; } }
        public Color ParticleColor { get { return particleColor; } set { particleColor = value; } }

        private void OnEnable()
        {
            foreach (MeshFilter mf in GetComponentsInChildren<MeshFilter>())
            {
                Transform child = mf.transform;
                MeshRenderer meshRenderer = mf.GetComponent<MeshRenderer>();
                Mesh mesh = mf.mesh;
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
                VFX.SetFloat(delayProperty, delay);
                VFX.SetFloat(lifeTimeProperty, lifetime);
                VFX.SetVector3(particleScaleProperty, particleScale);
                VFX.SetVector4(particleColorProperty, particleColor);
                VFX.SetMesh(meshProperty, mesh);
                meshRenderer.enabled = false;
            }
        }
    }
}