using UnityEngine;
using Shapes;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class CirclesConnectionsWithTextShowingAndBezierCurve : CirclesConnection
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
    private Color[] introColors;

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
                    Draw.Text(localPoints[i] + introTextsOffset[i % introTextsOffset.Length], Quaternion.Euler(localRotations[i]), introTexts[i % introTexts.Length], introTextsAlign, introTextsFontSize, introColors[i % introColors.Length]);
            }
            Draw.ResetAllDrawStates();
        }
    }

    protected override void Start()
    {
        base.Start();

        isShown = new bool[introTexts.Length];

        introColors = new Color[localPoints.Count];

        for (int i = 0; i < introColors.Length; i++)
        {
            introColors[i] = new Color(introTextsColor.r, introTextsColor.g, introTextsColor.b, 0);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (localPoints.Count != isShown.Length)
        {
            isShown = new bool[localPoints.Count];
        }

        if (localPoints.Count != introColors.Length)
        {
            introColors = new Color[localPoints.Count];

            for (int i = 0; i < introColors.Length; i++)
            {
                introColors[i] = new Color(introTextsColor.r, introTextsColor.g, introTextsColor.b, 0);
            }
        }


        if (renderingCamera != null)
        {
            for (int i = 0; i < localPoints.Count; i++)
            {
                Vector3 input = Input.mousePosition;
                input.z = transform.position.z;
                Vector3 mousePointInWorld = renderingCamera.ScreenToWorldPoint(input);
                Vector3 localInput = transform.InverseTransformPoint(mousePointInWorld);
                if (localInput.x < localPoints[i].x + hoverRadius &&
                    localInput.x > localPoints[i].x - hoverRadius &&
                    localInput.y < localPoints[i].y + hoverRadius &&
                    localInput.y > localPoints[i].y - hoverRadius)
                {
                    isShown[i] = true;
                }
                else
                {
                    isShown[i] = false;
                }

                if (isShown[i])
                {
                    introColors[i] = Color.Lerp(new Color(introColors[i].r, introColors[i].g, introColors[i].b, introColors[i].a), new Color(introColors[i].r, introColors[i].g, introColors[i].b, 1), colorLerpingScaler * Time.deltaTime);
                }
                else
                {
                    introColors[i] = Color.Lerp(new Color(introColors[i].r, introColors[i].g, introColors[i].b, introColors[i].a), new Color(introColors[i].r, introColors[i].g, introColors[i].b, 0), colorInverseLerpingScaler * Time.deltaTime);
                }
            }
        }
    }

    #if UNTIY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
