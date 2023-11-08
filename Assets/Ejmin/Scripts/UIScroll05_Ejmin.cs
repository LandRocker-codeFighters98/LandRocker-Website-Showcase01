using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class UIScroll05_Ejmin : ImmediateModeShapeDrawer
{
    private List<float> shortFonts = new List<float>();
    private List<float> longFonts = new List<float>();
    public float textFontLerpDuration;
    public float textFontInverseLerpDuration;

    [Header("Camera")]
    [SerializeField] private Camera cameraRendering;

    [Header("Click Props")]
    [SerializeField] private float radiusForClick;
    [SerializeField] private float radiusForMouseEntering;

    [Header("Scroll Props")]
    [SerializeField] private float scrollScaler;

    [HideInInspector] public float scrollAmountScaler = 1;
    public float ScrollAmountScaler { get => scrollAmountScaler; set => scrollAmountScaler = value; }

    [Header("Lines Props")]
    [SerializeField] private Vector3 landRockerStartLine01;
    [SerializeField] private Vector3 landRockerEndLine01;
    [SerializeField] private Vector3 landRockerStartLine02;
    [SerializeField] private Vector3 landRockerEndLine02;
    [SerializeField] private float height;
    [SerializeField] private float stepCount = 20;
    public float landRockerArcLineThickness;
    public Color landRockerArcLineColor;
    public LineEndCap landRockerArcLineEndCap;

    [SerializeField] private float landRockerLineThckness;
    [SerializeField] private Color landRockerLineColor;
    [SerializeField] private LineEndCap landRockerLineEndCap;

    [Header("Polyline Props")]
    [SerializeField] private int polyLineCount;
    [SerializeField] private Vector3[] polyLinePath;
    [SerializeField] private Vector3[] smallPolyLinePath;

    [SerializeField] private Color polyLineColor;
    [SerializeField] private float polyLineThickness;

    [SerializeField] private Color smallPolyLineColor;
    [SerializeField] private float smallPolyLineThickness;

    [SerializeField] private Vector3 offsetOnPolyLines;

    [Header("Polyline Lerping Props")]
    [SerializeField] private float lerpingDuration;
    [SerializeField] private float lerpingInverseDuration;
    [SerializeField] private float lerpingScaler;
    [SerializeField] private float lerpingInverseScaler;

    [Header("Dash Props")]
    [SerializeField] private DashStyle dashStyle;
    [SerializeField] private Color dashColor;
    [SerializeField] private float dashThickness;

    [Header("Disc Props")]
    [SerializeField] private float discRadius;

    [Header("Line Props")]
    [SerializeField] private float lineThickness;
    [SerializeField] private Color lineColor;
    [SerializeField] private LineEndCap lineEndCap;

    [Header("Short Texts")]
    [SerializeField] private string[] shortTexts;
    [SerializeField] private Vector3 shortTextsOffset;
    [SerializeField] private float shortTextsFontSize;
    [SerializeField] private Color shortTextsColor;
    [Range(1, 20)] [SerializeField] private float shortFontScalerWhenPassed;

    [Header("Intro Texts")]
    [SerializeField] private string[] introTexts;
    [SerializeField] private Vector3 introTextsOffset;
    [SerializeField] private float introTextsFontSize;
    [SerializeField] private Color introTextsColor;
    [Range(1, float.MaxValue)] [SerializeField] private float introFontScalerWhenPassed;

    [Header("Texts Lerping Props")]
    [SerializeField] private float colorLerpingScaler;
    [SerializeField] private float colorBackLerpingScaler;

    [Header("TimeLine")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private float timeLineLerpSpeed;
    public bool isScrolling = false;

    [HideInInspector] public float timeLineLerpSpeedScaler = 1;
    public float TimeLineLerpSpeedScaler { get => timeLineLerpSpeedScaler; set => timeLineLerpSpeedScaler = value; }

    //initial Props
    public Vector3 ourPos;
    public Vector3 initialPos;
    public Vector3 lastPos;
    public Vector3 tempScale;

    private List<bool> isPassed = new List<bool>();

    private PolylinePath path = new PolylinePath();
    private PolylinePath smallPath = new PolylinePath();
    private GameObject secondaryTransform;
    private int polyLineCountTemp;

    //Lerping
    private float lerpingTime = 1;
    private float lerpingTimeInverse = 2;

    PolylinePath temp = new PolylinePath();
    PolylinePath temp02 = new PolylinePath();

    //Touch
    private Vector2 previousTouch;
    private Vector2 currentTouch;
    private float moveTouch;

    //Text Coloring
    private List<Color> colors = new List<Color>();
    private List<Color> colors01 = new List<Color>();
    private List<bool> AreTextShown = new List<bool>();
    private bool showAllTexts;

    //Dashes
    private Vector3 dashOffset;

    //PolyLine Temp
    private Vector3 offsetOnPolyLinesTemp;


    void Start()
    {
        path.AddPoints(polyLinePath);
        smallPath.AddPoints(smallPolyLinePath);

        offsetOnPolyLinesTemp = offsetOnPolyLines;

        tempScale = transform.localScale;
        ourPos = initialPos = lastPos = transform.position;

        //Secondary Transform Props
        secondaryTransform = Instantiate(new GameObject(), initialPos, transform.rotation, transform);
        secondaryTransform.name = "SecondaryTransform";

        for (int i = 0; i < polyLineCount; i++)
        {
            Color color = new Color(shortTextsColor.r, shortTextsColor.g, shortTextsColor.b, 0);
            colors.Add(color);

            color = new Color(introTextsColor.r, introTextsColor.g, introTextsColor.b, 0);
            colors01.Add(color);

            shortFonts.Add(shortTextsFontSize);
            longFonts.Add(introTextsFontSize);
        }

        //dashOffset
        dashOffset = offsetOnPolyLines - path.LastPoint.point;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        path = new PolylinePath();
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

            Vector3 difference = initialPos - ourPos;
            ourPos = initialPos = lastPos = transform.position;
            ourPos.y -= difference.y;

            for (int i = 1; i <= polyLineCount; ++i)
            {
                lastPos += offsetOnPolyLines;
            }

            dashOffset = offsetOnPolyLines - path.LastPoint.point;
        }

        if (initialPos != transform.position)
        {
            Vector3 difference = initialPos - ourPos;
            ourPos = initialPos = transform.position;
            ourPos.y -= difference.y;
        }

        if (offsetOnPolyLines != offsetOnPolyLinesTemp)
        {
            offsetOnPolyLinesTemp = offsetOnPolyLines;

            ourPos = initialPos = lastPos = transform.position;

            for (int i = 1; i <= polyLineCount; ++i)
            {
                lastPos += offsetOnPolyLines;
            }

            dashOffset = offsetOnPolyLines - path.LastPoint.point;
        }

        if (polyLineCount != polyLineCountTemp)
        {
            polyLineCountTemp = polyLineCount;
            lastPos = initialPos = transform.position;
            for (int i = 1; i <= polyLineCount; ++i)
            {
                lastPos += offsetOnPolyLines;
            }
            dashOffset = offsetOnPolyLines - path.LastPoint.point;

            colors.Clear();
            colors01.Clear();
            shortFonts.Clear();
            longFonts.Clear();
            for (int i = 0; i < polyLineCount; i++)
            {
                Color color = new Color(shortTextsColor.r, shortTextsColor.g, shortTextsColor.b, 0);
                colors.Add(color);

                color = new Color(shortTextsColor.r, shortTextsColor.g, shortTextsColor.b, 0);
                colors01.Add(color);

                shortFonts.Add(shortTextsFontSize);
                longFonts.Add(introTextsFontSize);
            }
        }

        if (isPassed.Count != polyLineCount)
        {
            isPassed.Clear();
            for (int i = 0; i < polyLineCount; i++)
            {
                isPassed.Add(false);
            }
        }

        if (AreTextShown.Count != polyLineCount)
        {
            AreTextShown.Clear();
            for (int i = 0; i < polyLineCount; i++)
            {
                AreTextShown.Add(false);
            }
        }

        if (cameraRendering != null)
        {
            Vector3 initialScreenPoint = cameraRendering.WorldToScreenPoint(initialPos);
            Vector3 lastPointScreenPoint = cameraRendering.WorldToScreenPoint(lastPos);
            if (Input.mousePosition.x < initialScreenPoint.x + radiusForMouseEntering &&
                Input.mousePosition.x > initialScreenPoint.x - radiusForMouseEntering &&
                Input.mousePosition.y < initialScreenPoint.y + radiusForMouseEntering &&
                Input.mousePosition.y > lastPointScreenPoint.y - radiusForMouseEntering
                )
            {
                showAllTexts = true;
            }
            else
            {
                showAllTexts = false;
            }
        }

        if (isScrolling)
        {

            //Scroll
            float input = Input.GetAxisRaw("Mouse ScrollWheel");
            ourPos.y += input * scrollAmountScaler * scrollScaler;

            //Click/Drag

            if (cameraRendering != null)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    //Vector3 mousePos = Input.mousePosition;
                    //mousePos.z = cameraRendering.transform.position.z;
                    Ray ray = cameraRendering.ScreenPointToRay(Input.mousePosition);
                    Vector3 hitPoint = ray.direction * (cameraRendering.transform.position - transform.position).magnitude;


                    // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    // sphere.transform.position = hitPoint;

                    if (hitPoint.x < secondaryTransform.transform.position.x + radiusForClick)
                    {
                        ourPos.y = hitPoint.y;
                    }

                    //GameObject.Destroy(sphere, 0.2f);

                    // **** Old One *** Not Working
                    //Vector3 mousePosInPixels = Input.mousePosition;
                    //mousePosInPixels.z = (cameraRendering.transform.position - transform.position).magnitude;
                    //Vector3 mousePos02 = cameraRendering.ScreenToWorldPoint(mousePosInPixels);

                    //GameObject sphere02 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //sphere02.transform.position = mousePos02;

                    //if (mousePos02.x < radiusForClick && mousePos02.y < radiusForClick)
                    //{
                    //    ourPos.y = mousePos02.y;
                    //}
                    //GameObject.Destroy(sphere02 , 0.2f);
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
    }

    public override void DrawShapes(Camera cam)
    {
        if (cam != this.cameraRendering) // only draw in the player camera
            return;

        using (Draw.Command(cam))
        {
            Draw.ResetAllDrawStates();
            Draw.ResetMatrix();

            Draw.LineGeometry = LineGeometry.Flat2D;
            secondaryTransform.transform.position = initialPos;

            for (int i = 1; i <= polyLineCount; ++i)
            {
                Draw.Matrix = secondaryTransform.transform.localToWorldMatrix;

                if (showAllTexts)
                {
                    for (int x = 0; x < colors.Count; x++)
                    {
                        colors[x] = Color.Lerp(new Color(colors[x].r, colors[x].g, colors[x].b, colors[x].a), new Color(colors[x].r, colors[x].g, colors[x].b, 1), colorLerpingScaler * Time.deltaTime);
                        Draw.Text(Vector3.zero + shortTextsOffset, shortTexts[(i - 1) % shortTexts.Length], shortFonts[i - 1], colors[x]);
                    }

                    for (int x = 0; x < colors01.Count; x++)
                    {
                        colors01[x] = Color.Lerp(new Color(colors01[x].r, colors01[x].g, colors01[x].b, colors01[x].a), new Color(colors01[x].r, colors01[x].g, colors01[x].b, 1), colorLerpingScaler * Time.deltaTime);
                        Draw.Text(Vector3.zero + introTextsOffset, introTexts[(i - 1) % introTexts.Length], TextAlign.Left, longFonts[i - 1], colors01[x]);
                    }
                }

                if (!showAllTexts)
                {
                    for (int x = 0; x < colors.Count; x++)
                    {
                        if (!isPassed[x])
                        {
                            colors[x] = Color.Lerp(new Color(colors[x].r, colors[x].g, colors[x].b, colors[x].a), new Color(colors[x].r, colors[x].g, colors[x].b, 0), colorBackLerpingScaler * Time.deltaTime);
                            Draw.Text(Vector3.zero + shortTextsOffset, shortTexts[(i - 1) % shortTexts.Length], shortFonts[i - 1], colors[x]);
                        }
                    }

                    for (int x = 0; x < colors01.Count; x++)
                    {
                        colors01[x] = Color.Lerp(new Color(colors01[x].r, colors01[x].g, colors01[x].b, colors01[x].a), new Color(colors01[x].r, colors01[x].g, colors01[x].b, 0), colorBackLerpingScaler * Time.deltaTime);
                        Draw.Text(Vector3.zero + introTextsOffset, introTexts[(i - 1) % introTexts.Length], TextAlign.Left, longFonts[i - 1], colors01[x]);
                    }
                }

                #region [new Code for lerping]
                if (ourPos.y < secondaryTransform.transform.position.y && ourPos.y > secondaryTransform.transform.position.y + offsetOnPolyLines.y)
                {
                    if (!isPassed[i - 1])
                    {
                        isPassed[i - 1] = true;
                        lerpingTime = 1;
                    }

                    if (!AreTextShown[i - 1])
                    {
                        AreTextShown[i - 1] = true;
                    }

                    if (!showAllTexts && AreTextShown[i - 1])
                    {
                        colors[i - 1] = Color.Lerp(new Color(colors[i - 1].r, colors[i - 1].g, colors[i - 1].b, colors[i - 1].a), new Color(colors[i - 1].r, colors[i - 1].g, colors[i - 1].b, 1), colorLerpingScaler * Time.deltaTime);
                    }

                    shortFonts[i - 1] = Mathf.Lerp(shortFonts[i - 1], shortTextsFontSize * shortFontScalerWhenPassed, textFontLerpDuration * Time.deltaTime);
                    longFonts[i - 1] = Mathf.Lerp(longFonts[i - 1], introTextsFontSize * introFontScalerWhenPassed, textFontLerpDuration * Time.deltaTime);

                    Draw.Text(Vector3.zero + shortTextsOffset, shortTexts[(i - 1) % shortTexts.Length], shortFonts[i - 1], colors[i - 1]);

                    if (temp.Count != path.Count)
                    {
                        temp.Dispose();
                        temp = new PolylinePath();
                        for (int m = 0; m < path.Count; m++)
                        {
                            temp.AddPoint(path[m]);
                        }
                    }

                    lerpingTime += Time.deltaTime * lerpingScaler;

                    for (int m = 0; m < temp.Count; m++)
                    {
                        if (lerpingTime < lerpingDuration)
                        {
                            temp.SetPoint(m, path[m].point * lerpingTime);
                        }
                    }

                    if (i == polyLineCount)
                    {
                        Draw.DashStyle = dashStyle;
                        Draw.Line(temp.LastPoint.point, offsetOnPolyLines, dashThickness, dashColor);
                    }
                    else
                    {
                        Draw.DashStyle = dashStyle;
                        Draw.Line(temp.LastPoint.point, dashOffset, dashThickness, dashColor);
                    }

                    Draw.Polyline(temp, polyLineThickness, polyLineColor);
                }
                else
                {
                    if (AreTextShown[i - 1])
                    {
                        AreTextShown[i - 1] = false;
                    }

                    if (!isPassed[i - 1])
                    {
                        Draw.Polyline(path, polyLineThickness, polyLineColor);
                    }
                    else if (isPassed[i - 1])
                    {
                        lerpingTimeInverse -= Time.deltaTime * lerpingInverseScaler;

                        if (temp02.Count != path.Count)
                        {
                            temp02.Dispose();
                            temp02 = new PolylinePath();
                            for (int m = 0; m < path.Count; m++)
                            {
                                temp02.AddPoint(path[m]);
                            }
                        }

                        for (int m = 0; m < temp02.Count; m++)
                        {
                            if (isPassed[i - 1])
                            {
                                temp02.SetPoint(m, path[m].point * lerpingTimeInverse);

                            }
                        }

                        if (isPassed[i - 1])
                        {
                            Draw.Polyline(temp02, polyLineThickness, polyLineColor);
                        }

                        if (lerpingTimeInverse < lerpingInverseDuration)
                        {
                            isPassed[i - 1] = false;
                            lerpingTimeInverse = 2;
                        }
                    }


                    if (!showAllTexts && !AreTextShown[i - 1])
                    {
                        colors[i - 1] = Color.Lerp(new Color(colors[i - 1].r, colors[i - 1].g, colors[i - 1].b, colors[i - 1].a), new Color(colors[i - 1].r, colors[i - 1].g, colors[i - 1].b, 0), colorBackLerpingScaler * Time.deltaTime);
                    }

                    shortFonts[i - 1] = Mathf.Lerp(shortFonts[i - 1], shortTextsFontSize, textFontInverseLerpDuration * Time.deltaTime);
                    longFonts[i - 1] = Mathf.Lerp(longFonts[i - 1], introTextsFontSize, textFontInverseLerpDuration * Time.deltaTime);

                    Draw.Text(Vector3.zero + shortTextsOffset, shortTexts[(i - 1) % shortTexts.Length], shortFonts[i - 1], colors[i - 1]);

                    if (!showAllTexts)
                        Draw.Polyline(smallPath, smallPolyLineThickness, smallPolyLineColor);

                    if (i == polyLineCount)
                    {
                        Draw.DashStyle = dashStyle;
                        Draw.Line(path.LastPoint.point, offsetOnPolyLines, dashThickness, dashColor);
                    }
                    else
                    {
                        Draw.DashStyle = dashStyle;
                        Draw.Line(path.LastPoint.point, dashOffset, dashThickness, dashColor);
                    }
                }
                #endregion

                #region [RandRocker Line]
                Draw.Line(landRockerStartLine01, landRockerEndLine01, landRockerLineThckness, landRockerLineEndCap, landRockerLineColor);
                Draw.Line(landRockerStartLine02, landRockerEndLine02, landRockerLineThckness, landRockerLineEndCap, landRockerLineColor);
                //DrawWireArc(landRockerEndLine01 , Vector3.right , anglesRangeArc01, radiusArc01, maxStepArc01);

                a = landRockerEndLine01;
                b = landRockerEndLine02;
                h = height;
                DrawArc();
                //Draw.Arc(landRockerEndLine01, landRockerArcRadius, landRockerArcThckness, landRockerArcAngleStart, landRockerArcAngleEnd, landRockerArcEndCap, landRockerArcColor);
                #endregion

                secondaryTransform.transform.localPosition += offsetOnPolyLines;

                if (i == polyLineCount)
                {
                    Draw.Disc(Vector3.zero + offsetOnPolyLines, discRadius, polyLineColor);
                }
            }

            Draw.ResetMatrix();

            Draw.Line(initialPos, ourPos, lineThickness, lineEndCap, lineColor);

            Draw.ResetAllDrawStates();
        }
    }

    // public Transform someObject; //object that moves along parabola.
    // float objectT = 0; //timer for that object

    float h; //desired parabola height
    Vector3 a, b; //Vector positions for start and end

    //void Update()
    //{
    //    if (Ta  Tb ) {
    //        a = Ta.position; //Get vectors from the transforms
    //        b = Tb.position;

    //        if (someObject)
    //        {
    //            //Shows how to animate something following a parabola
    //            objectT = Time.time % 1; //completes the parabola trip in one second
    //            someObject.position = SampleParabola(a, b, h, objectT);
    //        }
    //    }
    //}


    void DrawArc()
    {
        //Draw the height in the viewport, so i can make a better gif :]
        //Handles.BeginGUI();
        //GUI.skin.box.fontSize = 16;
        //GUI.Box(new Rect(10, 10, 100, 25), h + "");
        //Handles.EndGUI();

        //Draw the parabola by sample a few times
        //Draw.Color = Color.red;
        //Draw.Line(a, b);
        Vector3 lastP = a;
        for (float i = 0; i < stepCount + 1; i++)
        {
            Vector3 p = SampleParabola(a, b, h, (i / stepCount));
            //Draw.Color = i % 2 == 0 ? Color.blue : Color.green;
            Draw.Line(lastP, p , landRockerArcLineThickness , landRockerArcLineEndCap , landRockerArcLineColor);
            lastP = p;
        }
    }

    #region Parabola sampling function
    /// <summary>
    /// Get position from a parabola defined by start and end, height, and time
    /// </summary>
    /// <param name='start'>
    /// The start point of the parabola
    /// </param>
    /// <param name='end'>
    /// The end point of the parabola
    /// </param>
    /// <param name='height'>
    /// The height of the parabola at its maximum
    /// </param>
    /// <param name='t'>
    /// Normalized time (0->1)
    /// </param>S
    Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
    {
        if (Mathf.Abs(start.x - end.x) < 0.1f)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.x += Mathf.Sin(t * Mathf.PI) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
            Vector3 up = Vector3.Cross(right, travelDirection);
            if (end.x > start.x) up = -up;
            Vector3 result = start + t * travelDirection;
            result += (Mathf.Sin(t * Mathf.PI) * height) * up.normalized;
            return result;
        }
    }
    #endregion


    private void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, float maxSteps = 20)
    {
        var srcAngles = GetAnglesFromDir(position, dir);
        var initialPos = position;
        var posA = initialPos;
        var stepAngles = anglesRange / maxSteps;
        var angle = srcAngles - anglesRange / 2;
        for (var i = 0; i <= maxSteps; i++)
        {
            var rad = Mathf.Deg2Rad * angle;
            var posB = initialPos;
            posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

            Draw.Line(posA, posB);

            angle += stepAngles;
            posA = posB;
        }
        Draw.Line(posA, initialPos);
    }

    private float GetAnglesFromDir(Vector3 position, Vector3 dir)
    {
        var forwardLimitPos = position + dir;
        var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

        return srcAngles;
    }

    void OnDestroy() => path.Dispose();

    public void EnableScrolling()
    {
        isScrolling = true;
    }
}
