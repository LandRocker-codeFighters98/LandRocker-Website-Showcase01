using UnityEngine;
using UnityEngine.VFX;

namespace LandRocker.VFX
{
    [RequireComponent(typeof(MeshFilter))]
    public class BigBang_Light : MonoBehaviour 
    {
        [SerializeField] public bool IAmAnimating = false;

        [SerializeField] protected int lightCount = 1000;
        [SerializeField] protected Gradient lightGradient;
        [SerializeField] [Range(0f, 1f)] protected float lightGradientPercent = 0;
        [SerializeField] protected AnimationCurve lightSizeCurve;
        [SerializeField] protected float lightSizeMultiplier = 2f;
        [SerializeField] [Range(0f, 1f)] protected float lightPercent = 0;

        [SerializeField] private VisualEffectAsset visualEffectAsset;

        private VisualEffect VFX;
        private Mesh mesh;

        private void Awake()
        {
            mesh = GetComponent<MeshFilter>().mesh;
        }

        void OnEnable()
        {
            if (gameObject.GetComponent<VisualEffect>() != null)
            {
                VFX = gameObject.GetComponent<VisualEffect>();
            }
            else
            {
                VFX = gameObject.AddComponent<VisualEffect>();
            }

            VFX.visualEffectAsset = visualEffectAsset;

            if (mesh)
                VFX.SetMesh("Mesh", mesh);
            VFX.SetInt("LightCount", lightCount);
            VFX.SetGradient("LightGradient", lightGradient);
            VFX.SetFloat("LightGradientPercent", lightGradientPercent);
            VFX.SetAnimationCurve("LightSizeCurve", lightSizeCurve);
            VFX.SetFloat("LightSizeMultiplier", lightSizeMultiplier);
            VFX.SetFloat("LightPercent", lightPercent);
        }

        void Update()
        {
            if (IAmAnimating)
            {
                VFX.SetFloat("LightGradientPercent", lightGradientPercent);
                VFX.SetFloat("LightPercent", lightPercent);
            }
        }

        public void SendEvent(string name)
        {
            VFX.SendEvent(name);
        }
    }
}