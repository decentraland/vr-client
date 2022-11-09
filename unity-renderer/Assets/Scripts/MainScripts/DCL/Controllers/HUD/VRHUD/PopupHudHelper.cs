using DCL;
using UnityEngine;

public class PopupHudHelper : VRHUDHelper
{
    [SerializeField]
    protected GameObject objectToHide;
    
    protected override void SetupHelper()
    {
        if (!objectToHide)
            objectToHide = gameObject;
        myTrans.localScale = 0.0025f * Vector3.one;
        objectToHide.SetActive(false);
        if (showHideAnimator)
            showHideAnimator.OnSetVisibility += PositionHud;
        else
            DataStore.i.common.onOpenNFTPrompt.OnChange += OnNFTPromptOpened;
    }
    private void OnNFTPromptOpened(NFTPromptModel current, NFTPromptModel previous)
    {
        PositionHud(true);
    }

    private void PositionHud(bool isVisible)
    {
        if(!isVisible) return;
        CrossPlatformManager.GetSurfacePoint(out var point, out var normal);
        myTrans.position = point + normal * .25f;
        myTrans.forward = -normal;
    }
}
