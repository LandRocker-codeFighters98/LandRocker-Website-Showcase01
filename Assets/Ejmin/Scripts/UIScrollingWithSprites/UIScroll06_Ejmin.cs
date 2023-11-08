using Shapes;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class UIScroll06_Ejmin : ImmediateModeShapeDrawer
{
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

    [Header("Line Props")]
    [SerializeField] private float lineThickness;
    [SerializeField] private Color lineColor;
    [SerializeField] private LineEndCap lineEndCap;

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
        }

        if (isScrolling)
        {

            //Scroll
            float input = Input.GetAxisRaw("Mouse ScrollWheel");
            ourPos.x -= input * scrollAmountScaler * scrollScaler;

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

                    if (hitPoint.y < secondaryTransform.transform.position.y + radiusForClick)
                    {
                        ourPos.x = hitPoint.x;
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
            ourPos.x = Mathf.Clamp(ourPos.x, initialPos.x, lastPos.x);

            if (ourPos.x == lastPos.x)
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
                if (ourPos.x > secondaryTransform.transform.position.x && ourPos.x <= secondaryTransform.transform.position.x + offsetOnSprites.x)
                {

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

                secondaryTransform.transform.localPosition += offsetOnSprites;
            }

            Draw.ResetMatrix();

            Draw.DashStyle = DashStyle.defaultDashStyleLine;
            Draw.Line(initialPos, ourPos, lineThickness, lineEndCap, lineColor);

            Draw.ResetAllDrawStates();
        }
    }

    public void EnableScrolling()
    {
        isScrolling = true;
    }
}
