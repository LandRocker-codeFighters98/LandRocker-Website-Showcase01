using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton02 : MonoBehaviour
{
    [SerializeField] private Camera cameraForRendering;
    [SerializeField] private Animator menuBarAnimator;

    private Animator animator;
    private static readonly int isHovered = Animator.StringToHash("isHovered");
    private static readonly int isMenuBarOpened = Animator.StringToHash("isMenuBarOpened");

    void OnEnable()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraForRendering != null)
        {
            Ray ray = cameraForRendering.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.tag == "MenuButton")
                {
                    if (!animator.GetBool(isHovered))
                        animator.SetBool(isHovered, true);
                }
            }
            else
            {
                if (animator.GetBool(isHovered))
                    animator.SetBool(isHovered, false);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "MenuButton")
                    {
                        menuBarAnimator.Play("Open", 0);
                    }
                }
            }
        }
    }
}
