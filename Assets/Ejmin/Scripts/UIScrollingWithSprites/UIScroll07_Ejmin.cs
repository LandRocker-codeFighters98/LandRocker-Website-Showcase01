using Shapes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using TMPro;
using System;
using System.Collections.Generic;

public class UIScroll07_Ejmin : ImmediateModeShapeDrawer
{
    delegate void UITextManagerForScrolling(int index);

    [Header("Events For Text Appearing")]
    public UnityEvent<int>[] textsManager_Actions;

    [Header("Camera")]
    [SerializeField] private Camera cameraRendering;

    [Header("Click Props")]
    [SerializeField] private float radiusForClick;
    [SerializeField] private float radiusForMouseEntering;

    [Header("Scroll Props")]
    [SerializeField] private float scrollScaler;

    [HideInInspector] public float scrollAmountScaler = 1;
    public float ScrollAmountScaler { get => scrollAmountScaler; set => scrollAmountScaler = value; }

    [Header("Sprites Props")]
    //Sprite height
    public float spriteHeightLogo;
    public float spriteHeightLogo_Focused;


    [Header("Sprite Renderer")]
    [SerializeField] private int spriteCount;
    [SerializeField] private Vector3 offsetOnSprites;
    [SerializeField] private Vector3 offsetOnSprites_Focused;
    [SerializeField] private Vector3 offsetLogos;
    GameObject[] logoGameObjects_ForGeneralAndFocused;

    [Header("Sprite Renderer for General")]
    [SerializeField] private Sprite spriteLogo_General;
    [SerializeField] private Color spriteLogo_Color_General;
    [SerializeField] private Vector3 spriteLogo_Scale_General;
    [SerializeField] private float alphaSpriteScaler_LogoSprite_General;
    [SerializeField] private float alphaSpriteScaler_LogoSprite_Fixed_General;
    [SerializeField] private float spriteLogoLerpScalerForBecomingBack_General;

    [Header("Sprite Renderer for Focused")]
    [SerializeField] private Sprite spriteLogo_Focused;
    [SerializeField] private Color spriteLogo_Focused_Color;
    [SerializeField] private Vector3 spriteLogo_Focues_Scale;
    [SerializeField] private float lerpingScalerFor_Focused;

    [Header("Sprite Renderer for Focused Glow")]
    [SerializeField] private Sprite spriteLogo_Focused_Glow;
    [SerializeField] private Color spriteLogo_Focused_Glow_Color;
    [SerializeField] private Vector3 spriteLogo_Focused_Glow_Scale;
    [SerializeField] private float lerpingScalerFor_Focused_Glow;
    GameObject[] logoGlowGameObjects_Focused_Glow;

    [Header("Sprite Renderer for Glow Circle")]
    [SerializeField] private Sprite spriteLogo_Circle_Glow;
    [SerializeField] private Color spriteLogo_Circle_Glow_Color;
    [SerializeField] private Vector3 spriteLogo_Circle_Glow_Scale;
    [SerializeField] private Vector3 spriteLogo_Circle_Glow_Scale_Lerping;
    [SerializeField] private float Circle_Glow_Lerp_Scaler;
    GameObject[] logoGameObjects__Circle_Glow;
    private Vector3 spriteLogo_Circle_Glow_Scale_Temp;

    [Header("Dash Props")]
    [SerializeField] private DashStyle dashStyle;
    [SerializeField] private Color dashColor;
    [SerializeField] private float dashThickness;

    [Header("Disc Props")]
    [SerializeField] private float discRadius;
    [SerializeField] private Color discColor;

    [Header("Line Moving Curve Props")]
    [SerializeField] private int stepCountForMovingCurve;
    [SerializeField] private float lineThicknessMovingCurve;
    [SerializeField] private Color lineColorMovingCurve;
    [SerializeField] private LineEndCap lineEndCapMovingCurve;

    [Header("Line Props")]
    [SerializeField] private float lineThickness;
    [SerializeField] private Color lineColor;
    [SerializeField] private LineEndCap lineEndCap;
    [SerializeField] private float lineLerpingScaler;

