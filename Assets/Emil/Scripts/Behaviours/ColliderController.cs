using UnityEngine;
using UnityEngine.Playables;

namespace LandRocker.VFX
{
    public class ColliderController : MonoBehaviour
    {
        [SerializeField] protected Camera camRenderingVFX;
        [SerializeField] protected PlayableDirector introPlayable;
        protected bool hasClicked;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !hasClicked && introPlayable.state == PlayState.Paused)
            {
                Ray ray = camRenderingVFX.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if(hit.collider.tag == "Clickable")
                    {
                        hasClicked = true;
                        introPlayable.Play();
                    }
                }
            }
        }


    }
}