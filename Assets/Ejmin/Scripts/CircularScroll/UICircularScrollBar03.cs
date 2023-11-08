using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextProps02
{
    public int id;
    public string content;
    public GameObject text;
}

public class UICircularScrollBar03 : MonoBehaviour
{
    public Camera cameraRendering;

    [Header("Middle Circle Props")]
    [SerializeField] private RectTransform circleShape;
    [SerializeField] private RectTransform UIScrollPanelForParenting;

    [Header("Texts Placements")]
    [SerializeField] private RectTransform[] textPlaces;

    [Header("Hiding Places")]
    [SerializeField] private RectTransform hidingPlaceUp;
    [SerializeField] private RectTransform hidingPlaceForAllToNotShown;
    [SerializeField] private RectTransform hidingPlaceDown;

    [Header("Texts GameObjects")]
    [SerializeField] private string[] textContents;
    [SerializeField] private float fontSize;
    [SerializeField] private int shownTextsCount;
    private List<TextProps02> textsGameObjects;

    [Header("General Props")]
    [SerializeField] private float radius;
    [SerializeField] private float angleFixed;
    [SerializeField] private float angleDynamic;

    [SerializeField] private float angleForRotation;
    [SerializeField] private float cycleFraction;

    [Header("Slerp Values")]
    [SerializeField] private float SlerpScaler;

    [Header("Fractions")]
    [SerializeField] private float minFraction;
    [SerializeField] private float ourFraction;
    [SerializeField] private float maxFraction;

    [Header("Scroll Props")]
    [SerializeField] private float scrollScaler;

    [Header("Director Props")]
    [SerializeField] private bool isScrolling;

    [Header("Texts")]
    [SerializeField] private Text[] texts;
    private float[] textAngles;

    public bool[] isPassedQuick;

    public Vector3 ourPos = Vector3.zero;
    public Vector3 ourPosFraction = Vector3.zero;
    public Vector3 ourPosFractionMin = Vector3.zero;
    public Vector3 ourPosFractionMax = Vector3.zero;
    public Vector3 initialPos = Vector3.zero;
    public Vector3 endPos = Vector3.zero;

    private GameObject gm_Debug;

    // Start is called before the first frame update
    void Start()
    {
        ourPos = circleShape.transform.position + radius * new Vector3(Mathf.Cos(-180 * Mathf.Deg2Rad), Mathf.Sin(-180 * Mathf.Deg2Rad));
        gm_Debug = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        if (textsGameObjects == null)
        {
            textsGameObjects = new List<TextProps02>();
        }

        for (int i = 0; i < textContents.Length; i++)
        {
            textsGameObjects.Add(new TextProps02());
            textsGameObjects[i].id = i;
            textsGameObjects[i].content = textContents[i];

            textsGameObjects[i].text = new GameObject("textGameObject_" + i);
            textsGameObjects[i].text.transform.parent = UIScrollPanelForParenting.transform;

            TextMeshPro textMeshPro = textsGameObjects[i].text.AddComponent<TextMeshPro>();
            textMeshPro.text = textsGameObjects[i].content;
            textMeshPro.fontSize = fontSize;
            textMeshPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textMeshPro.verticalAlignment = VerticalAlignmentOptions.Middle;
            textMeshPro.overflowMode = TextOverflowModes.Overflow;

            textsGameObjects[i].text.transform.position = hidingPlaceForAllToNotShown.transform.position;
        }

        for (int i = 0; i < shownTextsCount; i++)
        {
            float tetha = 90 + ((i + 1) * 45);

            textPlaces[i].transform.position = circleShape.transform.position + (radius * new Vector3(Mathf.Cos((tetha) * Mathf.Deg2Rad), Mathf.Sin((tetha) * Mathf.Deg2Rad)));
        }

        hidingPlaceUp.transform.position = circleShape.transform.position + (radius * new Vector3(Mathf.Cos((90) * Mathf.Deg2Rad), Mathf.Sin((90) * Mathf.Deg2Rad)));
        hidingPlaceDown.transform.position = circleShape.transform.position + (radius * new Vector3(Mathf.Cos((270) * Mathf.Deg2Rad), Mathf.Sin((270) * Mathf.Deg2Rad)));
        hidingPlaceForAllToNotShown.transform.position = circleShape.transform.position + (radius * new Vector3(Mathf.Cos((0) * Mathf.Deg2Rad), Mathf.Sin((0) * Mathf.Deg2Rad)));

        for (int i = 0; i < textContents.Length; i++)
        {
            if (i == 0)
            {
                textsGameObjects[i].text.transform.position = textPlaces[1].transform.position;
            }
            else if (i == 1)
            {
                textsGameObjects[i].text.transform.position = textPlaces[2].transform.position;
            }
            else if (i == 2)
            {
                textsGameObjects[i].text.transform.position = hidingPlaceDown.transform.position;
            }
            else
            {
                textsGameObjects[i].text.transform.position = hidingPlaceForAllToNotShown.transform.position;
            }
        }

        minFraction = 90 + 45;
        maxFraction = 90 + 90;
        ourFraction = 180;
        angleForRotation = 180;
    }

