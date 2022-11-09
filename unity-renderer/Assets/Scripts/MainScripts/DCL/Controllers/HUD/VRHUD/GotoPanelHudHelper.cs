using DCL;
using DCL.Huds;
using GotoPanel;
using SignupHUD;
using UnityEngine;

public class GotoPanelHudHelper : VRHUDHelper
{
    [SerializeField]
    private GotoPanelHUDView view;
    private BaseVariable<bool> dataStoreIsOpen = DataStore.i.exploreV2.isOpen;
    protected override void SetupHelper()
    {
        myTrans.localScale = 0.002f * Vector3.one;
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
        var forward = VRHUDController.I.GetForward();
        #if UNITY_ANDROID && !UNITY_EDITOR
        myTrans.position = Camera.main.transform.position + forward;
        #else
        myTrans.position = Camera.main.transform.position + forward + 2.3f*Vector3.up + 3.75f*Vector3.forward;
        #endif
        myTrans.forward = forward;
    }
}
