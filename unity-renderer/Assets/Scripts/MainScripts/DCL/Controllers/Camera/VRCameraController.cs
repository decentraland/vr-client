using System;
using DCL.CameraTool;
using UnityEngine;

namespace DCL.Camera
{
    public class VRCameraController : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.Camera cam;
        [SerializeField]
        private CameraController controller;
        [SerializeField]
        private Transform targetToFollow;
        [SerializeField]
        private float secondsBetweenTurns = .5f;
        [SerializeField]
        private float rotation = 45f;

        [SerializeField]
        private InputAction_Trigger cameraChangeAction;
        [SerializeField]
        private InputAction_Measurable cameraX;
        [SerializeField]
        private InputAction_Measurable characterX;
        [SerializeField]
        private InputAction_Measurable characterY;

        private DateTime lastTurned;

        private readonly Vector3 offset = new Vector3(0f, -1f, 0f);
        private Transform myTrans;
        private Transform camTrans;

        private bool smoothRotate;

        void Start()
        {
            myTrans = transform;
            camTrans = UnityEngine.Camera.main.transform;
            
            CommonScriptableObjects.cameraBlocked.OnChange += CameraBlockedOnchange;
            controller.SetCameraMode(CameraMode.ModeId.FirstPerson);
            cameraChangeAction.isTriggerBlocked = CommonScriptableObjects.cameraBlocked;
            CommonScriptableObjects.cameraBlocked.Set(true);

            DCLCharacterController.i.OnUpdateFinish += FollowCharacter;
            cameraX.OnValueChanged += RotateCamera;
        }

        private void Update()
        {
            DisableCamera();
            if (!camTrans.hasChanged)
                return;
            CommonScriptableObjects.cameraForward.Set(camTrans.forward);
            CommonScriptableObjects.cameraRight.Set(camTrans.right);
            CommonScriptableObjects.cameraPosition.Set(camTrans.position);
        }
        
        private void DisableCamera()
        {
            if (!cam.enabled)
                return;
            cam.enabled = false;
        }

        private void RotateCamera(DCLAction_Measurable action, float value)
        {
            if (smoothRotate)
            {
                SmoothRotate(value);
                return;
            } 
            SnapRotate(value);
        }

        private void SnapRotate(float value)
        {
            if ((DateTime.Now - lastTurned).TotalSeconds < secondsBetweenTurns) return;
            var yValue = value >= 0 ? rotation : -rotation;
            myTrans.eulerAngles += new Vector3(0f, yValue, 0f);
            lastTurned = DateTime.Now;
        }

        private void SmoothRotate(float value)
        {
            myTrans.eulerAngles += new Vector3(0f, value * rotation * Time.deltaTime, 0f);
        }

        private void FollowCharacter(float deltaTime)
        {
            myTrans.position = targetToFollow.position;// + offset;
        }

        private void CameraBlockedOnchange(bool current, bool previous)
        {
            if (current == false)
                CommonScriptableObjects.cameraBlocked.Set(true);
        }

        public void SetSmoothRotate(bool value) => smoothRotate = value;
    }
}