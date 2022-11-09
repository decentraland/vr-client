using DCL;
using UnityEngine;

public class TermsOfServiceHudHelper : VRHUDHelper
{
    [SerializeField]
    private TermsOfServiceHUDView view;
    private BaseVariable<bool> dataStoreIsOpen = DataStore.i.exploreV2.isOpen;
    protected override void SetupHelper()
    {
        myTrans.localScale = 0.00075f * Vector3.one;
        if (myTrans is RectTransform rect)
        {
            rect.sizeDelta = new Vector2(1920, 1080);
        }
        view.OnSetVisibility += OnVisiblityChange;
    }
    private void OnVisiblityChange(bool visible)
    {
        if (dataStoreIsOpen.Get())
            myTrans.localRotation = Quaternion.identity;
        else if (visible) 
            Position();
    }
    
    private void Position()
    {
        var forward = CommonScriptableObjects.cameraForward;
        myTrans.position = Camera.main.transform.position + forward;
        myTrans.forward = forward;
    }
}