    [Header("Arc Props Line")]
    [SerializeField] private float height_Line;
    [SerializeField] private float stepCount_Line = 20;
    public float landRockerArcLineThickness_Line;
    public Color landRockerArcLineColor_Line;
    public LineEndCap landRockerArcLineEndCap_Line;
    private bool[] isPassedTotal;
    private int indexForPassed;
    private int indexForPassedForDrawingLine;

    [Header("Arc Props Dashed")]
    [SerializeField] private float height_Dashed;
    [SerializeField] private float stepCount_Dashed = 20;
    public float landRockerArcLineThickness_Dashed;
    public Color landRockerArcLineColor_Dashed;
    public LineEndCap landRockerArcLineEndCap_Dashed;

    [Header("Intro Texts")]
    [SerializeField] private string[] introTexts;
    [SerializeField] private Vector3 introTextsOffset;
    [SerializeField] private float introTextsFontSize;
    [SerializeField] private TMP_FontAsset introFontAsset_Gerenal;
    [SerializeField] private TMP_FontAsset introFontAsset_Focused;
    [SerializeField] private float introTextsAngle;
    [SerializeField] private float alphaTextScaler;
    [SerializeField] private float alphaTextFixed;
    [SerializeField] private Color introTextsColor_Focused;
    [SerializeField] private Color introTextsColor_Generic;
    [SerializeField] private TextAlign introTextsAlign;
    private Color[] introTextsColors;

    [Header("Text Colliders")]
    [Tooltip("It is just for showing , there is no need to fill the spots in the inspector")] [SerializeField] private GameObject[] textCollidersGameObjects;
    [SerializeField] private Vector3 textColliderAngleDeg;
    [SerializeField] private Vector3 textColliderOffset;
    [SerializeField] private Vector3 textColliderSize;
    private Vector3 textColliderSizeTemp;
    private Vector3 textColliderAngleDegTemp;
    private Vector3 textColliderOffsetTemp;

