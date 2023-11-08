using UnityEngine;
using UnityEngine.VFX;


namespace LandRocker.VFX
{
    [RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
    public class BigBang_Expansion : MonoBehaviour
    {
        [SerializeField] public bool isMeshRendering = true;
        [SerializeField] public bool IAmAnimating = false;

        [SerializeField] [Range(1, 3)] protected int burstCountMultiplier = 1;
        [SerializeField] protected float burstDelay = 0;
        [SerializeField] protected AnimationCurve burstSizeCurve = new AnimationCurve();
        [SerializeField] protected float burstSizeMultiplier = 5;
        [SerializeField] [GradientUsage(true)] protected Gradient burstGradient;
        [SerializeField] [Range(0f, 1f)] protected float burstGradientPercent = 0;
        [SerializeField] protected Vector3 burstLookAtPosLocal = Vector3.zero;
        [SerializeField] protected AnimationCurve lineSizeCurve = new AnimationCurve();
        [SerializeField] [Range(0f, 1f)] protected float burstPercent = 0;

        [SerializeField] private VisualEffectAsset visualEffectAsset;

        private VisualEffect VFX;
        private Mesh mesh;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            mesh = GetComponent<MeshFilter>().mesh;
            meshRenderer = GetComponent<MeshRenderer>();
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
            VFX.SetInt("BurstCountMultiplier", burstCountMultiplier);
            VFX.SetFloat("BurstDelay", burstDelay);
            VFX.SetAnimationCurve("BurstSizeCurve", burstSizeCurve);
            VFX.SetFloat("BurstSizeMultiplier", burstSizeMultiplier);
            VFX.SetGradient("BurstGradient", burstGradient);
            VFX.SetFloat("BurstGradientPercent", burstGradientPercent);
            VFX.SetVector3("BurstLookAtPosLocal", burstLookAtPosLocal);
            VFX.SetAnimationCurve("LineSizeCurve", lineSizeCurve);
            VFX.SetFloat("BurstPercent", burstPercent);

            if (!isMeshRendering)
                meshRenderer.enabled = false;
        }

        void Update()
        {
            if (IAmAnimating)
            {
                VFX.SetFloat("BurstGradientPercent", burstGradientPercent);
                VFX.SetFloat("BurstPercent", burstPercent);
            }
        }

        public void SendEvent(string name)
        {
            VFX.SendEvent(name);
        }
    }
}
