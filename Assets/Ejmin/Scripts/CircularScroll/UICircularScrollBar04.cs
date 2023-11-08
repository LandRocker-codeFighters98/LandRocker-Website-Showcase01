using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Playables;
using Shapes;
using TMPro;

public class UICircularScrollBar04 : ImmediateModeShapeDrawer
{
    [Header("Camera")]
    [SerializeField] private Camera cameraRendering;

    [Header("Texts")]
    [SerializeField] private string[] introTexts;
    [SerializeField] private Vector3 introTextsOffset;
    [SerializeField] private float introTextsFontSize;
    [SerializeField] private float textOffset;
    [SerializeField] private TextAlign introTextsAlign;
    [SerializeField] private Color introTextsColor;
    [SerializeField] private TMP_FontAsset textFontAsset;
    [SerializeField] private float alphaTextScaler;
    [SerializeField] private float alphaTextFixed;
    private Color[] introTextsColors;
    private Vector3[] posTexts;
    private float[] angleTexts;
    private bool[] isPassedTexts;
    private bool[] isPassedTextsWithoutTheLastOne;

    [Header("Texts Scrolling Appearance")]
    [SerializeField] private TextScrollAppearing01 textAppearing;

    [Header("Text Colliders")]
    [Tooltip("It is just for showing , there is no need to fill the spots in the inspector")] [SerializeField] private GameObject[] textCollidersGameObjects;
    [SerializeField] private Vector3 textColliderSize;
    [SerializeField] private Vector3 textColliderCenter;
    private Vector3 textColliderSizeTemp;
    private Vector3 textColliderCenterTemp;

    [Header("Circle Props")]
    [SerializeField] private float radius;
    [SerializeField] private int stepCount = 200;
    [SerializeField] private float startDegree;
    [SerializeField] private float endDegree;
    [SerializeField] private float circleThickness;
    [SerializeField] private LineEndCap circleEndCap;
    [SerializeField] private Color circleColor;

    [Header("Sprite Renderer for Cross")]
    [SerializeField] private Sprite spriteLogo_Cross;
    [SerializeField] private Color spriteLogo_Cross_Color;
    [SerializeField] private Vector3 spriteLogo_Cross_Scale;
    [SerializeField] private float crossMovingOnCircleScaler;
    GameObject logoGameObjects_Cross;

    [Header("Sprite Renderer for Cross Glow")]
    [SerializeField] private Sprite spriteLogo_Cross_Glow;
    [SerializeField] private Color spriteLogo_Cross_Glow_Color;
    [SerializeField] private Vector3 spriteLogo_Cross_Glow_Scale;
    GameObject logoGlowGameObjects_Cross_GLow;

    [Header("Sprite Renderer for Glow Circle")]
    [SerializeField] private Sprite spriteLogo_Circle_Glow;
    [SerializeField] private Color spriteLogo_Circle_Glow_Color;
    [SerializeField] private Vector3 spriteLogo_Circle_Glow_Scale;
    [SerializeField] private Vector3 spriteLogo_Circle_Glow_Scale_Lerping;
    [SerializeField] private float Circle_Glow_Lerp_Scaler;
    private Vector3 spriteLogo_Circle_Glow_Scale_Temp;
    public bool circleGlowIsBig = false;
    GameObject logoGameObjects_Circle_Glow;

    [Header("Input")]
    [SerializeField] private float scrollScaler;

