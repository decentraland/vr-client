using DCL;
using UnityEngine;

public class ControlsHudHelper : VRHUDHelper
{
    [SerializeField]
    private ControlsHUDView view;
    private BaseVariable<bool> dataStoreIsOpen = DataStore.i.exploreV2.isOpen;
    protected override void SetupHelper()
    {
        //view.OnSetVisibility += OnVisiblityChange;
        
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
        myTrans.position = CommonScriptableObjects.cameraPosition.Get() + forward + 2*Vector3.up + Vector3.forward;
        myTrans.forward = forward;
        Debug.Log("AvatarEditor: Positioned");
    }
}
