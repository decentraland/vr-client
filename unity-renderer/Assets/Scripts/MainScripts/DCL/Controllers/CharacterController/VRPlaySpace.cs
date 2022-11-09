using UnityEngine;

namespace DCL.VR
{
    public class VRPlaySpace : MonoBehaviour
    {
        public static VRPlaySpace i;

        [SerializeField]
        private GameObject cameraObject;
        
        private void Awake()
        {
            i = this;
        }
        
        public Camera GetCamera()
        {
            return cameraObject.GetComponent<Camera>();
        }

        public void SetCameraInactive()
        {
            cameraObject.SetActive(false);
        }
    }
}
