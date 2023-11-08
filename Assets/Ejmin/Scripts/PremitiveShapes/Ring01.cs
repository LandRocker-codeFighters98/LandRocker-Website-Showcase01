using UnityEngine;
using Shapes;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class Ring01 : ImmediateModeShapeDrawer
{
    [SerializeField] private Camera cameraForRendering;

    [Header("Rings Props")]
    [SerializeField] private Color ringColor;
    [SerializeField] private Vector3 ringOffset;
    [SerializeField] private float ringRadius;
    [SerializeField] private float ringThickness;

    public override void DrawShapes(Camera cam)
    {
        if (null != this.cameraForRendering) // only draw in the player camera
            return;

        using (Draw.Command(cam))
        {
            Draw.ResetAllDrawStates();
            Draw.Matrix = transform.localToWorldMatrix;

            Draw.Ring(Vector3.zero + ringOffset, ringRadius, ringThickness, ringColor);

            Draw.ResetMatrix();
            Draw.ResetAllDrawStates();
        }
    }
}