    [Header("TimeLine")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private float timeLineLerpSpeed;
    public bool isScrolling = false;

    //initial Props
    public Vector3 ourPos;
    public Vector3 initialPos;
    public Vector3 lastPos;
    public float rotated;
    public float rotatedWhenClicked;

    //Debuging
    GameObject circleDebug;

    //private scale once
    bool scaleCrossOnce;

    //For Scaling Once
    int indexPassedForOnceChecking = -1;

    [HideInInspector] public bool showingTextBegin = true;

    private Stack<Coroutine> coroutineStackForScrolling = new Stack<Coroutine>();

    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();

        initialPos = radius * new Vector3(Mathf.Cos(startDegree * Mathf.Deg2Rad), Mathf.Sin(startDegree * Mathf.Deg2Rad));
        lastPos = radius * new Vector3(Mathf.Cos(endDegree * Mathf.Deg2Rad), Mathf.Sin(endDegree * Mathf.Deg2Rad));
        ourPos = initialPos;
        rotated = startDegree;

        //logo Cross
        logoGameObjects_Cross = new GameObject("Cross Shape");
        logoGameObjects_Cross.transform.parent = transform;
        logoGameObjects_Cross.transform.localScale = spriteLogo_Cross_Scale;
        SpriteRenderer spriteRendere_logo_Cross = logoGameObjects_Cross.AddComponent<SpriteRenderer>();
        spriteRendere_logo_Cross.sprite = spriteLogo_Cross;
        spriteRendere_logo_Cross.sortingOrder = 2;
        spriteRendere_logo_Cross.color = spriteLogo_Cross_Color;

        //logo Cross Glow
        logoGlowGameObjects_Cross_GLow = new GameObject("Cross Shape Glow");
        logoGlowGameObjects_Cross_GLow.transform.parent = transform;
        logoGlowGameObjects_Cross_GLow.transform.localScale = spriteLogo_Cross_Glow_Scale;
        SpriteRenderer spriteRendere_logo_Cross_Glow = logoGlowGameObjects_Cross_GLow.AddComponent<SpriteRenderer>();
        spriteRendere_logo_Cross_Glow.sprite = spriteLogo_Cross_Glow;
        spriteRendere_logo_Cross_Glow.sortingOrder = 1;
        spriteRendere_logo_Cross_Glow.color = spriteLogo_Cross_Glow_Color;

        //logo Glow
        spriteLogo_Circle_Glow_Scale_Temp = spriteLogo_Circle_Glow_Scale;
        logoGameObjects_Circle_Glow = new GameObject("Glow");
        logoGameObjects_Circle_Glow.transform.parent = transform;
        logoGameObjects_Circle_Glow.transform.localScale = spriteLogo_Circle_Glow_Scale;
        SpriteRenderer spriteRendere_logo__Cirlce_Glow = logoGameObjects_Circle_Glow.AddComponent<SpriteRenderer>();
        spriteRendere_logo__Cirlce_Glow.sprite = spriteLogo_Circle_Glow;
        spriteRendere_logo__Cirlce_Glow.sortingOrder = 0;
        spriteRendere_logo__Cirlce_Glow.color = spriteLogo_Circle_Glow_Color;

        //Text Colors
        introTextsColors = new Color[introTexts.Length];
        for (int i = 0; i < introTextsColors.Length; i++)
        {
            introTextsColors[i] = introTextsColor;
        }

        //Debuging *********** Start
        //circleDebug = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //circleDebug.transform.localScale = Vector3.one * 0.1f;
        //circleDebug.transform.parent = transform;
        //Debuging *********** End
    }

