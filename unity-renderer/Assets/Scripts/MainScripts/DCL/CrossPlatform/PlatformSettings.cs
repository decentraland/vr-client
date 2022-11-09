using UnityEngine;
using UnityEngine.XR.Management;

[CreateAssetMenu(fileName = "PlatformSettings", menuName = "CrossPlatform/PlatformSettings")]
public class PlatformSettings : ScriptableObject
{
    [SerializeField]
    private string nonVRController = "Player";
    public string NonVRController => nonVRController;

    [SerializeField]
    private string vrControllerName;
    public string VRController => vrControllerName;
}