    [Header("TimeLine")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private float timeLineLerpSpeed;
    public bool isScrolling = false;

    [HideInInspector] public float timeLineLerpSpeedScaler = 1;
    public float TimeLineLerpSpeedScaler { get => timeLineLerpSpeedScaler; set => timeLineLerpSpeedScaler = value; }
    public Vector3[] points;

    //initial Props
    public Vector3 ourPos;
    public Vector3 initialPos;
    public Vector3 lastPos;
    public Vector3 tempScale;

    private GameObject secondaryTransform;
    private int spritesCountTemp;

    //Touch
    private Vector2 previousTouch;
    private Vector2 currentTouch;
    private float moveTouch;

    //Dashes
    private Vector3 dashOffset;

    //Sprite Offset Temp
    private Vector3 offsetOnSprtitesTemp;

    //Passed
    bool[] isPassed;
    bool[] isPassedQuick;

    //When Passed
    int tempindexForPassed = 1;
    bool lineStartingPassed;
    Vector3 lineStartingTemp = Vector3.zero;


    void Start()
    {
        spriteLogo_Circle_Glow_Scale_Temp = spriteLogo_Circle_Glow_Scale;

        offsetOnSprtitesTemp = offsetOnSprites;

        tempScale = transform.localScale;
        ourPos = initialPos = lastPos = transform.position;

        //Secondary Transform Props
        secondaryTransform = Instantiate(new GameObject(), initialPos, transform.rotation, transform);
        secondaryTransform.name = "SecondaryTransform";

        introTextsColors = new Color[introTexts.Length];
        for (int i = 0; i < introTextsColors.Length; i++)
        {
            introTextsColors[i] = introTextsColor_Generic;
        }
    }

    void Update()
    {
        if (tempScale != transform.localScale)
        {
            tempScale = transform.localScale;

            Vector3 difference = initialPos - ourPos;
            ourPos = initialPos = lastPos = transform.position;
            ourPos.x -= difference.x;

            for (int i = 1; i <= spriteCount; ++i)
            {
                lastPos += offsetOnSprites;
            }
        }

        if (initialPos != transform.position)
        {
            Vector3 difference = initialPos - ourPos;
            ourPos = initialPos = transform.position;
            ourPos.x -= difference.x;
        }

        if (offsetOnSprites != offsetOnSprtitesTemp)
        {
            offsetOnSprtitesTemp = offsetOnSprites;

            ourPos = initialPos = lastPos = transform.position;

            for (int i = 1; i <= spriteCount; ++i)
            {
                lastPos += offsetOnSprites;
            }
        }

        if (spriteCount != spritesCountTemp)
        {

            for (int i = 0; i < spritesCountTemp; i++)
            {
                //Sprites for Focused and general
                Destroy(logoGameObjects_ForGeneralAndFocused[i]);
            }

            for (int i = 0; i < spritesCountTemp; i++)
            {
                //Sprites Focused Glow Back
                Destroy(logoGlowGameObjects_Focused_Glow[i]);
            }

            for (int i = 0; i < spritesCountTemp; i++)
            {
                //Sprites Circle Glow Back
                Destroy(logoGameObjects__Circle_Glow[i]);
            }

            if (textCollidersGameObjects != null)
            {
                for (int i = 0; i < spritesCountTemp; i++)
                {
                    //Sprites Circle Glow Back
                    if (textCollidersGameObjects[i] != null)
                        Destroy(textCollidersGameObjects[i]);
                }
            }

            spritesCountTemp = spriteCount;
            lastPos = initialPos = transform.position;
            for (int i = 1; i <= spriteCount; ++i)
            {
                lastPos += offsetOnSprites;
            }

            //Sprites
            logoGameObjects_ForGeneralAndFocused = new GameObject[spriteCount];
            logoGlowGameObjects_Focused_Glow = new GameObject[spriteCount];
            logoGameObjects__Circle_Glow = new GameObject[spriteCount];
            isPassed = new bool[spriteCount];
            isPassedQuick = new bool[spriteCount];
            isPassedTotal = new bool[spriteCount];
            for (int i = 0; i < spriteCount; i++)
            {
                //Sprites For Gerneral And Focuses
                logoGameObjects_ForGeneralAndFocused[i] = new GameObject();
                logoGameObjects_ForGeneralAndFocused[i].transform.parent = transform;
                logoGameObjects_ForGeneralAndFocused[i].transform.localPosition = Vector3.zero;
                logoGameObjects_ForGeneralAndFocused[i].transform.name = "LogoGameObject_ForGeneralAndFocused" + i;
                logoGameObjects_ForGeneralAndFocused[i].AddComponent<SpriteRenderer>();
                logoGameObjects_ForGeneralAndFocused[i].GetComponent<SpriteRenderer>().sprite = spriteLogo_General;

                logoGameObjects_ForGeneralAndFocused[i].transform.localPosition += offsetOnSprites * i + offsetLogos;

                //Sprites Focused
                logoGlowGameObjects_Focused_Glow[i] = new GameObject();
                logoGlowGameObjects_Focused_Glow[i].transform.parent = transform;
                logoGlowGameObjects_Focused_Glow[i].transform.localPosition = Vector3.zero;
                logoGlowGameObjects_Focused_Glow[i].transform.name = "LogoGlowGameObject_" + i;
                logoGlowGameObjects_Focused_Glow[i].AddComponent<SpriteRenderer>();
                logoGlowGameObjects_Focused_Glow[i].GetComponent<SpriteRenderer>().sprite = null;

                logoGlowGameObjects_Focused_Glow[i].transform.localPosition += offsetOnSprites * i + offsetLogos;

                //Sprites Glow Circle
                logoGameObjects__Circle_Glow[i] = new GameObject();
                logoGameObjects__Circle_Glow[i].transform.parent = transform;
                logoGameObjects__Circle_Glow[i].transform.localPosition = Vector3.zero;
                logoGameObjects__Circle_Glow[i].transform.name = "LogoGlowGameObject_Circle" + i;
                logoGameObjects__Circle_Glow[i].AddComponent<SpriteRenderer>();
                logoGameObjects__Circle_Glow[i].GetComponent<SpriteRenderer>().sprite = null;

                logoGameObjects__Circle_Glow[i].transform.localPosition += offsetOnSprites * i + offsetLogos;

                isPassed[i] = false;
                isPassedQuick[i] = false;
            }

            //Colliders
            textCollidersGameObjects = new GameObject[spriteCount];
            for (int i = 0; i < spriteCount; i++)
            {
                textCollidersGameObjects[i] = new GameObject("Collider_" + i);
                textCollidersGameObjects[i].transform.parent = transform;
                textCollidersGameObjects[i].transform.position = transform.position + (offsetOnSprites * i) + introTextsOffset + textColliderOffset;
                textCollidersGameObjects[i].transform.rotation = Quaternion.Euler(textColliderAngleDeg);
                BoxCollider boxCollider = textCollidersGameObjects[i].AddComponent<BoxCollider>();
                boxCollider.size = textColliderSize;
                boxCollider.isTrigger = true;
            }
        }

        if (textColliderOffsetTemp != textColliderOffset || textColliderAngleDegTemp != textColliderAngleDeg || textColliderSizeTemp != textColliderSize)
        {
            for (int i = 0; i < spriteCount; i++)
            {
                textCollidersGameObjects[i].transform.position = transform.position + (offsetOnSprites * i) + introTextsOffset + textColliderOffset;
                textCollidersGameObjects[i].transform.rotation = Quaternion.Euler(textColliderAngleDeg);
                BoxCollider boxCollider = textCollidersGameObjects[i].GetComponent<BoxCollider>();
                boxCollider.size = textColliderSize;

                textColliderOffsetTemp = textColliderOffset;
                textColliderAngleDegTemp = textColliderAngleDeg;
                textColliderSizeTemp = textColliderSize;
            }
        }


        if (isScrolling)
        {

            //Scroll
            float input = Input.GetAxisRaw("Mouse ScrollWheel");
            ourPos.x -= input * scrollAmountScaler * scrollScaler;

            //Click/Drag

            if (cameraRendering != null)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Ray ray = cameraRendering.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        ourPos = initialPos + (offsetOnSprites * Convert.ToInt32(hit.collider.name.Substring(9)));
                        ourPos += Vector3.right * 0.1f;
                        indexForPassedForDrawingLine = Convert.ToInt32(hit.collider.name.Substring(9)) + 1;
                        //lineStattingTemp = initialPos + (offsetOnSprites * (tempindexForPassed - 1));
                        //indexForPassedForDrawingLine = Convert.ToInt32(hit.collider.name.Substring(9)) - 1;
                    }

                    ////Vector3 mousePos = Input.mousePosition;
                    ////mousePos.z = cameraRendering.transform.position.z;
                    //Ray ray = cameraRendering.ScreenPointToRay(Input.mousePosition);
                    //Vector3 hitPoint = ray.direction * (cameraRendering.transform.position - transform.position).magnitude;

                    //// GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //// sphere.transform.position = hitPoint;

                    //if (hitPoint.y < secondaryTransform.transform.position.y + radiusForClick)
                    //{
                    //    ourPos.x = hitPoint.x;
                    //}

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
            ourPos.x = Mathf.Clamp(ourPos.x, initialPos.x, lastPos.x);

            if (ourPos.x == lastPos.x)
            {
                ourPos = initialPos;
                lineStartingTemp = initialPos;

                indexForPassedForDrawingLine = 0;
                tempindexForPassed = 1;

                lineStartingPassed = false;
                endOfLine = false;
            }

            //TimeLine
            if (director)
            {
                double scrollBarPercenatege = (ourPos - initialPos).magnitude / (lastPos - initialPos).magnitude;
                director.time = Mathf.Lerp((float)director.time, (float)(scrollBarPercenatege * director.duration), Time.deltaTime * timeLineLerpSpeed * timeLineLerpSpeedScaler);
            }
        }
    }

