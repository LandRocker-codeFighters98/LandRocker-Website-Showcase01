using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelButton02 : MonoBehaviour
{
    [SerializeField] private Animator menuBarAnimator;
    private bool firstTimeClicked;

    public void OnCancelButton_Click()
    {
        if (!firstTimeClicked)
        {
            menuBarAnimator.Play("Close", 0, Mathf.Clamp01(1 - menuBarAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime));
            firstTimeClicked = true;
        }
    }

    private void OnEnable()
    {
        firstTimeClicked = false;
    }
}