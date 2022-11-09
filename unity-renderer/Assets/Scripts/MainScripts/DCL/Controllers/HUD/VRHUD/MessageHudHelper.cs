using DCL.Huds;
using UnityEngine;

public class MessageHudHelper : VRHUDHelper
{
    protected override void SetupHelper()
    {
        VRHUDController.LoadingStart += HideMessage;
    }
    
    private void HideMessage()
    {
        gameObject.SetActive(false);
        VRHUDController.LoadingStart -= HideMessage;
    }
}
