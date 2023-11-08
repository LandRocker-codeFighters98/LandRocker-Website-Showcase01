using System.Collections.Generic;
using UnityEngine;
using Shapes;
using UnityEngine.Playables;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class UIScroll : ImmediateModeShapeDrawer
{
    //Camera
    [SerializeField] private Camera cameraRendering;

    //Click Props
    [SerializeField] private float radiusForClick;

    //PolyLine Props
    [SerializeField] private int polyLineCount;
    [SerializeField] private Vector3[] polyLinePath;
    [SerializeField] private Color polyLineColor;
    [SerializeField] private float polyLineThickness;
    [SerializeField] private Vector3 offsetOnPolyLines;

    //Dash Props
    [SerializeField] private DashStyle dashStyle;
    [SerializeField] private Vector3 dashOffset;
    [SerializeField] private Color dashColor;
    [SerializeField] private float dashThickness;

    //Disc Props
    [SerializeField] private float discRadius;

    //Line Props
    [SerializeField] private float lineThickness;
    [SerializeField] private Color lineColor;
    [SerializeField] private LineEndCap lineEndCap;

    //Short Texts
    [SerializeField] private string[] shortTexts;
    [SerializeField] private Vector3 shortTextsOffset;
    [SerializeField] private float shortTextsFontSize;
    [SerializeField] private Color shortTextsColor;

    //Intro Texts
    [SerializeField] private string[] introTexts;
    [SerializeField] private Vector3 introTextsOffset;
    [SerializeField] private float introTextsFontSize;
    [SerializeField] private Color introTextsColor;

    //Scolling Props
    private Vector3 ourPos;
    private List<bool> isPassed = new List<bool>();
    public float scrollScaler;

    private PolylinePath path = new PolylinePath();

    private Vector3 initialPos;
    private Vector3 lastPos;
    private GameObject secondaryTransform;
    private int polyLineCountTemp;

    //Lerping 
    [SerializeField] private float lerpingDuration;
    [SerializeField] private float lerpingInverseDuration;
    [SerializeField] private float lerpingScaler;
    [SerializeField] private float lerpingInverseScaler;
    private float lerpingTime = 1;
    private float lerpingTimeInverse = 2;

    PolylinePath temp = new PolylinePath();
    PolylinePath temp02 = new PolylinePath();

    //TimeLine
    [SerializeField] private PlayableDirector director;
    [SerializeField] private float timeLineLerpSpeed;

    public float timeLineLerpSpeedScaler = 1;
    public float TimeLineLerpSpeedScaler { get => timeLineLerpSpeedScaler; set => timeLineLerpSpeedScaler = value; }

    public float scrollAmountScaler = 1;
    public float ScrollAmountScaler { get => scrollAmountScaler; set => scrollAmountScaler = value; }

    //Touch
    private Vector2 previousTouch;
    private Vector2 currentTouch;
    private float moveTouch;

    void Start()
    {
        path.AddPoints(polyLinePath);

        //Secondary Transform Props
        secondaryTransform = Instantiate(new GameObject(), initialPos, transform.rotation, transform);
        secondaryTransform.name = "SecondaryTransform";

        //Scrolling
        ourPos = initialPos = lastPos;
    }

    void Update()
    {
        if (initialPos != transform.position)
        {
            float difference = initialPos.y - ourPos.y;
            ourPos = initialPos = transform.position;
            ourPos.y -= difference;
        }

        if (polyLineCount != polyLineCountTemp)
        {
            polyLineCountTemp = polyLineCount;
            lastPos = initialPos;
            for (int i = 1; i <= polyLineCount; ++i)
            {
                lastPos += offsetOnPolyLines;
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

        //Scroll
        float input = Input.GetAxisRaw("Mouse ScrollWheel");
        ourPos.y += input * scrollAmountScaler * scrollScaler;

        //Click/Drag
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 mousePos = cameraRendering.ScreenToWorldPoint(Input.mousePosition);
            //float ydif = Mathf.Abs(mousePos.y - secondaryTransform.transform.position.y);
            float xdif = Mathf.Abs(mousePos.x - secondaryTransform.transform.position.x);

            if (xdif < radiusForClick)
            {
                ourPos.y = mousePos.y;
            }
        }

        ////Touch Swiping
        //Touch touch = new Touch();
        //Vector2 touchDir = Vector2.zero;

        //if (Input.touchCount > 0)
        //{
        //    touch = Input.GetTouch(0);

        //    if (touch.phase == TouchPhase.Began)
        //        previousTouch = touch.position;
        //    else if (touch.phase == TouchPhase.Moved)
        //        currentTouch = touch.position;
        //    else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
        //        currentTouch = previousTouch = Vector2.zero;
        //}

        //touchDir = (currentTouch - previousTouch).normalized;

        //if ((touchDir.x > 0 && touchDir.y > 0) || (touchDir.x < 0 && touchDir.y > 0))
        //    moveTouch = 1;//up
        //else if ((touchDir.x > 0 && touchDir.y < 0) || (touchDir.x < 0 && touchDir.y < 0))
        //    moveTouch = -1; //down
        //else
        //    moveTouch = 0; //nothing

        //ourPos.y += moveTouch;

        ////Touch Click
        //if (Input.touchCount > 0)
        //{
        //    Vector3 touchPos = cameraRendering.ScreenToWorldPoint(touch.position);
        //    float ydif = Mathf.Abs(touchPos.y - secondaryTransform.transform.position.y);
        //    float xdif = Mathf.Abs(touchPos.x - secondaryTransform.transform.position.x);

        //    if (xdif < radiusForClick || ydif < radiusForClick)
        //    {
        //        ourPos.y = touchPos.y;
        //    }
        //}

        //Clamping
        ourPos.y = Mathf.Clamp(ourPos.y, lastPos.y, initialPos.y);

        if (ourPos.y == lastPos.y)
        {
            ourPos = initialPos;
        }

        //TimeLine
        if (director != null)
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

                #region [new Code for lerping]
                if (ourPos.y < secondaryTransform.transform.position.y && ourPos.y > secondaryTransform.transform.position.y + offsetOnPolyLines.y)
                {
                    Draw.Text(Vector3.zero + shortTextsOffset, shortTexts[(i - 1) % shortTexts.Length], shortTextsFontSize, shortTextsColor);
                    Draw.Text(Vector3.zero + introTextsOffset, introTexts[(i - 1) % introTexts.Length], TextAlign.Left, introTextsFontSize, introTextsColor);


                    if (!isPassed[i - 1])
                    {
                        isPassed[i - 1] = true;
                        lerpingTime = 1;
                    }

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

                    Draw.Polyline(temp, polyLineThickness, polyLineColor);

                    if (i == polyLineCount)
                    {
                        Draw.DashStyle = dashStyle;
                        Draw.Line(temp.LastPoint.point, offsetOnPolyLines, dashThickness, dashColor);
                    }
                    else
                    {
                        Draw.DashStyle = dashStyle;
                        Draw.Line(temp.LastPoint.point, offsetOnPolyLines + dashOffset, dashThickness, dashColor);
                    }
                }
                else
                {
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

                        if (i == polyLineCount && isPassed[i - 1])
                        {
                            Draw.DashStyle = dashStyle;
                            Draw.Line(temp.LastPoint.point, offsetOnPolyLines, dashThickness, dashColor);
                        }
                        else if (isPassed[i - 1])
                        {
                            Draw.DashStyle = dashStyle;
                            Draw.Line(temp.LastPoint.point, offsetOnPolyLines + dashOffset, dashThickness, dashColor);
                        }
                    }
                }
                #endregion

                if (i == polyLineCount && !isPassed[i - 1])
                {
                    Draw.DashStyle = dashStyle;
                    Draw.Line(path.LastPoint.point, offsetOnPolyLines,  dashThickness, dashColor);
                }
                else if (!isPassed[i - 1])
                {
                    Draw.DashStyle = dashStyle;
                    Draw.Line(path.LastPoint.point, offsetOnPolyLines + dashOffset,  dashThickness, dashColor);
                }

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

    void OnDestroy() => path.Dispose();

}