    bool iscircleBig = false;
    bool isFocusedLogoBig = false;

    public override void DrawShapes(Camera cam)
    {
        if (cameraRendering != null)
        {
            if (cam != this.cameraRendering) // only draw in the player camera
                return;
        }

        using (Draw.Command(cam))
        {
            Draw.ResetAllDrawStates();
            Draw.ResetMatrix();
            secondaryTransform.transform.position = initialPos;

            for (int i = 1; i <= spriteCount; ++i)
            {
                Draw.Matrix = secondaryTransform.transform.localToWorldMatrix;

                #region [new Code for lerping]
                if (ourPos.x >= secondaryTransform.transform.position.x && ourPos.x < secondaryTransform.transform.position.x + offsetOnSprites.x)
                {
                    indexForPassed = i;
                    indexForPassedForDrawingLine = i;

                    //Sprites Focused Logo
                    SpriteRenderer _spriteRenderer_Focused = logoGameObjects_ForGeneralAndFocused[i - 1].GetComponent<SpriteRenderer>();

                    _spriteRenderer_Focused.sprite = spriteLogo_Focused;
                    _spriteRenderer_Focused.sortingOrder = -1;
                    _spriteRenderer_Focused.color = spriteLogo_Focused_Color;

                    //Sprites Focused Glow
                    SpriteRenderer _spriteRenderer_Focused_Glow = logoGlowGameObjects_Focused_Glow[i - 1].GetComponent<SpriteRenderer>();
                    _spriteRenderer_Focused_Glow.sprite = spriteLogo_Focused_Glow;
                    _spriteRenderer_Focused_Glow.color = spriteLogo_Focused_Glow_Color;
                    _spriteRenderer_Focused_Glow.sortingOrder = -2;

                    if (!isPassed[i - 1])
                    {
                        isPassedQuick[i - 1] = true;
                        isPassed[i - 1] = true;

                        if (textsManager_Actions.Length > 0)
                            textsManager_Actions[0].Invoke(i - 1);

                        spriteLogo_Circle_Glow_Scale = spriteLogo_Circle_Glow_Scale_Temp;
                        iscircleBig = false;
                        isFocusedLogoBig = false;

                        _spriteRenderer_Focused.transform.localScale = Vector3.zero;
                        _spriteRenderer_Focused_Glow.transform.localScale = Vector3.zero;
                    }

                    _spriteRenderer_Focused.transform.localScale = Vector3.Lerp(_spriteRenderer_Focused.transform.localScale, spriteLogo_Focues_Scale, lerpingScalerFor_Focused * Time.deltaTime);
                    _spriteRenderer_Focused_Glow.transform.localScale = Vector3.Lerp(_spriteRenderer_Focused_Glow.transform.localScale, spriteLogo_Focused_Glow_Scale, lerpingScalerFor_Focused_Glow * Time.deltaTime);


                    //Sprites Focused Glow Circle
                    SpriteRenderer _spriteRenderer_Glow_Circle = logoGameObjects__Circle_Glow[i - 1].GetComponent<SpriteRenderer>();

                    _spriteRenderer_Glow_Circle.sprite = spriteLogo_Circle_Glow;



                    if (!iscircleBig)
                    {
                        spriteLogo_Circle_Glow_Scale = Vector3.Lerp(spriteLogo_Circle_Glow_Scale, spriteLogo_Circle_Glow_Scale_Lerping, Circle_Glow_Lerp_Scaler * Time.deltaTime);

                        if ((spriteLogo_Circle_Glow_Scale - spriteLogo_Circle_Glow_Scale_Lerping).magnitude < 0.05f)
                            iscircleBig = true;
                    }

                    if (iscircleBig)
                    {
                        spriteLogo_Circle_Glow_Scale = Vector3.Lerp(spriteLogo_Circle_Glow_Scale, spriteLogo_Circle_Glow_Scale_Temp, Circle_Glow_Lerp_Scaler * Time.deltaTime);

                        if ((spriteLogo_Circle_Glow_Scale - spriteLogo_Circle_Glow_Scale_Temp).magnitude < 0.05f)
                            iscircleBig = false;
                    }

                    _spriteRenderer_Glow_Circle.color = spriteLogo_Circle_Glow_Color;
                    _spriteRenderer_Glow_Circle.transform.localScale = spriteLogo_Circle_Glow_Scale;
                    _spriteRenderer_Glow_Circle.sortingOrder = -3;

                    Draw.Text(introTextsOffset, introTextsAngle, introTexts[(i - 1) % spriteCount], introTextsAlign, introTextsFontSize, introFontAsset_Focused, introTextsColor_Focused);
                }
                else
                {
                    //Sprites
                    SpriteRenderer _spriteRenderer = logoGameObjects_ForGeneralAndFocused[i - 1].GetComponent<SpriteRenderer>();
                    _spriteRenderer.sprite = spriteLogo_General;
                    _spriteRenderer.sortingOrder = -1;
                    Color color = new Color(spriteLogo_Color_General.r, spriteLogo_Color_General.g, spriteLogo_Color_General.b, (alphaSpriteScaler_LogoSprite_General * spriteLogo_Color_General.a * (1 - (ourPos - secondaryTransform.transform.position).magnitude / (initialPos - lastPos).magnitude)));

                    if (color.a > alphaSpriteScaler_LogoSprite_Fixed_General)
                    {
                        _spriteRenderer.color = new Color(spriteLogo_Color_General.r, spriteLogo_Color_General.g, spriteLogo_Color_General.b, (alphaSpriteScaler_LogoSprite_General * spriteLogo_Color_General.a * (1 - (ourPos - secondaryTransform.transform.position).magnitude / (initialPos - lastPos).magnitude)));
                    }
                    else
                    {
                        _spriteRenderer.color = new Color(spriteLogo_Color_General.r, spriteLogo_Color_General.g, spriteLogo_Color_General.b, alphaSpriteScaler_LogoSprite_Fixed_General);
                    }


                    SpriteRenderer _spriteRenderer_Focused = logoGlowGameObjects_Focused_Glow[i - 1].GetComponent<SpriteRenderer>();
                    _spriteRenderer_Focused.sprite = null;
                    _spriteRenderer_Focused.color = spriteLogo_Focused_Glow_Color;
                    _spriteRenderer_Focused.transform.localScale = spriteLogo_Focused_Glow_Scale;

                    if (isPassed[i - 1])
                    {
                        if (isPassedQuick[i - 1])
                        {
                            _spriteRenderer.transform.localScale = Vector3.zero;
                            _spriteRenderer_Focused.transform.localScale = Vector3.zero;
                            isPassedQuick[i - 1] = false;

                            if (textsManager_Actions.Length > 0)
                                textsManager_Actions[1].Invoke(i - 1);
                        }

                        if ((_spriteRenderer.transform.localScale - spriteLogo_Scale_General).magnitude < 0.5f)
                            isPassed[i - 1] = false;
                    }
                    //Sprite for default Logos
                    _spriteRenderer.transform.localScale = Vector3.Lerp(_spriteRenderer.transform.localScale, spriteLogo_Scale_General, spriteLogoLerpScalerForBecomingBack_General * Time.deltaTime);

                    SpriteRenderer _spriteRenderer_Glow_Circle = logoGameObjects__Circle_Glow[i - 1].GetComponent<SpriteRenderer>();
                    _spriteRenderer_Glow_Circle.sprite = null;
                    _spriteRenderer_Glow_Circle.color = spriteLogo_Circle_Glow_Color;
                    _spriteRenderer_Glow_Circle.transform.localScale = spriteLogo_Circle_Glow_Scale;

                    introTextsColors[i - 1] = new Color(introTextsColor_Generic.r, introTextsColor_Generic.g, introTextsColor_Generic.b, (1 - alphaTextScaler * (introTextsColor_Generic.a * (ourPos - secondaryTransform.transform.position).magnitude / (initialPos - lastPos).magnitude)));

                    if (introTextsColors[i - 1].a < alphaTextFixed)
                    {
                        introTextsColors[i - 1] = new Color(introTextsColor_Generic.r, introTextsColor_Generic.g, introTextsColor_Generic.b, alphaTextFixed);
                    }

                    Draw.Text(introTextsOffset, introTextsAngle, introTexts[(i - 1) % spriteCount], introTextsAlign, introTextsFontSize, introFontAsset_Gerenal, introTextsColors[i - 1]);
                }
                #endregion

                if (!isPassedQuick[i - 1])
                {
                    dashOffset = offsetOnSprites - spriteHeightLogo * Vector3.right;
                    if (i == spriteCount)
                    {
                        Draw.Disc(Vector3.zero + offsetOnSprites, discRadius, discColor);
                        Draw.DashStyle = dashStyle;
                        Draw.Line(spriteHeightLogo * Vector3.right, offsetOnSprites, dashThickness, dashColor);
                    }
                    else
                    {
                        Draw.DashStyle = dashStyle;
                        Draw.Line(spriteHeightLogo * Vector3.right, dashOffset, dashThickness, dashColor);
                    }
                }
                else
                {
                    dashOffset = offsetOnSprites_Focused - spriteHeightLogo_Focused * Vector3.right;
                    if (i == spriteCount)
                    {
                        Draw.Disc(Vector3.zero + offsetOnSprites, discRadius, discColor);
                        Draw.DashStyle = dashStyle;
                        Draw.Line(spriteHeightLogo_Focused * Vector3.right, offsetOnSprites, dashThickness, dashColor);
                    }
                    else
                    {
                        Draw.DashStyle = dashStyle;
                        Draw.Line(spriteHeightLogo_Focused * Vector3.right, dashOffset, dashThickness, dashColor);
                    }
                }

                DrawArc(Vector3.zero, offsetOnSprites, (i - 1) % 2 == 0 ? height_Dashed : -height_Dashed, stepCount_Dashed, landRockerArcLineThickness_Dashed, landRockerArcLineEndCap_Dashed, landRockerArcLineColor_Dashed);

                secondaryTransform.transform.localPosition += offsetOnSprites;

                if (ourPos.x > initialPos.x && ourPos.x <= initialPos.x + offsetOnSprites.x * indexForPassed)
                {
                    Draw.Matrix = transform.localToWorldMatrix;
                    for (int o = 1; o <= indexForPassed - 1; o++)
                    {
                        DrawArc(Vector3.zero + offsetOnSprites * (o - 1), Vector3.zero + (offsetOnSprites) * (o), (o) % 2 == 0 ? height_Line : -height_Line, stepCount_Line, landRockerArcLineThickness_Line, landRockerArcLineEndCap_Line, landRockerArcLineColor_Line);

                        //if (lastPos.x - ourPos.x < 0.2f)
                        //    DrawArc(Vector3.zero + offsetOnSprites * (indexForPassed - 1), Vector3.zero + offsetOnSprites * (indexForPassed), (indexForPassed) % 2 == 0 ? height_Line : -height_Line, stepCount_Line, landRockerArcLineThickness_Line, landRockerArcLineEndCap_Line, landRockerArcLineColor_Line);
                    }

                    for (int o = 0; o <= indexForPassed - 1; o++)
                    {
                        if (o == indexForPassed - 1)
                        {
                            //For Debuging *** Not Necessary ***
                            //Debug.Log(o + "  isPassed");

                            Vector3 initialPosOnFractionedLine = offsetOnSprites * (o);
                            Vector3 ourPosOnFractionedLine = (transform.InverseTransformPoint(ourPos) - (offsetOnSprites) * (o));
                            Vector3 endPosOnFractionedLine = (offsetOnSprites) * (o + 1);

                            float pointOnCurve = ((o - 1) % 2 == 0 ? height_Line : -height_Line) * Mathf.Sin((((initialPosOnFractionedLine.x + ourPosOnFractionedLine.x) - initialPosOnFractionedLine.x) / (endPosOnFractionedLine.x - initialPosOnFractionedLine.x)) * Mathf.PI);

                            //Draws the red line going down and up For Debuging *** Not Necessary ***
                            //Draw.Line(initialPosOnFractionedLine, initialPosOnFractionedLine + ourPosOnFractionedLine + pointOnCurve * Vector3.up, 0.3f, Color.red);


                            //Drawling the actual line

                            Draw.DashStyle = DashStyle.defaultDashStyleLine;
                            float tempBefore;
                            float tempAfter;
                            for (int a = 0; a < stepCountForMovingCurve; a++)
                            {
                                Vector3 ourPosBefore = ourPosOnFractionedLine;
                                ourPosBefore.x = (ourPosOnFractionedLine.x * a) / stepCountForMovingCurve;

                                Vector3 ourPosAfter = ourPosOnFractionedLine;
                                ourPosAfter.x = (ourPosOnFractionedLine.x * a + 1) / stepCountForMovingCurve;


                                tempBefore = ((o - 1) % 2 == 0 ? height_Line : -height_Line) * Mathf.Sin((((initialPosOnFractionedLine.x + ourPosBefore.x) - initialPosOnFractionedLine.x) / (endPosOnFractionedLine.x - initialPosOnFractionedLine.x)) * Mathf.PI);
                                tempAfter = ((o - 1) % 2 == 0 ? height_Line : -height_Line) * Mathf.Sin((((initialPosOnFractionedLine.x + ourPosAfter.x) - initialPosOnFractionedLine.x) / (endPosOnFractionedLine.x - initialPosOnFractionedLine.x)) * Mathf.PI);
                                Draw.Line(initialPosOnFractionedLine + ourPosBefore + tempBefore * Vector3.up, initialPosOnFractionedLine + ourPosAfter + tempAfter * Vector3.up, lineThicknessMovingCurve, lineEndCapMovingCurve, lineColorMovingCurve);
                            }
                        }
                    }


                    if (i == indexForPassedForDrawingLine && tempindexForPassed != indexForPassedForDrawingLine)
                    {
                        if (!lineStartingPassed)
                        {
                            lineStartingPassed = true;
                            lineStartingTemp = initialPos + (offsetOnSprites * (tempindexForPassed - 1));
                        }

                        if(indexForPassed > tempindexForPassed)
                        lineStartingTemp = Vector3.Lerp(lineStartingTemp, initialPos + (offsetOnSprites * (indexForPassedForDrawingLine - 1)), lineLerpingScaler * Time.deltaTime);

                        if (lineStartingTemp.x >= initialPos.x + (offsetOnSprites * (indexForPassedForDrawingLine - 1)).x - 0.2f)
                        {
                            lineStartingPassed = false;
                            lineStartingTemp = initialPos + (offsetOnSprites * (indexForPassedForDrawingLine - 1));
                            tempindexForPassed = indexForPassedForDrawingLine;
                        }
                        // Not needed
                        //Draw.Line(initialPos + (offsetOnSprites * (indexForPassedForDrawingLine - 1)), lineStartingTemp, lineThickness, lineEndCap, lineColor);
                    }

                    if (ourPos.x >= lastPos.x - 0.45f)
                    {
                        endOfLine = true;
                    }
                    else
                    {
                        endOfLine = false;
                    }
                }
            }

            Draw.ResetMatrix();

            //Drawing the Straight Line From start to ourPos
            //Draw.DashStyle = DashStyle.defaultDashStyleLine;
            //Draw.Line(initialPos, ourPos, lineThickness, lineEndCap, lineColor);

            if (!endOfLine)
            {
                Draw.Line(initialPos, lineStartingTemp, lineThickness, lineEndCap, lineColor);
            }
            else
            {
                Draw.Line(initialPos, lastPos, lineThickness, lineEndCap, lineColor);
            }

            Draw.ResetAllDrawStates();
        }
    }

