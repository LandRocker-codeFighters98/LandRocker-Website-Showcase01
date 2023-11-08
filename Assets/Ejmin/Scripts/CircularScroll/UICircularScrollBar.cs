using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICircularScrollBar : MonoBehaviour
{
    public Camera cameraRendering;

    [SerializeField] private RectTransform circleShape;
    [SerializeField] private RectTransform hidingPlaceUp;
    [SerializeField] private RectTransform hidingPlaceDown;
    [SerializeField] private float radius;
    [SerializeField] private float angleFixed;
    [SerializeField] private float angleDynamic;
    [SerializeField] private float angleForRotation;
    [SerializeField] private float textRotations;

    [Header("Slerp Values")]
    [SerializeField] private float SlerpScaler;

    [Header("Fractions")]
    [SerializeField] private float minFraction;
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

        if (textAngles == null)
        {
            textAngles = new float[texts.Length];
        }

        for (int i = 0; i < texts.Length; i++)
        {
            float tetha = angleFixed + ((i + 1) * angleDynamic);

            textAngles[i] = angleFixed + ((i + 1) * angleDynamic);

            texts[i].transform.position = circleShape.transform.position + (radius * new Vector3(Mathf.Cos(tetha * Mathf.Deg2Rad), Mathf.Sin(tetha * Mathf.Deg2Rad)));
        }

        minFraction = angleFixed;
        maxFraction = angleFixed + angleDynamic;
        textRotations = minFraction;
    }

    // Update is called once per frame
    void Update()
    {
        if (isScrolling)
        {
            //Scroll
            float input = Input.GetAxisRaw("Mouse ScrollWheel");
            float inputWithScrollScaler = -input * scrollScaler * Time.deltaTime;

            angleForRotation += inputWithScrollScaler;

            circleShape.localRotation = Quaternion.Euler(0, 0, angleForRotation);

            ourPos = circleShape.transform.position + (radius * new Vector3(Mathf.Cos((-180 + angleForRotation) * Mathf.Deg2Rad), Mathf.Sin((-180 + angleForRotation) * Mathf.Deg2Rad)));

            //For Fraction Rotation
            textRotations += inputWithScrollScaler;

            if (textRotations < minFraction)
            {
                textRotations = maxFraction;
                Vector3[] poses = new Vector3[texts.Length];
                for (int i = 0; i < texts.Length; i++)
                {
                    poses[i] = texts[i].transform.position;
                }

                StopAllCoroutines();
                for (int o = 0; o < poses.Length; o++)
                {
                    int y = o - 1 < 0 ? poses.Length - 1 : o - 1;
                    StartCoroutine(SlerpBackWard(y, poses));
                    StartCoroutine(SlerpBackWardUp(y, poses));
                    //texts[o].transform.position = Vector3.Slerp(texts[o].transform.position , poses[y] , 0.2f * Time.deltaTime);
                }
            }

            if (textRotations > maxFraction)
            {
                textRotations = minFraction;

                textRotations = minFraction;
                Vector3[] poses = new Vector3[texts.Length];
                for (int i = 0; i < texts.Length; i++)
                {
                    poses[i] = texts[i].transform.position;
                }

                StopAllCoroutines();
                for (int o = 0; o < poses.Length; o++)
                {
                    if (o != poses.Length - 2)
                    {
                        StartCoroutine(SlerpForward(o, poses));
                    }
                    else
                    {
                        StartCoroutine(SlerpForwardHide(o, poses));
                    }
                    //texts[o].transform.position = Vector3.Slerp(texts[o].transform.position, poses[(o + 1) % poses.Length], 0.2f * Time.deltaTime);
                }
            }

            gm_Debug.transform.position = ourPos;

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
