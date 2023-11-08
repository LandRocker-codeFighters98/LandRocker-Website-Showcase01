using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class MenuBar02 : ImmediateModeShapeDrawer
{
    [Header("Camera")]
    [SerializeField] private Camera cameraForRendering;
    //[SerializeField] private Camera cameraForRaycasting;

    staticUICamera02_Rotated camRotatedProps;

    [Header("Rotation")]
    [SerializeField] private Vector3 transformRotation;

    [Header("Circles")]
    [SerializeField] private int circleCounts;
    [SerializeField] private Color circleColor;

    [SerializeField] private float circleRadiusOffset;
    [SerializeField] private float dashSize;
    [SerializeField] private float dashSpacing;
    [SerializeField] private float dashOffset;

    [Header("Planets")]
    [SerializeField] private GameObject[] planets;

    private Quaternion circleRotationTemp = Quaternion.identity;
    private Vector3 transformRotationTemp;

    // Start is called before the first frame update
    void Start()
    {
        transformRotationTemp = transformRotation;

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
    }


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

            Draw.ResetMatrix();
            Draw.ResetAllDrawStates();
        }
    }
}
