using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace LandRocker.Behaviours
{
    public class Generation_MeshGradientWithBlockChainLines_VFX : MonoBehaviour
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


        [System.Serializable]
        private class NetworkBlock
        {
            /// <summary>
            /// Sets int count to amount of lines and neighbors
            /// </summary>
            /// <param name="count">neighbors and lines count</param>
            /// <param name="transform">transform of the network</param>
            public NetworkBlock(Transform transform, int count)
            {
                neighbors = new Transform[count];
                lines = new LineRenderer[count];
                blockTransform = transform;
            }
            public Transform GetTransform { get { return blockTransform; } }
            public Transform[] GetNeighbors { get { return neighbors; } }
            public LineRenderer[] GetLines { get { return lines; } }

            protected Transform blockTransform;
            protected Transform[] neighbors;
            protected LineRenderer[] lines;
        }

        [System.Serializable]
        private class DistanceBlock
        {
            public float distance;
            public NetworkBlock networkBlock;
        }

        [Header("Blocks")]
        public Material blocksMaterial;
        public Material linesMaterial;
        public AnimationCurve lineRenderersWidth;
        public float cubesScaleMultipler = 0.01f;
        public int generationProbability = 50;
        public int lineAndNeighborsCount = 3;
        private int probability;

        [SerializeField] private List<NetworkBlock> networkBlocks = new List<NetworkBlock>();

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


                probability = 100 / generationProbability;
                if (Random.Range(0, probability) == 0)
                {
                    GameObject _GO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    _GO.GetComponent<MeshRenderer>().material = blocksMaterial;
                    _GO.transform.localScale = Vector3.one * cubesScaleMultipler;
                    _GO.transform.SetParent(meshFilter.transform);
                    _GO.transform.localPosition = Vector3.zero;
                    _GO.transform.rotation = Quaternion.LookRotation(_GO.transform.position - Camera.main.transform.position);
                    _GO.name = "Block_" + meshFilter.name;

                    NetworkBlock network = new NetworkBlock(_GO.transform, lineAndNeighborsCount);
                    networkBlocks.Add(network);
                }
            }

            for (int i = 0; i < networkBlocks.Count; i++)
            {
                List<DistanceBlock> distanceBlocks = new List<DistanceBlock>();
                for (int j = 0; j < networkBlocks.Count; j++)
                {
                    if (i != j)
                    {
                        DistanceBlock distanceBlock = new DistanceBlock();
                        distanceBlock.networkBlock = networkBlocks[j];
                        distanceBlock.distance = Vector3.Distance(networkBlocks[i].GetTransform.position, networkBlocks[j].GetTransform.position);
                        distanceBlocks.Add(distanceBlock);
                    }
                }

                distanceBlocks = distanceBlocks.OrderBy<DistanceBlock, float>((x) => x.distance).ToList();

                for (int j = 0; j < lineAndNeighborsCount; j++)
                {
                    networkBlocks[i].GetNeighbors[j] = distanceBlocks[j].networkBlock.GetTransform;

                    GameObject _GO = new GameObject();
                    _GO.name = networkBlocks[i].GetTransform.name + "_Line_" + j.ToString();
                    LineRenderer lineRenderer = _GO.AddComponent<LineRenderer>();
                    lineRenderer.widthCurve = lineRenderersWidth;
                    lineRenderer.material = linesMaterial;
                    networkBlocks[i].GetLines[j] = lineRenderer;
                }
            }
        }


        protected void Update()
        {
            for (int i = 0; i < networkBlocks.Count; i++)
            {
                for (int j = 0; j < networkBlocks[i].GetNeighbors.Length; j++)
                {
                    Vector3[] Poses = new Vector3[2];
                    Poses[0] = networkBlocks[i].GetTransform.position;
                    Poses[1] = networkBlocks[i].GetNeighbors[j].position;
                    networkBlocks[i].GetLines[j].SetPositions(Poses);
                }
            }
        }

        protected  void OnDisable()
        {
            for (int i = 0; i < networkBlocks.Count; i++)
            {
                for (int j = 0; j < networkBlocks[i].GetLines.Length; j++)
                {
                    Destroy(networkBlocks[i].GetLines[j]);
                }

                Destroy(networkBlocks[i].GetTransform.gameObject);
            }

            networkBlocks.Clear();
        }
    }


}