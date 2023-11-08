using UnityEngine;
using UnityEngine.Playables;

namespace LandRocker.TimelineTrack
{
    [System.Serializable]
    public class TMPBehaviour : PlayableBehaviour
    {   
        public string text = string.Empty;
        public Color color = Color.white;
        public bool isModifyingCharacterSpacing = false;
        public float characterSpacing = 0.0f;
        public float lineSpacing = 0.0f;
        public float paragraphSpacing = 0.0f;
        public float wordSpacing = 0.0f;


        #region [Used If Not Blending And Working With single playables]
        // public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        // {
        //     if (!DataSet){
        //         DataSet = true;
        //         tmpData = playerData as TextMeshProUGUI;
        //     }
        //     if(tmpData !=null)
        //     {
        //           tmpData.text = tmpData.text;
        //           var tempcolor = tmpData.color;
        //           tempcolor.a= info.weight;
        //         tmpData.color = tempcolor;
        //     }
    
        // }

        // public override void OnBehaviourPause(Playable playable, FrameData info)
        // {
        //     //DataSet =false;
        //     tmpData.text ="";
        // }
        #endregion
    }
}