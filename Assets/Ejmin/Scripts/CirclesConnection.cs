using System.Collections.Generic;
using UnityEngine;
using Shapes;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class CirclesConnection : ImmediateModeShapeDrawer
{
    [Header("Camera")] public Camera renderingCamera;
    [Space]
    [SerializeField] protected bool isMovingCamera;
    [SerializeField] protected float distanceCheck;
    protected float distanceFromCamera = -1;

    [Header("Points")]
    [SerializeField] protected List<Vector3> localPoints = new List<Vector3>();
    [SerializeField] protected List<Vector3> localRotations = new List<Vector3>();

    [Header("Rings Props")]
    [SerializeField] private Color innerRingInitialColor;
    [SerializeField] private Color outerRingInitialColor;
    [SerializeField] private float innerRingRadius;
    [SerializeField] private float outerRingRadius;
    [SerializeField] private float innerRingThickness;
    [SerializeField] private float outerRingThickness;

    [Header("Dashes")]
    public DashStyle dashStyle;
    public float thickness;
    public Color dashColor;
    public LineEndCap dashLineEndCapStyle;

    [Header("PolyLines")]
    [SerializeField] private Vector3[] smallPolyLinePoints;
    [SerializeField] private Color smallPolyLineColor;
    [SerializeField] private float smallPolyLineThickness;
    [SerializeField] private PolylineJoins smallPolylineJoins;
    [SerializeField] private bool smallPolylineClosedEnd;

    private PolylinePath smallPath = new PolylinePath();
    private int localPointsCountTemp;

    protected Vector3 worldPositionForCheckingDistance;
    private List<PolylinePath> smallPolyLinePathTemp = new List<PolylinePath>();

    public override void DrawShapes(Camera cam)
    {
        if (null != this.renderingCamera) // only draw in the player camera
            return;

        using (Draw.Command(cam))
        {
            Draw.ResetAllDrawStates();
            Draw.Matrix = transform.localToWorldMatrix;

            for (int i = 0; i < localPoints.Count; i++)
            {
                Draw.Ring(localPoints[i], Quaternion.Euler(localRotations[i]), innerRingRadius, innerRingThickness, innerRingInitialColor);
                Draw.Ring(localPoints[i], Quaternion.Euler(localRotations[i]), outerRingRadius, outerRingThickness, outerRingInitialColor);

                int j = i + 1 >= localPoints.Count ? 0 : i + 1;

                Draw.DashStyle = dashStyle;
                Draw.Line(localPoints[i] + (localPoints[j] - localPoints[i]).normalized * outerRingRadius, localPoints[j] + (localPoints[i] - localPoints[j]).normalized * outerRingRadius, thickness, dashLineEndCapStyle, dashColor);

                for (int m = 0; m < smallPolyLinePathTemp.Count; m++)
                {
                    Draw.Polyline(smallPolyLinePathTemp[m], smallPolylineClosedEnd, smallPolyLineThickness, smallPolylineJoins, smallPolyLineColor);
                }
            }
        }

        Draw.ResetAllDrawStates();
    }


#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        smallPath = new PolylinePath();
        smallPath.AddPoints(smallPolyLinePoints);


        smallPolyLinePathTemp = new List<PolylinePath>();
        for (int i = 0; i < localPoints.Count; i++)
        {
            smallPolyLinePathTemp.Add(new PolylinePath());

            for (int j = 0; j < smallPolyLinePoints.Length; j++)
            {
                smallPolyLinePathTemp[i].AddPoint(smallPolyLinePoints[j] + localPoints[i]);
            }
        }
    }
#endif

    protected virtual void Start()
    {
        smallPolyLinePathTemp = new List<PolylinePath>();
        for (int i = 0; i < localPoints.Count; i++)
        {
            smallPolyLinePathTemp.Add(new PolylinePath());

            for (int j = 0; j < smallPolyLinePoints.Length; j++)
            {
                smallPolyLinePathTemp[i].AddPoint(smallPolyLinePoints[j] + localPoints[i]);
            }
        }
    }

    protected virtual void Update()
    {
        worldPositionForCheckingDistance = transform.position;

        if (isMovingCamera)
        {
            distanceFromCamera = Vector3.Distance(renderingCamera.transform.position, worldPositionForCheckingDistance);
        }

        if (localRotations.Count != localPoints.Count)
        {
            if (localPoints.Count > localPointsCountTemp)
            {
                for (int i = 0; i < localPoints.Count - localPointsCountTemp; i++)
                {
                    localRotations.Add(Vector3.zero);
                }

                for (int i = 0; i < localPoints.Count - localPointsCountTemp; i++)
                {
                    smallPolyLinePathTemp.Add(new PolylinePath());

                    for (int j = 0; j < smallPolyLinePoints.Length; j++)
                    {
                        smallPolyLinePathTemp[i].AddPoint(smallPolyLinePoints[j] + localPoints[i]);
                    }
                }
            }
            else
            {
                localRotations.Clear();
                for (int i = 0; i < localPoints.Count; i++)
                {
                    localRotations.Add(Vector3.zero);
                }

                smallPolyLinePathTemp = new List<PolylinePath>();
                for (int i = 0; i < localPoints.Count; i++)
                {
                    smallPolyLinePathTemp.Add(new PolylinePath());

                    for (int j = 0; j < smallPolyLinePoints.Length; j++)
                    {
                        smallPolyLinePathTemp[i].AddPoint(smallPolyLinePoints[j] + localPoints[i]);
                    }
                }
            }

            localPointsCountTemp = localPoints.Count;
        }
    }
}
