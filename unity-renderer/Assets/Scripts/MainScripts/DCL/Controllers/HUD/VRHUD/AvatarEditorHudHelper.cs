using DCL;
using UnityEngine;

public class AvatarEditorHudHelper : VRHUDHelper
{
    [SerializeField]
    private AvatarEditorHUDView view;
    private BaseVariable<bool> dataStoreIsOpen = DataStore.i.exploreV2.isOpen;
    protected override void SetupHelper()
    {
        
        view.OnSetVisibility += OnVisiblityChange;
    }
    private void OnVisiblityChange(bool visible)
    {
        // if (dataStoreIsOpen.Get())
        //     myTrans.localRotation = Quaternion.identity;
        // else if (visible) 
            Position();
    }
    
    private void Position()
    {
        myTrans.localScale = 0.0034f*Vector3.one;
        var forward = CommonScriptableObjects.cameraForward;
        myTrans.position = Camera.main.transform.position +forward + 15.0f* Vector3.right;
        myTrans.forward = forward;
        
    }
}