    private bool endOfLine;

    #region[Drawing the static Curves]
    void DrawArc(Vector3 start, Vector3 end, float height, float stepCount, float landRockerArcLineThickness, LineEndCap landRockerArcLineEndCap, Color landRockerArcLineColor)
    {
        //Draw the height in the viewport, so i can make a better gif :]
        //Handles.BeginGUI();
        //GUI.skin.box.fontSize = 16;
        //GUI.Box(new Rect(10, 10, 100, 25), h + "");
        //Handles.EndGUI();

        //Draw the parabola by sample a few times
        //Draw.Color = Color.red;
        //Draw.Line(a, b);
        Vector3 lastP = start;
        for (float i = 0; i < stepCount + 1; i++)
        {
            Vector3 p = SampleParabola(start, end, height, (i / stepCount_Dashed));
            //Draw.Color = i % 2 == 0 ? Color.blue : Color.green;
            Draw.Line(lastP, p, landRockerArcLineThickness, landRockerArcLineEndCap, landRockerArcLineColor);
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
        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += Mathf.Sin(t * Mathf.PI) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
            Vector3 up = Vector3.Cross(right, travelDirection);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += (Mathf.Sin(t * Mathf.PI) * height) * up.normalized;
            return result;
        }
    }
    #endregion
    #endregion

    public void EnableScrolling()
    {
        isScrolling = true;
    }
}
