using UnityEngine;
using Shapes;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class CirclesConnectionsWithText : CirclesConnection
{
    [Header("Intro Texts")]
    [SerializeField] private string[] introTexts;
    [SerializeField] private Vector3[] introTextsOffset;
    [SerializeField] private float introTextsFontSize;
    [SerializeField] private Color introTextsColor;
    [SerializeField] private TextAlign introTextsAlign;

    [Header("Intro Texts Lerping")]
    [SerializeField] private float colorLerpingScaler;
    [SerializeField] private float colorInverseLerpingScaler;

    [Header("Mouse Hover")]
    [SerializeField] private float hoverRadius;

    private bool[] isShown;

    public override void DrawShapes(Camera cam)
    {
        base.DrawShapes(cam);

        if (null != this.renderingCamera) // only draw in the player camera
            return;

        using (Draw.Command(cam))
        {
            Draw.ResetAllDrawStates();
            Draw.Matrix = transform.localToWorldMatrix;
            for (int i = 0; i < localPoints.Count; i++)
            {
                if (introTexts.Length > 0 && introTextsOffset.Length > 0)
                    Draw.Text(localPoints[i] + introTextsOffset[i % introTextsOffset.Length], Quaternion.Euler(localRotations[i]), introTexts[i % introTexts.Length], introTextsAlign, introTextsFontSize, introTextsColor);
            }
            Draw.ResetAllDrawStates();
        }
    }

    protected override void Start()
    {
        base.Start();

        isShown = new bool[introTexts.Length];
    }

    protected override void Update()
    {
        base.Update();

        if (localPoints.Count != isShown.Length)
        {
            isShown = new bool[localPoints.Count];
        }
    }


#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
