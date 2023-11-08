using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

[System.Serializable]
public class ContentOnCircle
{
    public string content;
    public float angleInDeg;
    public Vector3 contentOffset;
    public float contentAngleInDeg;
    public Vector3 textRotInDeg;
    public TextAlign contentAlignment;
    public float contentFontSize;
    public int OnWhatCircleItsOn;
    public Color contentColor;
    public Vector3 colliderSize;
    public Vector3 colliderCenter;
    public Vector3 Text;
}

public class MenuBar01 : ImmediateModeShapeDrawer
{
    [Header("Camera")]
    [SerializeField] private Camera cameraForRendering;
    //[SerializeField] private Camera cameraForRaycasting;

    staticUICamera02_Rotated camRotatedProps;

    [Header("Rotation")]
    [SerializeField] private Vector3 transformRotation;

    [Header("Texts")]
    [SerializeField] private ContentOnCircle[] contents;
    public Vector3[] textRotInDegGoing;
    [Tooltip("It's Just for visualization")]
    [SerializeField] private GameObject[] TextGameObjects;
    [Tooltip("It's Just for visualization")]
    [SerializeField] private BoxCollider[] ColliderReferences;

    [Header("Circles")]
    [SerializeField] private int circleCounts;
    [SerializeField] private Color circleColor;

    [SerializeField] private float circleRadiusOffset;
    [SerializeField] private float dashSize;
    [SerializeField] private float dashSpacing;
    [SerializeField] private float dashOffset;

    [Header("Planets")]
    [SerializeField] private GameObject[] planets;

    [Header("Animating")]
    public bool isMenuBarOpened;
    public float animationLerpSpeed;

    private Quaternion circleRotationTemp = Quaternion.identity;
    private Vector3 transformRotationTemp;

    // Start is called before the first frame update
    void Start()
    {
        transformRotationTemp = transformRotation;

        TextGameObjects = new GameObject[contents.Length];
        ColliderReferences = new BoxCollider[contents.Length];
        for (int i = 0; i < contents.Length; i++)
        {
            TextGameObjects[i] = new GameObject();
            TextGameObjects[i].transform.parent = transform;
            TextGameObjects[i].name = "Texts GameObject_" + i;
            TextGameObjects[i].tag = "MenuBar_Options";
            ColliderReferences[i] = TextGameObjects[i].AddComponent<BoxCollider>();
            ColliderReferences[i].size = contents[i].colliderSize;
            ColliderReferences[i].center = contents[i].colliderCenter;
        }

        textRotInDegTemp = new Vector3[contents.Length];
        for (int i = 0; i < contents.Length; i++)
        {
            textRotInDegTemp[i] = contents[i].textRotInDeg;
        }

        camRotatedProps = cameraForRendering.GetComponent<staticUICamera02_Rotated>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = transformRotation;
        temp += transformRotationTemp * Time.deltaTime;
        transformRotation = temp;
        circleRotationTemp = Quaternion.Euler(transformRotation);

        for (int i = 0; i < planets.Length; i++)
        {
            planets[i].transform.RotateAround(transform.position, Vector3.forward, transformRotationTemp.z * Time.deltaTime);
        }

        if (camRotatedProps.isRotated)
        {
            for (int i = 0; i < contents.Length; i++)
            {
                contents[i].textRotInDeg = Vector3.Lerp(contents[i].textRotInDeg , textRotInDegGoing[i], animationLerpSpeed * Time.deltaTime);
            }
        }
        else
        {
            for (int i = 0; i < contents.Length; i++)
            {
                contents[i].textRotInDeg = Vector3.Lerp(contents[i].textRotInDeg, textRotInDegTemp[i], animationLerpSpeed * 2 * Time.deltaTime);
            }
        }

        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    Ray ray = cameraForRaycasting.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out RaycastHit hit))
        //    {
        //        print(hit.collider.gameObject.name);
        //    }
        //}
    }



    Vector3[] textRotInDegTemp;

    public override void DrawShapes(Camera cam)
    {
        base.DrawShapes(cam);

        Draw.ResetAllDrawStates();

        if (null == this.cameraForRendering) // only draw in the player camera
            return;

        using (Draw.Command(cam))
        {
            Draw.Matrix = transform.localToWorldMatrix;

            float circleRadiusOffsetIncremental = 0;
            for (int i = 0; i < circleCounts; i++)
            {
                circleRadiusOffsetIncremental += (i + 1) * circleRadiusOffset;
                Draw.UseDashes = true;
                Draw.DashStyle = DashStyle.MeterDashes(DashType.Rounded, dashSize, dashSpacing, DashSnapping.EndToEnd, dashOffset);
                Draw.Ring(Vector3.zero, circleRotationTemp, circleRadiusOffsetIncremental, circleColor);
            }

            circleRadiusOffsetIncremental = 0;
            for (int i = 0; i < circleCounts; i++)
            {
                circleRadiusOffsetIncremental += (i + 1) * circleRadiusOffset;
                for (int j = 0; j < contents.Length; j++)
                {
                    if ((i + 1) == contents[j].OnWhatCircleItsOn)
                        Draw.Text(circleRadiusOffsetIncremental * (new Vector3(Mathf.Cos(contents[j].angleInDeg * Mathf.Deg2Rad), Mathf.Sin(contents[j].angleInDeg * Mathf.Deg2Rad)) + contents[j].contentOffset), Quaternion.Euler(contents[j].textRotInDeg), contents[j].content, contents[j].contentAlignment, contents[j].contentFontSize, contents[j].contentColor);
                }
            }

            circleRadiusOffsetIncremental = 0;
            for (int i = 0; i < circleCounts; i++)
            {
                circleRadiusOffsetIncremental += (i + 1) * circleRadiusOffset;
                for (int j = 0; j < TextGameObjects.Length; j++)
                {
                    if ((i + 1) == contents[j].OnWhatCircleItsOn)
                        //Draw.Text(circleRadiusOffsetIncremental * (new Vector3(Mathf.Cos(contents[j].angleInDeg * Mathf.Deg2Rad), Mathf.Sin(contents[j].angleInDeg * Mathf.Deg2Rad)) + contents[j].contentOffset), contents[j].contentAngleInDeg * Mathf.Deg2Rad, contents[j].content, contents[j].contentAlignment, contents[j].contentFontSize, contents[j].contentColor);
                        TextGameObjects[j].transform.localPosition = circleRadiusOffsetIncremental * (new Vector3(Mathf.Cos(contents[j].angleInDeg * Mathf.Deg2Rad), Mathf.Sin(contents[j].angleInDeg * Mathf.Deg2Rad)) + contents[j].contentOffset);
                }
            }

            Draw.ResetMatrix();
            Draw.ResetAllDrawStates();
        }
    }
}
