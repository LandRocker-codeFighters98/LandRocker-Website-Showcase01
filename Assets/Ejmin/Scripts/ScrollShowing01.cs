using UnityEngine;
using Shapes;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class ScrollShowing01 : ImmediateModeShapeDrawer
{
    [SerializeField] private Camera cameraForRendering;

    [Header("Polyline Props")]
    [SerializeField] private Vector3[] polyLinePath01;
    [SerializeField] private Vector3 offset01;

    [SerializeField] private Vector3[] polyLinePath02;
    [SerializeField] private Vector3 offset02;

    [SerializeField] private Color polyLineColor01;
    [SerializeField] private float polyLineThickness01;

    [SerializeField] private Color polyLineColor02;
    [SerializeField] private float polyLineThickness02;

    [Header("Line Props")]
    [SerializeField] private Color lineColor01;
    [SerializeField] private float lineThickness01;

    [SerializeField] private Color lineColor02;
    [SerializeField] private float lineThickness02;
    [SerializeField] private LineEndCap lineEndCap;
    [SerializeField] private float offsetLine01;
    [SerializeField] private float offsetLine02;
    [SerializeField] private float offsetLine03;

    [Header("Dash Props")]
    [SerializeField] private DashStyle dashStyle;
    [SerializeField] private LineEndCap dashLineEndCap;
    [SerializeField] private Color dashColor;
    [SerializeField] private Vector3 dashOffsetStart;
    [SerializeField] private Vector3 dashOffsetEnd;
    [SerializeField] private float dashThickness;

    [Header("Text")]
    [SerializeField] private Color textColor;
    [SerializeField] private string text;
    [SerializeField] private float textFontSize;
    [SerializeField] private Vector3 textOffset;
    [SerializeField] private TextAlign textAlign;

    private PolylinePath path01 = new PolylinePath();
    private PolylinePath path02 = new PolylinePath();

    private Vector3 offsetTemp01;
    private Vector3 offsetTemp02;

    // Start is called before the first frame update
    void Start()
    {
        path01.AddPoints(polyLinePath01);
        path02.AddPoints(polyLinePath02);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        path01 = new PolylinePath();
        for (int i = 0; i < polyLinePath01.Length; i++)
        {
            path01.AddPoint(polyLinePath01[i] + offset01);
        }

        path02 = new PolylinePath();
        for (int i = 0; i < polyLinePath02.Length; i++)
        {
            path02.AddPoint(polyLinePath02[i] + offset02);
        }

        offsetTemp01 = offset01;
        offsetTemp02 = offset02;
    }
#endif

    // Update is called once per frame
    void Update()
    {
        if (path01.Count != polyLinePath01.Length)
        {
            path01 = new PolylinePath();
            for (int i = 0; i < polyLinePath01.Length; i++)
            {
                path01.AddPoint(polyLinePath01[i] + offset01);
            }
        }

        if (path02.Count != polyLinePath02.Length)
        {
            path02 = new PolylinePath();
            for (int i = 0; i < polyLinePath02.Length; i++)
            {
                path02.AddPoint(polyLinePath02[i] + offset02);
            }
        }

        if (offsetTemp01 != offset01)
        {
            for (int i = 0; i < polyLinePath01.Length; i++)
            {
                path01.SetPoint(i, polyLinePath01[i] + offset01);
            }
            offsetTemp01 = offset01;
        }

        if (offsetTemp02 != offset02)
        {
            for (int i = 0; i < polyLinePath02.Length; i++)
            {
                path01.SetPoint(i, polyLinePath02[i] + offset02);
            }
            offsetTemp02 = offset02;
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

            Draw.Polyline(path01, polyLineThickness01, polyLineColor01);
            Draw.Polyline(path02, polyLineThickness02, polyLineColor02);

            Draw.Line(path01.LastPoint.point, path01.LastPoint.point + offsetLine01 * Vector3.up, lineThickness01, lineEndCap, lineColor01);
            Draw.Line(path01.LastPoint.point + offsetLine01 * Vector3.up - offsetLine02 * Vector3.right, path01.LastPoint.point + offsetLine01 * Vector3.up + offsetLine03 * Vector3.right, lineThickness02, lineEndCap, lineColor02);

            Draw.Text(textOffset, text, textAlign, textFontSize, textColor);

            Draw.DashStyle = dashStyle;
            Draw.Line(path02.LastPoint.point + dashOffsetStart, dashOffsetEnd, dashThickness,  dashLineEndCap, dashColor);
            
            Draw.ResetMatrix();
            Draw.ResetAllDrawStates();
        }
    }
}
