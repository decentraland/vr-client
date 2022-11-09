using UnityEngine;

public class DisableEventSystem : MonoBehaviour
{
    private void Awake()
    {
        if (CrossPlatformManager.IsVRPlatform()) gameObject.SetActive(false);
    }
}
