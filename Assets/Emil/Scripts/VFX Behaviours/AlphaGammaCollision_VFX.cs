using UnityEngine;
using UnityEngine.VFX.Utility;
using UnityEngine.VFX;
using LandRocker.VFX.Interfaces;

namespace LandRocker.Behaviours
{
    public class AlphaGammaCollision_VFX : MonoBehaviour , IVFX
    {
        [Header("Line Renderer with VFX")]
        [Space]

        [SerializeField] [Range(1, 4)] protected int meshCountDivider = 2;
        [SerializeField] [Range(0.2f, 10)] protected float linePosScalar = 0.2f;
        [SerializeField] protected float lineLifeTime = 5;
        [SerializeField] protected float lineMoveSpeed = 0.2f;
        [SerializeField] [GradientUsage(true)] protected Gradient lineGradient;

        [Header("BigBang Attraction Props with VFX")]
        [Space]

        [SerializeField] protected Vector3 localLookAtPos = Vector3.zero;
        [SerializeField] [Range(0f, 1f)] protected float forceRadiusPercent = 0;
        [SerializeField] [Range(0.1f, 100)] protected float forceSphereRadius = 20;
        [SerializeField] protected float attractionForce = 15.7f;
        [SerializeField] protected float attractionSpeed = 16.22f;
        [SerializeField] protected float stickForce = 513.7f;

        [SerializeField] private ExposedProperty meshProperty = "Mesh";
        [SerializeField] private ExposedProperty meshCountDividerProperty = "MeshCountDivider";
        [SerializeField] private ExposedProperty linePosScalarProperty = "LinePosScalar";
        [SerializeField] private ExposedProperty lineLifeTimeProperty = "LineLifeTime";
        [SerializeField] private ExposedProperty lineMoveSpeedProperty = "LineMoveSpeed";
        [SerializeField] private ExposedProperty lineGradientProperty = "LineGradient";
        [SerializeField] private ExposedProperty localLookAtPosProperty = "LocalLookAtPos";
        [SerializeField] private ExposedProperty forceRadiusPercentProperty = "ForceRadiusPercent";
        [SerializeField] private ExposedProperty forceSphereRadiusProperty = "ForceSphereRadius";
        [SerializeField] private ExposedProperty attractionForceProperty = "AttractionForce";
        [SerializeField] private ExposedProperty attractionSpeedProperty = "AttractionSpeed";
        [SerializeField] private ExposedProperty stickForceProperty = "StickForce";
        [SerializeField] private ExposedProperty playForceAttractionEvent = "Play_ForceAttraction";
        [SerializeField] private ExposedProperty stopForceAttractionEvent = "Stop_ForceAttraction";
        [SerializeField] private ExposedProperty playLineDrawEvent = "Play_LineDraw";
        [SerializeField] private ExposedProperty stopLineDrawEvent = "Stop_LineDraw";

        [SerializeField] protected VisualEffectAsset visualEffectAsset;

        public bool IAmAnimating = false;
        private VisualEffect VFXSelf;
        private Mesh mesh;

        public int MeshCountDivider { get { return meshCountDivider; } set { meshCountDivider = value; } }
        public float LinePosScalar { get { return linePosScalar; } set { linePosScalar = value; } }
        public float LineLifeTime { get { return lineLifeTime; } set { lineLifeTime = value; } }
        public float LineMoveSpeed { get { return lineMoveSpeed; } set { lineMoveSpeed = value; } }
        public Gradient LineGradient { get { return lineGradient; } set { lineGradient = value; } }
        public Vector3 LocalLookAtPos { get { return localLookAtPos; } set { localLookAtPos = value; } }
        public float ForceRadiusPercent { get { return forceRadiusPercent; } set { forceRadiusPercent = value; } }
        public float ForceSphereRadius { get { return forceSphereRadius; } set { forceSphereRadius = value; } }
        public float AttractionForce { get { return attractionForce; } set { attractionForce = value; } }
        public float AttractionSpeed { get { return attractionSpeed; } set { attractionSpeed = value; } }
        public float StickForce { get { return stickForce; } set { stickForce = value; } }

        protected void Awake()
        {
            VFXSelf = gameObject.AddComponent<VisualEffect>();
            VFXSelf.visualEffectAsset = visualEffectAsset;
            mesh = GetComponent<MeshFilter>().mesh;
        }

        protected void OnEnable()
        {
            if (VFXSelf == null)
                return;

            if (mesh)
                VFXSelf.SetMesh(meshProperty, mesh);

            VFXSelf.SetInt(meshCountDividerProperty, meshCountDivider);
            VFXSelf.SetFloat(linePosScalarProperty, linePosScalar);
            VFXSelf.SetFloat(lineLifeTimeProperty, lineLifeTime);
            VFXSelf.SetFloat(lineMoveSpeedProperty, lineMoveSpeed);
            VFXSelf.SetGradient(lineGradientProperty, lineGradient);
            VFXSelf.SetVector3(localLookAtPosProperty, localLookAtPos);
            VFXSelf.SetFloat(forceRadiusPercentProperty, forceRadiusPercent);
            VFXSelf.SetFloat(forceSphereRadiusProperty, forceSphereRadius);
            VFXSelf.SetFloat(attractionForceProperty, attractionForce);
            VFXSelf.SetFloat(attractionSpeedProperty, attractionSpeed);
            VFXSelf.SetFloat(stickForceProperty, stickForce);
        }

        protected void Update()
        {
            if (IAmAnimating)
            {
                if (VFXSelf != null)
                {
                    //VFXSelf.SetInt(meshCountDividerProperty, meshCountDivider);
                    VFXSelf.SetFloat(linePosScalarProperty, linePosScalar);
                    //VFXSelf.SetFloat(lineLifeTimeProperty, lineLifeTime);
                    VFXSelf.SetFloat(lineMoveSpeedProperty, lineMoveSpeed);
                    VFXSelf.SetGradient(lineGradientProperty, lineGradient);
                    //VFXSelf.SetVector3(localLookAtPosProperty, localLookAtPos);
                    VFXSelf.SetFloat(forceRadiusPercentProperty, forceRadiusPercent);
                    VFXSelf.SetFloat(forceSphereRadiusProperty, forceSphereRadius);
                    VFXSelf.SetFloat(attractionForceProperty, attractionForce);
                    VFXSelf.SetFloat(attractionSpeedProperty, attractionSpeed);
                    VFXSelf.SetFloat(stickForceProperty, stickForce);
                }
            }
        }

        protected void OnDisable()
        {
            if (VFXSelf == null)
                return;

            StopLineParticles();
            StopForceAttractionParticles();
        }

        public void PlayLineParticles()
        {
            VFXSelf.SendEvent(playLineDrawEvent);
        }
        public void StopLineParticles()
        {
            VFXSelf.SendEvent(stopLineDrawEvent);
        }
        public void PlayForceAttractionParticles()
        {
            VFXSelf.SendEvent(playForceAttractionEvent);
        }
        public void StopForceAttractionParticles()
        {
            VFXSelf.SendEvent(stopForceAttractionEvent);
        }

        public void PlayVFX(string name)
        {
            VFXSelf.SendEvent(name);
        }

        public void StopVFX(string name)
        {
            VFXSelf.SendEvent(name);
        }
    }
}