    private void Start()
    {
        //Text Poses
        if (posTexts == null)
        {
            posTexts = new Vector3[introTexts.Length];

            for (int i = 0; i < posTexts.Length; i++)
            {
                float textdegree = startDegree;
                textdegree += i * (endDegree - startDegree) / (introTexts.Length - 1);
                posTexts[i] = radius * new Vector3(Mathf.Cos(textdegree * Mathf.Deg2Rad), Mathf.Sin(textdegree * Mathf.Deg2Rad)) + textOffset * new Vector3(Mathf.Cos(textdegree * Mathf.Deg2Rad), Mathf.Sin(textdegree * Mathf.Deg2Rad)) + introTextsOffset;
            }
        }

        //Text Angles
        if (angleTexts == null)
        {
            angleTexts = new float[introTexts.Length];

            for (int i = 0; i < angleTexts.Length; i++)
            {
                float textdegree = startDegree;
                textdegree += i * (endDegree - startDegree) / (introTexts.Length - 1);
                angleTexts[i] = textdegree;
            }
        }

        //Text Passed
        if (isPassedTexts == null)
        {
            isPassedTexts = new bool[introTexts.Length];
        }

        //TextsWithoutTheLastOne Passed
        if (isPassedTextsWithoutTheLastOne == null)
        {
            isPassedTextsWithoutTheLastOne = new bool[introTexts.Length];
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Text Colors
        if (introTextsColors.Length != introTexts.Length)
        {
            introTextsColors = new Color[introTexts.Length];
            for (int i = 0; i < introTextsColors.Length; i++)
            {
                introTextsColors[i] = introTextsColor;
            }
        }

        //Click/Drag
        if (cameraRendering != null)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = cameraRendering.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.tag == "ScrollBar")
                    {
                        int indexClicked = Convert.ToInt32(hit.collider.name.Substring(14));
                        float degreeToRotate = startDegree + indexClicked * ((endDegree - startDegree) / (introTexts.Length - 1));

                        //LeanTween.cancelAll(logoGameObjects_Cross);
                        //LeanTween.cancelAll(logoGlowGameObjects_Cross_GLow);

                        for (int i = 0; i < coroutineStackForScrolling.Count; i++)
                        {
                            StopCoroutine(coroutineStackForScrolling.Pop());
                        }

                        coroutineStackForScrolling.Push(StartCoroutine(whenClickedCrossLerps(degreeToRotate)));
                    }
                }
            }
        }

        if (isScrolling)
        {
            //Debuging *********** Start
            //circleDebug.transform.localPosition = ourPos;
            //Debuging *********** End

            //Scroll
            float input = Input.GetAxisRaw("Mouse ScrollWheel");

            if (input != 0)
            {
                for (int i = 0; i < coroutineStackForScrolling.Count; i++)
                {
                    StopCoroutine(coroutineStackForScrolling.Pop());
                }
            }

            rotated -= input * scrollScaler;

            rotated = Mathf.Clamp(rotated, startDegree, endDegree);
            ourPos = radius * new Vector3(Mathf.Cos((rotated) * Mathf.Deg2Rad), Mathf.Sin((rotated) * Mathf.Deg2Rad));

            //Orientations
            //Cross
            logoGameObjects_Cross.transform.localPosition = ourPos;
            logoGameObjects_Cross.transform.right = -ourPos.normalized;
            //Cross Glow
            logoGlowGameObjects_Cross_GLow.transform.localPosition = ourPos;
            logoGlowGameObjects_Cross_GLow.transform.right = -ourPos.normalized;
            //Circle Glow
            logoGameObjects_Circle_Glow.transform.localPosition = ourPos;
            logoGameObjects_Circle_Glow.transform.right = -ourPos.normalized;

            if (circleGlowIsBig)
            {
                if ((logoGameObjects_Circle_Glow.transform.localScale - spriteLogo_Circle_Glow_Scale).magnitude < 0.2f)
                    circleGlowIsBig = false;

                logoGameObjects_Circle_Glow.transform.localScale = Vector3.Lerp(logoGameObjects_Circle_Glow.transform.localScale, spriteLogo_Circle_Glow_Scale, Circle_Glow_Lerp_Scaler * Time.deltaTime * 2);
            }

            if (!circleGlowIsBig)
            {
                if ((logoGameObjects_Circle_Glow.transform.localScale - spriteLogo_Circle_Glow_Scale_Lerping).magnitude < 0.2f)
                    circleGlowIsBig = true;

                logoGameObjects_Circle_Glow.transform.localScale = Vector3.Lerp(logoGameObjects_Circle_Glow.transform.localScale, spriteLogo_Circle_Glow_Scale_Lerping, Circle_Glow_Lerp_Scaler * Time.deltaTime);
            }

            //Text Poses
            if (introTexts.Length != posTexts.Length)
            {
                posTexts = new Vector3[introTexts.Length];

                for (int i = 0; i < posTexts.Length; i++)
                {
                    float textdegree = startDegree;
                    textdegree += i * (endDegree - startDegree) / (introTexts.Length - 1);
                    posTexts[i] = radius * new Vector3(Mathf.Cos(textdegree * Mathf.Deg2Rad), Mathf.Sin(textdegree * Mathf.Deg2Rad)) + textOffset * new Vector3(Mathf.Cos(textdegree * Mathf.Deg2Rad), Mathf.Sin(textdegree * Mathf.Deg2Rad)) + introTextsOffset;
                }
            }

            //Text Angles
            if (introTexts.Length != angleTexts.Length)
            {
                angleTexts = new float[introTexts.Length];

                for (int i = 0; i < angleTexts.Length; i++)
                {
                    float textdegree = startDegree;
                    textdegree += i * (endDegree - startDegree) / (introTexts.Length - 1);
                    angleTexts[i] = textdegree;
                }
            }

            //Text Passed
            if (introTexts.Length != isPassedTexts.Length)
            {
                isPassedTexts = new bool[introTexts.Length];
            }

            //TextsWithoutTheLastOne Passed
            if (introTexts.Length != isPassedTextsWithoutTheLastOne.Length)
            {
                isPassedTextsWithoutTheLastOne = new bool[introTexts.Length];
            }

            for (int i = 0; i < angleTexts.Length; i++)
            {
                if (!(i + 1 >= angleTexts.Length))
                {
                    if (rotated >= angleTexts[i] && rotated < angleTexts[i + 1])
                    {
                        if (indexPassedForOnceChecking != i && !isPassedTexts[i])
                            indexPassedForOnceChecking = i;

                        if (!isPassedTexts[i])
                        {
                            isPassedTexts[i] = true;
                        }

                        if (!isPassedTextsWithoutTheLastOne[i] || showingTextBegin)
                        {
                            isPassedTextsWithoutTheLastOne[i] = true;

                            if (showingTextBegin)
                                showingTextBegin = false;

                            if (textAppearing && textAppearing.gameObject.activeSelf)
                            {
                                StartCoroutine(TextAppearing(i, 0.5f));
                                StopCoroutine(TextAppearing(i, 0.5f));
                            }
                        }
                    }
                    else
                    {
                        if (isPassedTexts[i])
                            isPassedTexts[i] = false;

                        if (isPassedTextsWithoutTheLastOne[i] && !(rotated >= angleTexts[angleTexts.Length - 1]))
                        {
                            isPassedTextsWithoutTheLastOne[i] = false;
                        }
                    }
                }
                else
                {
                    if (rotated >= angleTexts[i])
                    {
                        if (indexPassedForOnceChecking != i && !isPassedTexts[i])
                            indexPassedForOnceChecking = i;

                        if (!isPassedTexts[i])
                            isPassedTexts[i] = true;

                        if (!isPassedTextsWithoutTheLastOne[i] || showingTextBegin)
                        {
                            isPassedTextsWithoutTheLastOne[i] = true;

                            if (showingTextBegin)
                                showingTextBegin = false;

                            if (textAppearing && textAppearing.gameObject.activeSelf)
                            {
                                StartCoroutine(TextAppearing(i, 0.5f));
                                StopCoroutine(TextAppearing(i, 0.5f));
                            }
                        }
                    }
                    else
                    {
                        if (isPassedTexts[i])
                            isPassedTexts[i] = false;
                    }
                }
            }

            //Text Colors
            for (int i = 0; i < introTextsColors.Length; i++)
            {
                introTextsColors[i] = new Color(introTextsColor.r, introTextsColor.g, introTextsColor.b, (alphaTextScaler * (introTextsColor.a * (1 - ((ourPos - posTexts[i]).magnitude / (posTexts[0] - posTexts[1]).magnitude)))));

                if (introTextsColors[i].a < alphaTextFixed)
                {
                    introTextsColors[i] = new Color(introTextsColor.r, introTextsColor.g, introTextsColor.b, alphaTextFixed);
                }
            }

            //Text Colliders
            if (textCollidersGameObjects.Length != introTexts.Length)
            {
                textCollidersGameObjects = new GameObject[introTexts.Length];
                for (int i = 0; i < textCollidersGameObjects.Length; i++)
                {
                    textCollidersGameObjects[i] = new GameObject("Collider_Text_" + i);
                    textCollidersGameObjects[i].tag = "ScrollBar";
                    textCollidersGameObjects[i].transform.parent = transform;
                    BoxCollider boxCollider = textCollidersGameObjects[i].AddComponent<BoxCollider>();
                    textCollidersGameObjects[i].transform.localPosition = posTexts[i];

                    boxCollider.center = textColliderCenter;
                    boxCollider.size = textColliderSize;

                }
                textColliderSizeTemp = textColliderSize;
                textColliderCenterTemp = textColliderCenter;
            }

            if (textColliderCenterTemp.magnitude != textColliderCenter.magnitude)
            {
                for (int i = 0; i < textCollidersGameObjects.Length; i++)
                {
                    BoxCollider boxCollider = textCollidersGameObjects[i].GetComponent<BoxCollider>();

                    boxCollider.center = textColliderCenter;
                }

                textColliderCenterTemp = textColliderCenter;
            }

            if (textColliderSizeTemp.magnitude != textColliderSize.magnitude)
            {
                for (int i = 0; i < textCollidersGameObjects.Length; i++)
                {
                    BoxCollider boxCollider = textCollidersGameObjects[i].GetComponent<BoxCollider>();

                    boxCollider.size = textColliderSize;
                }

                textColliderSizeTemp = textColliderSize;
            }

            for (int i = 0; i < posTexts.Length; i++)
            {
                float textdegree = startDegree;
                textdegree += i * (endDegree - startDegree) / (introTexts.Length - 1);
                if (((ourPos + textOffset * new Vector3(Mathf.Cos(textdegree * Mathf.Deg2Rad), Mathf.Sin(textdegree * Mathf.Deg2Rad)) + introTextsOffset) - posTexts[i]).magnitude < 0.1f && !scaleCrossOnce && isPassedTexts[i] && indexPassedForOnceChecking >= 0)
                {
                    scaleCrossOnce = true;
                    indexPassedForOnceChecking = -1;

                    Draw.Text(posTexts[i], introTexts[i], introTextsAlign, introTextsFontSize, textFontAsset, introTextsColors[i]);

                    LeanTween.scale(logoGameObjects_Cross, logoGameObjects_Cross.transform.localScale * 1.2f, 1).setEase(LeanTweenType.punch).setOnComplete(CrossScaleOnComplete);
                    LeanTween.scale(logoGlowGameObjects_Cross_GLow, logoGlowGameObjects_Cross_GLow.transform.localScale * 1.2f, 1).setEase(LeanTweenType.punch).setOnComplete(CrossScaleOnComplete);
                }
            }

            //TimeLine
            if (director)
            {
                double scrollBarPercenatege = (ourPos - initialPos).magnitude / (lastPos - initialPos).magnitude;
                director.time = Mathf.Lerp((float)director.time, (float)(scrollBarPercenatege * director.duration), Time.deltaTime * timeLineLerpSpeed);
            }
        }
    }

    public IEnumerator TextAppearing(int index, float seconds)
    {
        textAppearing.textComponent.text = "";
        yield return new WaitForSeconds(seconds);
        textAppearing.WriteText(index == angleTexts.Length - 1 ? index - 1 : index);
    }

    private void CrossScaleOnComplete()
    {
        scaleCrossOnce = false;
    }

    public override void DrawShapes(Camera cam)
    {
        base.DrawShapes(cam);

        if (cameraRendering != null)
        {
            if (cam != this.cameraRendering) // only draw in the player camera
                return;
        }

        using (Draw.Command(cam))
        {
            Draw.Matrix = transform.localToWorldMatrix;
            for (int i = 0; i < stepCount; i++)
            {
                float tempDegreeBefore = startDegree;
                float tempDegreeAfter = startDegree;

                tempDegreeBefore += ((i) * (endDegree - startDegree)) / stepCount;
                tempDegreeAfter += ((i + 1) * (endDegree - startDegree)) / stepCount;

                Vector3 ourPosBefore = radius * new Vector3(Mathf.Cos(tempDegreeBefore * Mathf.Deg2Rad), Mathf.Sin(tempDegreeBefore * Mathf.Deg2Rad));

                Vector3 ourPosAfter = radius * new Vector3(Mathf.Cos(tempDegreeAfter * Mathf.Deg2Rad), Mathf.Sin(tempDegreeAfter * Mathf.Deg2Rad));

                Draw.Line(ourPosBefore, ourPosAfter, circleThickness, circleEndCap, circleColor);
            }

            for (int i = 0; i < introTexts.Length; i++)
            {
                float textdegree = startDegree;
                textdegree += i * (endDegree - startDegree) / (introTexts.Length - 1);
                Vector3 textPos = radius * new Vector3(Mathf.Cos(textdegree * Mathf.Deg2Rad), Mathf.Sin(textdegree * Mathf.Deg2Rad)) + textOffset * new Vector3(Mathf.Cos(textdegree * Mathf.Deg2Rad), Mathf.Sin(textdegree * Mathf.Deg2Rad)) + introTextsOffset;
                Draw.Text(textPos, introTexts[i], introTextsAlign, introTextsFontSize, textFontAsset, introTextsColors[i]);
                //if (isPassedTexts[i])
                //{
                //    Draw.Text(textPos, introTexts[i], introTextsAlign, introTextsFontSize, textFontAsset, introTextsColors[i]);
                //}
            }

            Draw.ResetAllDrawStates();
            Draw.ResetMatrix();
        }
    }

    private IEnumerator whenClickedCrossLerps(float degreeToRotate)
    {
        while (!(Mathf.Abs(rotated - degreeToRotate) < 0.1))
        {
            rotated = Mathf.LerpAngle(rotated, degreeToRotate, crossMovingOnCircleScaler * Time.deltaTime);
            ourPos = transform.TransformPoint(radius * new Vector3(Mathf.Cos((rotated) * Mathf.Deg2Rad), Mathf.Sin((rotated) * Mathf.Deg2Rad)));
            yield return null;
        }

        rotated = degreeToRotate;
        ourPos = transform.TransformPoint(radius * new Vector3(Mathf.Cos((degreeToRotate) * Mathf.Deg2Rad), Mathf.Sin((degreeToRotate) * Mathf.Deg2Rad)));
    }

}
