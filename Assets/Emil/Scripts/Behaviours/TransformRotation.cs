using UnityEngine;

namespace LandRocker.Behaviours
{
    public class TransformRotation : MonoBehaviour
    {
        [SerializeField] public float rotationSpeed = 5f;
        [SerializeField] public Vector3 rotationAxis;
        public void Update()
        {
            transform.localRotation *= Quaternion.Euler(rotationAxis * rotationSpeed * Time.deltaTime);
        }
    }
}