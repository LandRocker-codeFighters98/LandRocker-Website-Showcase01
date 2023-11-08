using System.Collections.Generic;
using UnityEngine;
using Shapes;
using UnityEngine.Playables;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class UIScrollFraction : ImmediateModeShapeDrawer
{
    public float textFontLerpDuration;
    public float textFontInverseLerpDuration;

    [Header("Camera")]
    [SerializeField] private Camera cameraRendering;

    [Header("Click Props")]
    [SerializeField] private float radiusForClick;

    [Header("Scroll Props")]
    [SerializeField] private float scrollScaler;

    [HideInInspector] public float scrollAmountScaler = 1;
    public float ScrollAmountScaler { get => scrollAmountScaler; set => scrollAmountScaler = value; }

    [Header("Polyline Props")]
    [SerializeField] private int polyLineCount;
    [SerializeField] private Vector3[] polyLinePath;
    [SerializeField] private Vector3[] smallPolyLinePath;

    [SerializeField] private Color polyLineColor;
    [SerializeField] private float polyLineThickness;

    [SerializeField] private Color smallPolyLineColor;
    [SerializeField] private float smallPolyLineThickness;

    [SerializeField] private Vector3 offsetOnPolyLines;

    [Header("Dash Props")]
    [SerializeField] private DashStyle dashStyle;
    [SerializeField] private Color dashColor;
    [SerializeField] private float dashThickness;

    [Header("Line Props")]
    [SerializeField] private float lineThickness;
    [SerializeField] private Color lineColor;
    [SerializeField] private LineEndCap lineEndCap;

    [Header("TimeLine")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private float timeLineLerpSpeed;

    [HideInInspector] public float timeLineLerpSpeedScaler = 1;
    public float TimeLineLerpSpeedScaler { get => timeLineLerpSpeedScaler; set => timeLineLerpSpeedScaler = value; }

    //initial Props
    private Vector3[] polyLinePathTemp;

    private List<bool> isPassed = new List<bool>();

    private PolylinePath path = new PolylinePath();
    private PolylinePath smallPath = new PolylinePath();

    public Vector3 ourPos;
    public Vector3 tempScale;
    public Vector3 initialPos;
    public Vector3 lastPos;
    private GameObject secondaryTransform;
    private int polyLineCountTemp;

    //Touch
    private Vector2 previousTouch;
    private Vector2 currentTouch;
    private float moveTouch;

    //Dashes
    private Vector3 dashOffset;

    //PolyLine Temp
    private Vector3 offsetOnPolyLinesTemp;

    public float scale = 1;

    void Start()
    {
        for (int i = 0; i < polyLinePath.Length; i++)
        {
            polyLinePath[i].x *= scale;
        }
        path.AddPoints(polyLinePath);
        smallPath.AddPoints(smallPolyLinePath);

        offsetOnPolyLinesTemp = offsetOnPolyLines;

        tempScale = transform.localScale;
        ourPos = initialPos = lastPos = transform.position;

        //Secondary Transform Props
        secondaryTransform = Instantiate(new GameObject(), initialPos, transform.rotation, transform);
        secondaryTransform.name = "SecondaryTransform";

        //dashOffset
        dashOffset = offsetOnPolyLines - path.LastPoint.point;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        path = new PolylinePath();
        for (int i = 0; i < polyLinePath.Length; i++)
        {
            polyLinePath[i].x *= scale;
        }
        path.AddPoints(polyLinePath);

        smallPath = new PolylinePath();
        smallPath.AddPoints(smallPolyLinePath);

        //dashOffset
        dashOffset = offsetOnPolyLines - path.LastPoint.point;
    }
#endif

    void Update()
    {
        if (tempScale != transform.localScale)
        {
            tempScale = transform.localScale;

            Vector3 difference = initialPos - Vector3.Scale(ourPos, transform.localScale);
            ourPos = initialPos = transform.position;
            ourPos.y -= difference.y;

            lastPos = initialPos = transform.position;
            for (int i = 1; i <= polyLineCount; ++i)
            {
                if (i != polyLineCount)
                    lastPos += Vector3.Scale(offsetOnPolyLines, (transform.localScale));
            }

            dashOffset = offsetOnPolyLines - path.LastPoint.point;
        }

        if (initialPos != transform.position)
        {
            Vector3 difference = initialPos - Vector3.Scale(ourPos, transform.localScale);
            ourPos = initialPos = transform.position;
            ourPos.y -= difference.y;
        }

        if (polyLineCount != polyLineCountTemp)
        {
            polyLineCountTemp = polyLineCount;

            Vector3 difference = initialPos - Vector3.Scale(ourPos, transform.localScale);
            ourPos = initialPos = lastPos = transform.position;
            ourPos.y -= difference.y;

            for (int i = 1; i <= polyLineCount; ++i)
            {
                if (i != polyLineCount)
                    lastPos += Vector3.Scale(offsetOnPolyLines, (transform.localScale));
            }
        }

        if (offsetOnPolyLines != offsetOnPolyLinesTemp)
        {
            offsetOnPolyLinesTemp = offsetOnPolyLines;

            initialPos = lastPos = transform.position;

            for (int i = 1; i <= polyLineCount; ++i)
            {
                if (i != polyLineCount)
                    lastPos += Vector3.Scale(offsetOnPolyLines, (transform.localScale));
            }

            dashOffset = offsetOnPolyLines - path.LastPoint.point;
        }

        if (isPassed.Count != polyLineCount)
        {
            isPassed.Clear();
            for (int i = 0; i < polyLineCount; i++)
            {
                isPassed.Add(false);
            }
        }

        //Scroll
        float input = Input.GetAxisRaw("Mouse ScrollWheel");
        ourPos.y += input * scrollAmountScaler * scrollScaler;

        //Click/Drag
        if (cameraRendering != null)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Vector3 mousePosInPixels = Input.mousePosition;
                mousePosInPixels.z = (cameraRendering.transform.position - transform.position).magnitude;
                Vector3 mousePos = cameraRendering.ScreenToWorldPoint(mousePosInPixels);

                float radius = Vector3.Distance(mousePos, secondaryTransform.transform.position);

                if (radius < radiusForClick)
                {
                    ourPos.y = mousePos.y;
                }
            }
        }

        //Clamping
        ourPos.y = Mathf.Clamp(ourPos.y, lastPos.y, initialPos.y);

        if (ourPos.y == lastPos.y)
        {
            ourPos = initialPos;
        }

        //TimeLine
        if (director)
        {
            double scrollBarPercenatege = (ourPos - initialPos).magnitude / (lastPos - initialPos).magnitude;
            director.time = Mathf.Lerp((float)director.time, (float)(scrollBarPercenatege * director.duration), Time.deltaTime * timeLineLerpSpeed * timeLineLerpSpeedScaler);
        }
    }

    public override void DrawShapes(Camera cam)
    {
        if (null != this.cameraRendering) // only draw in the player camera
            return;

        using (Draw.Command(cam))
        {
            Draw.ResetAllDrawStates();
            Draw.ResetMatrix();
            secondaryTransform.transform.position = initialPos;

            for (int i = 1; i <= polyLineCount; ++i)
            {
                Draw.Matrix = secondaryTransform.transform.localToWorldMatrix;

                if (ourPos.y < secondaryTransform.transform.position.y && ourPos.y > secondaryTransform.transform.position.y + Vector3.Scale(offsetOnPolyLines, transform.localScale).y)
                {
                    if (!isPassed[i - 1])
                    {
                        isPassed[i - 1] = true;
                    }

                }
                else
                {
                    if (isPassed[i - 1])
                    {
                        isPassed[i - 1] = false;
                    }
                }

                if (!isPassed[i - 1])
                    Draw.Polyline(smallPath, smallPolyLineThickness, smallPolyLineColor);

                Draw.Polyline(path, polyLineThickness, polyLineColor);

                if (i != polyLineCount)
                {
                    secondaryTransform.transform.position += Vector3.Scale(offsetOnPolyLines, transform.localScale);
                    Draw.DashStyle = dashStyle;
                    Draw.Line(path.LastPoint.point, dashOffset, dashThickness, dashColor);
                }
            }

            Draw.ResetMatrix();

            Draw.DashStyle = DashStyle.defaultDashStyleLine;
            Draw.Line(initialPos, ourPos, lineThickness, lineEndCap, lineColor);

            Draw.ResetAllDrawStates();
        }
    }

    void OnDestroy() => path.Dispose();
}
