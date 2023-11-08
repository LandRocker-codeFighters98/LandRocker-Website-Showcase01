using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using TMPro;

public class TextsAppearing_Manager : ImmediateModeShapeDrawer
{
    [SerializeField] private Camera cameraRendering;

    [SerializeField] private GameObject[] introTexts;
    [SerializeField] private UIScroll07_Ejmin uiScroll;

    [Header("Texts Passing Props")]
    [SerializeField] private bool[] isPassed;
    [SerializeField] private Vector3 offsetTextFromOrigin;
    [SerializeField] private Vector3 offsetToNotShow;

    [Header("Intro Texts")]
    [SerializeField] private string[] introTexts_Contents;
    [SerializeField] private Vector3 introTextsOffset;
    [SerializeField] private float introTextsFontSize;
    [SerializeField] private float introTextsAngle;
    [SerializeField] private Color introTextsColor;
    [SerializeField] private TextAlign introTextsAlign;
    [SerializeField] private TMP_FontAsset introFontAsset;

    protected virtual void Start()
    {
        isPassed = new bool[introTexts.Length];
    }

    protected virtual void Update()
    {
        for (int i = 0; i < introTexts.Length; i++)
        {
            if (isPassed[i])
            {
                introTexts[i].transform.position = Vector3.Lerp(introTexts[i].transform.position, transform.position, 5);
            }

            if (!isPassed[i])
            {
                introTexts[i].transform.position = Vector3.Lerp(introTexts[i].transform.position, transform.position + offsetToNotShow, 5);
            }
        }
    }

    public void TextIsPassed(int index)
    {
        if (index >= introTexts.Length)
            return;

        isPassed[index] = true;
        introTexts[index].transform.position = transform.position - offsetTextFromOrigin;
        //introTexts[index].gameObject.SetActive(true);
    }

    public void TextNotPassed(int index)
    {
        if (index >= introTexts.Length)
            return;

        isPassed[index] = false;
    }

    public override void DrawShapes(Camera cam)
    {
        base.DrawShapes(cam);

        if (cameraRendering != null)
        {
            if (cam != this.cameraRendering) // only draw in the player camera
                return;
        }

        using (Draw.Command(cam))
        {
            for (int i = 0; i < introTexts.Length; i++)
            {
                Draw.Matrix = introTexts[i].transform.localToWorldMatrix;

                if (isPassed[i])
                {
                    Vector3 pos = introTexts[i].transform.localPosition;
                    pos = Vector3.Lerp(pos , introTextsOffset + pos , 0.5f * Time.deltaTime);
                    Draw.Text(Vector3.zero + introTextsOffset, introTextsAngle, introTexts_Contents[i], introTextsAlign, introTextsFontSize, introFontAsset, introTextsColor);
                }

                if (!isPassed[i])
                {
                    Vector3 pos = introTexts[i].transform.localPosition;
                    pos = Vector3.Lerp(pos, offsetToNotShow + pos, 0.5f * Time.deltaTime);
                }
            }

            Draw.ResetAllDrawStates();
            Draw.ResetMatrix();
        }
    }
}
