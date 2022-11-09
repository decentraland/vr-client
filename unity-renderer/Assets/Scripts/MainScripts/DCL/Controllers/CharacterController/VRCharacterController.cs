using DCL.VR;
using UnityEngine;

public class VRCharacterController : MonoBehaviour
{
    [SerializeField]
    private Transform cameraParent;
    [SerializeField]
    private BooleanVariable menuOpen;
    
 //#if (UNITY_ANDROID && !UNITY_EDITOR)
//     private readonly Vector3 offset = new Vector3(0f, 0.55f, 0f);
// #else
    private readonly Vector3 offset = new Vector3(0f, -0.85f, 0f);
// #endif
    private Transform mixedRealityPlayspace;

    private void Start()
    {
        mixedRealityPlayspace = VRPlaySpace.i.transform;
        menuOpen.OnChange += MenuOpened;
        PlaceCamera();
    }
    private void MenuOpened(bool current, bool previous)
    {
        if (!current)
            return;
        PlaceCamera();
        menuOpen.OnChange -= MenuOpened;
    }

    private void PlaceCamera()
    {
        mixedRealityPlayspace.parent = cameraParent;
        mixedRealityPlayspace.localPosition = offset;
        // var canvas = GameObject.Find("Canvas");
        // #if UNITY_ANDROID && !UNITY_EDITOR
        // canvas.transform.localPosition += Vector3.down;
        // #endif
        mixedRealityPlayspace.localRotation = Quaternion.identity;
    }
}
