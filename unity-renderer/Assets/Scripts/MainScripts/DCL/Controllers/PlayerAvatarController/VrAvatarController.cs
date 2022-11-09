using System.Collections;
using DCL.VR;
using UnityEngine;

namespace DCL
{
    public class VrAvatarController : MonoBehaviour
    {
        [SerializeField]
        private PlayerAvatarController controller;
        
        private Transform cam;
        private Transform myTrans;
        
        private IEnumerator Start()
        {
            myTrans = transform;
            yield return null;
            controller.ApplyHideAvatarModifier();
            //DCLCharacterController.i.OnUpdateFinish += OnUpdateFinish;
        }
        
        private void OnUpdateFinish(float obj)
        {
            var pos = myTrans.position;
            var camPos = cam.position;
            if (pos != camPos)
                myTrans.position = cam.position;
        }
    }
}