    //bool cycleCompletedStart;
    //bool cycleCompletedEnd;
    //[SerializeField]
    //float index;

    public int index = 0;

    //Update is called once per frame
    void Update()
    {
        if (isScrolling)
        {
            //Scroll
            float input = Input.GetAxisRaw("Mouse ScrollWheel");
            float inputWithScrollScaler = -input * scrollScaler * Time.deltaTime;

            if (index <= 0)
            {
                index = 0;
                if (inputWithScrollScaler < 0)
                {
                    inputWithScrollScaler = 0;
                    angleForRotation = 180;
                    ourFraction = minFraction;
                }
            }
            else if (index > textContents.Length)
            {
                index = textContents.Length;
                if (inputWithScrollScaler > 0)
                {
                    inputWithScrollScaler = 0;
                    angleForRotation = 180 + (45 * textContents.Length);
                    ourFraction = maxFraction;
                }
            }

            angleForRotation += inputWithScrollScaler;

            circleShape.localRotation = Quaternion.Euler(0, 0, angleForRotation);

            ourPos = circleShape.transform.position + (radius * new Vector3(Mathf.Cos((angleForRotation) * Mathf.Deg2Rad), Mathf.Sin((angleForRotation) * Mathf.Deg2Rad)));

            gm_Debug.transform.position = ourPos;

            //For Fraction Rotation
            ourFraction += inputWithScrollScaler;

            if (ourFraction < minFraction)
            {
                --index;
                ourFraction = maxFraction;

                //Vector3[] tempTextPoses = new Vector3[textsGameObjects.Count];
                //for (int i = 0; i < textsGameObjects.Count; i++)
                //{
                //    tempTextPoses[i] = textsGameObjects[i].text.transform.position;
                //}

                //StopAllCoroutines();
                //for (int o = 0; o < tempTextPoses.Length; o++)
                //{
                //    int y = o - 1 < 0 ? tempTextPoses.Length - 1 : o - 1;
                //    StartCoroutine(SlerpBackWard(y, tempTextPoses));
                //    StartCoroutine(SlerpBackWardUp(y, tempTextPoses));
                //    //texts[o].transform.position = Vector3.Slerp(texts[o].transform.position , poses[y] , 0.2f * Time.deltaTime);
                //}
            }

            if (ourFraction > maxFraction)
            {
                ++index;
                ourFraction = minFraction;

                //Vector3[] poses = new Vector3[texts.Length];
                //for (int i = 0; i < texts.Length; i++)
                //{
                //    poses[i] = texts[i].transform.position;
                //}

                //StopAllCoroutines();
                //for (int o = 0; o < poses.Length; o++)
                //{
                //    if (o != poses.Length - 2)
                //    {
                //        StartCoroutine(SlerpForward(o, poses));
                //    }
                //    else
                //    {
                //        StartCoroutine(SlerpForwardHide(o, poses));
                //    }
                //    //texts[o].transform.position = Vector3.Slerp(texts[o].transform.position, poses[(o + 1) % poses.Length], 0.2f * Time.deltaTime);
                //}
            }

            for (int i = 0; i < texts.Length; i++)
            {
                //Getting the angle on ourPos **** Start
                Vector3 dir = (ourPos - circleShape.transform.position).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                if (angle < 0)
                {
                    angle += 360;
                }
                //Getting the angle on ourPos **** End

                //Getting the text angle on ourPos **** Start
                for (int j = 0; j < textAngles.Length; j++)
                {
                    Vector3 textDir = (texts[j].transform.position - circleShape.transform.position).normalized;
                    float textAngle = Mathf.Atan2(textDir.y, textDir.x) * Mathf.Rad2Deg;

                    if (textAngle < 0)
                    {
                        textAngle += 360;
                    }
                    textAngles[i] = textAngle;
                }
                //Getting the text angle on ourPos **** End

                //if (textRotations >  && angle < textAngles[(i + 1) % textAngles.Length])
                //{
                //    Vector3[] poses = new Vector3[texts.Length];
                //    for (int o = 0; o < poses.Length; o++)
                //    {
                //        poses[o] = texts[o].transform.position;
                //    }

                //    for (int o = 0; o < poses.Length; o++)
                //    {
                //        texts[o].transform.position = poses[(o + 1) % poses.Length];
                //    }
                //}
            }
        }
    }

