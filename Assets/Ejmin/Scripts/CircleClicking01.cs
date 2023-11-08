using UnityEngine;
using Shapes;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class CircleClicking01 : ImmediateModeShapeDrawer
{
    [SerializeField] private Camera cameraForRendering;

    [Header("For Animating")]
    [Range(0, 1)] [SerializeField] private float colorAlpha;
    private float colorAlphaTemp;

    [Header("Rings Props")]
    [SerializeField] private Color innerRingInitialColor;
    [SerializeField] private Color outerRingInitialColor;
    [SerializeField] private float innerRingRadius;
    [SerializeField] private float outerRingRadius;
    [SerializeField] private float innerRingThickness;
    [SerializeField] private float outerRingThickness;

    [Header("Line Props")]
    [SerializeField] private Color lineColor;
    [SerializeField] private float lineThickness;
    [SerializeField] private LineEndCap lineEndCap;

    [SerializeField] private Vector3 offsetLine01;
    [SerializeField] private Vector3 offsetLine02;

    [Header("Text")]
    [SerializeField] private Color textColor;
    [SerializeField] private string text;
    [SerializeField] private float textFontSize;
    [SerializeField] private Vector3 textOffset;
    [SerializeField] private TextAlign textAlign;

    private Color innerColor;
    private Color outerColor;
    private Color textColorTemp;
    private Color lineColorTemp;

    [Header("Clicking")]
    [SerializeField] private float clickRadius;

#if UNITY_EDITOR
    private void OnValidate()
    {
        innerColor = new Color(innerRingInitialColor.r, innerRingInitialColor.g, innerRingInitialColor.b, colorAlphaTemp);
        outerColor = new Color(outerRingInitialColor.r, outerRingInitialColor.g, outerRingInitialColor.b, colorAlphaTemp);
        textColorTemp = new Color(textColor.r, textColor.g, textColor.b, colorAlphaTemp);
    }
#endif

    private void Update()
    {
        if (colorAlphaTemp != colorAlpha)
        {
            colorAlphaTemp = colorAlpha;
            innerColor = new Color(innerRingInitialColor.r, innerRingInitialColor.g, innerRingInitialColor.b, colorAlphaTemp);
            outerColor = new Color(outerRingInitialColor.r, outerRingInitialColor.g, outerRingInitialColor.b, colorAlphaTemp);
            textColorTemp = new Color(textColor.r, textColor.g, textColor.b, colorAlphaTemp);
            lineColorTemp = new Color(lineColor.r, lineColor.g, lineColor.b, colorAlphaTemp);
        }

        if (cameraForRendering != null)
        {
            Vector3 input = Input.mousePosition;
            input.z = (cameraForRendering.transform.position - transform.position).magnitude;
            Vector3 mousePointInWorld = cameraForRendering.ScreenToWorldPoint(input);
            Vector3 localInput = transform.InverseTransformPoint(mousePointInWorld);
            if (localInput.x < clickRadius &&
                localInput.x > clickRadius &&
                localInput.y < clickRadius &&
                localInput.y > clickRadius)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    print("clicked");
                }
            }
        }
    }

    public override void DrawShapes(Camera cam)
    {
        if (null != this.cameraForRendering) // only draw in the player camera
            return;

        using (Draw.Command(cam))
        {
            Draw.ResetAllDrawStates();
            Draw.ResetMatrix();
            Draw.Matrix = transform.localToWorldMatrix;

            Draw.Ring(Vector3.zero, innerRingRadius, innerRingThickness, innerColor);
            Draw.Ring(Vector3.zero, outerRingRadius, outerRingThickness, outerColor);

            Draw.Text(Vector3.zero + textOffset, text, textAlign, textFontSize, textColorTemp);
            Draw.Line(offsetLine01, offsetLine02 , lineThickness , lineEndCap , lineColorTemp);

            Draw.ResetMatrix();
            Draw.ResetAllDrawStates();
        }
    }
}
