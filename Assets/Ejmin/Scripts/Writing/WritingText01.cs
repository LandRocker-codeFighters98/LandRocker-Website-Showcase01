using UnityEngine;
using Shapes;
using TMPro;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class WritingText01 : ImmediateModeShapeDrawer
{
    [SerializeField] private Camera cameraForRendering;

    [Header("Text")]
    [SerializeField] private Color textColor;
    [SerializeField] private Vector3 textRotationInDegree;
    [SerializeField] private string text;
    [SerializeField] private float textFontSize;
    [SerializeField] private Vector3 textOffset;
    [SerializeField] private TextAlign textAlign;
    [SerializeField] private TMP_FontAsset fontAsset;

    public override void DrawShapes(Camera cam)
    {
        if (null != this.cameraForRendering) // only draw in the player camera
            return;

        using (Draw.Command(cam))
        {
            Draw.ResetAllDrawStates();
            Draw.Matrix = transform.localToWorldMatrix;

            Draw.Text(textOffset, Quaternion.Euler(textRotationInDegree) , text, textAlign, textFontSize, fontAsset, textColor);

            Draw.ResetMatrix();
            Draw.ResetAllDrawStates();
        }
    }
}