    private IEnumerator SlerpForward(int index, Vector3[] poses)
    {
        while ((texts[index].transform.position - poses[(index + 1) % poses.Length]).magnitude > 0.1f)
        {
            texts[index].transform.position = Vector3.Slerp(texts[index].transform.position, poses[(index + 1) % poses.Length], SlerpScaler * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator SlerpForwardHide(int index, Vector3[] poses)
    {
        while ((texts[index].transform.position - hidingPlaceDown.transform.position).magnitude > 0.1f)
        {
            texts[index].transform.position = Vector3.Slerp(texts[index].transform.position, hidingPlaceDown.transform.position, SlerpScaler * Time.deltaTime);
            yield return null;
        }

        texts[index].transform.position = hidingPlaceUp.transform.position;

        while ((texts[index].transform.position - poses[(index + 1) % poses.Length]).magnitude > 0.1f)
        {
            texts[index].transform.position = Vector3.Slerp(texts[index].transform.position, poses[(index + 1) % poses.Length], SlerpScaler * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator SlerpBackWard(int index, Vector3[] poses)
    {
        if (index != 0)
        {
            while ((texts[index].transform.position - poses[(index + 1) % poses.Length]).magnitude > 0.1f)
            {
                texts[index].transform.position = Vector3.Slerp(texts[index].transform.position, poses[(index + 1) % poses.Length], SlerpScaler * Time.deltaTime);
                yield return null;
            }
        }
    }

    private IEnumerator SlerpBackWardUp(int index, Vector3[] poses)
    {
        if (index == 0)
        {
            while ((texts[index].transform.position - hidingPlaceUp.transform.position).magnitude > 0.1f)
            {
                texts[index].transform.position = Vector3.Slerp(texts[index].transform.position, hidingPlaceUp.transform.position, SlerpScaler * Time.deltaTime);
                yield return null;
            }

            texts[index].transform.position = hidingPlaceDown.transform.position;

            while ((texts[index].transform.position - poses[(index + 1) % poses.Length]).magnitude > 0.1f)
            {
                texts[index].transform.position = Vector3.Slerp(texts[index].transform.position, poses[(index + 1) % poses.Length], SlerpScaler * Time.deltaTime);
                yield return null;
            }
        }
    }
}
