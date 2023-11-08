using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
class PolyLine01 : ImmediateModeShapeDrawer
{
    [SerializeField] private Camera cameraForRendering;

    [Header("PolyLine Props")]
    [SerializeField] private Vector3[] polyLinePath;
    [SerializeField] private PolylinePath path;
    [SerializeField] private bool isClosed;
    [SerializeField] private PolylineJoins polylineJoins;

    [SerializeField] private Color polyLineColor;
    [SerializeField] private float polyLineThickness;

    [SerializeField] private Vector3 offsetOnPolyLines;

    public override void DrawShapes(Camera cam)
    {
        if (null != this.cameraForRendering) // only draw in the player camera
            return;

        using (Draw.Command(cam))
        {
            Draw.ResetAllDrawStates();
            Draw.Matrix = transform.localToWorldMatrix;

            Draw.Polyline(path, isClosed, polyLineThickness, polylineJoins, polyLineColor);

            Draw.ResetMatrix();
            Draw.ResetAllDrawStates();
        }
    }
}
