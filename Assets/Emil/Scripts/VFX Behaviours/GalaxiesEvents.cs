using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace LandRocker.VFX
{
    public class GalaxiesEvents : MonoBehaviour
    {
        [SerializeField] public VisualEffect VFX;
        [SerializeField] protected ExposedProperty isActiveProperty = "IsActive";
        public void SendEvent(string nameOfTheEvent)
        {
            bool activate = false;
            string[] words = nameOfTheEvent.Split(new char[] { '_' });
            switch (words[0])
            {
                case string s when s.ToLower().StartsWith("p"):
                    activate = true;
                    break;

                case string s when s.ToLower().StartsWith("s"):
                    activate = false;
                    break;

                default: 
                    break;
            }

            VFX.SetBool(isActiveProperty, activate);
            VFX.SendEvent(nameOfTheEvent);
        }
    }
}