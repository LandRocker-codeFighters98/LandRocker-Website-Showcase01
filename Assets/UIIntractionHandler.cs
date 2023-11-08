using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIIntractionHandler : MonoBehaviour
{
    [SerializeField] private Camera cameraForRaycasting;

    // Update is called once per frame
    void Update()
    {
        Ray ray = cameraForRaycasting.ScreenPointToRay(Input.mousePosition);
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                print(hit.collider.name);
            }
        }
    }
}
