using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAndScrollDown : MonoBehaviour
{
    bool firstTime;
    // Update is called once per frame
    void Update()
    {
        if (!firstTime)
        {
            float input = Input.GetAxisRaw("Mouse ScrollWheel");

            if (input != 0)
            {
                GetComponent<Animator>().SetBool("isScrolling", true);
                firstTime = true;
            }
        }
    }
}
