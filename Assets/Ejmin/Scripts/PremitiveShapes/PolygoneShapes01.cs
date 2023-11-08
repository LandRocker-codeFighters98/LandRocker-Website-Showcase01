using UnityEngine;
using Shapes;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class PolygoneShapes01 : ImmediateModeShapeDrawer
{
    [Header("Camera")]
    [SerializeField] private Camera cameraForRendering;

    [SerializeField] private Vector2[] points;
    [SerializeField] private PolygonPath path;
    [SerializeField] private GradientFill polygonGradientFill;
    [SerializeField] private ShapesBlendMode polygonBlendMode;
    [SerializeField] private PolygonTriangulation polygonTriangulation;
    [SerializeField] private Color polygonColor;

    // Start is called before the first frame update
    void Start()
    {
        path = new PolygonPath();
        path.AddPoints(points);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        path = new PolygonPath();
        path.AddPoints(points);
    }
#endif

    // Update is called once per frame
    void Update()
    {
        if (path.Count != points.Length)
        {
            path = new PolygonPath();
            path.AddPoints(points);
        }
    }

    public override void DrawShapes(Camera cam)
    {
        if (null != this.cameraForRendering) // only draw in the player camera
            return;

        using (Draw.Command(cam))
        {  
            Draw.ResetAllDrawStates();
            Draw.Matrix = transform.localToWorldMatrix;

            Draw.GradientFill = polygonGradientFill;
            Draw.BlendMode = polygonBlendMode;
            Draw.Polygon(path, polygonTriangulation, polygonColor);

            Draw.ResetMatrix();
            Draw.ResetAllDrawStates();
        }
    }
}
