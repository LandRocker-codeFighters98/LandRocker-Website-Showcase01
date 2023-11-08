using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton01 : MonoBehaviour
{
    [SerializeField] private Camera cameraForRendering;
    [SerializeField] private GameObject[] cameras;
    [SerializeField] private GameObject MenuCanvas;

    private Animator animator;
    private static readonly int isHovered = Animator.StringToHash("isHovered");

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

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (hit.collider.tag == "MenuButton")
                    {
                        cameras[0].SetActive(false);
                        cameras[1].SetActive(true);
                    }
                }
            }
            else
            {
                if (animator.GetBool(isHovered))
                    animator.SetBool(isHovered, false);
            }
        }
    }
}
