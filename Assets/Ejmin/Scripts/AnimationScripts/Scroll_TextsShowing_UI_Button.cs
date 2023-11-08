using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll_TextsShowing_UI_Button : MonoBehaviour
{
    [SerializeField] private Camera cameraForRendering;
    [SerializeField] private Animator animator;
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private UICircularScrollBar04 uiCircular;
    [SerializeField] private float fadingInOutTime;

    private bool isButtonClick = true;

    IEnumerator coroutine;

    private static readonly int ButtonClickId = Animator.StringToHash("ButtonClick");

    // Update is called once per frame
    void Update()
    {
        if (cameraForRendering != null)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = cameraForRendering.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.tag == "UIButtonClick")
                    {
                        isButtonClick = !animator.GetBool(ButtonClickId);
                        animator.SetBool(ButtonClickId, isButtonClick);
                    }
                }
            }
        }

        if (isButtonClick)
        {
            if (!cg.gameObject.activeSelf)
            {
                cg.gameObject.SetActive(true);
                uiCircular.showingTextBegin = true;
            }
            cg.alpha = Mathf.Lerp(cg.alpha, 1, fadingInOutTime * Time.deltaTime);
        }
        else
        {
            cg.alpha = Mathf.Lerp(cg.alpha, 0, fadingInOutTime * Time.deltaTime);
        }

        if (cg.alpha - 0.1f < 0)
        {

            if (cg.gameObject.activeSelf)
            {
                uiCircular.StopAllCoroutines();
                uiCircular.showingTextBegin = false;
                cg.gameObject.SetActive(false);
            }
        }
    }
}
