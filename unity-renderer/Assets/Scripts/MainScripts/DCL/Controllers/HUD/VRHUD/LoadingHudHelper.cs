using DCL;
using DCL.Huds;
using UnityEngine;

public class LoadingHudHelper : VRHUDHelper
{
    [SerializeField]
    private LayerMask loadingMask;
    [SerializeField]
    private ShowHideAnimator animator;

    protected override void SetupHelper()
    {
        myTrans.localScale = 0.00075f * Vector3.one;
        
        if (myTrans is RectTransform rect)
        {
            rect.sizeDelta = new Vector2(1920, 1080);
        }
        VRHUDController.I.SetupLoading(animator);
        VRHUDController.LoadingStart += () =>
        {
            CrossPlatformManager.SetCameraForLoading(loadingMask);
            var forward = VRHUDController.I.GetForward();
            myTrans.position = Camera.main.transform.position + forward;
            DebugConfigComponent.i.HideWebViewScreens();
            myTrans.forward = forward;
        };
        VRHUDController.LoadingEnd += CrossPlatformManager.SetCameraForGame;
    }
}
