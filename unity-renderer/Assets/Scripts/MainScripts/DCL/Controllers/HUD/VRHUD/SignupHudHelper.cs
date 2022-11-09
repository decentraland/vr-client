using SignupHUD;
using UnityEngine;

public class SignupHudHelper : VRHUDHelper
{
    [SerializeField]
    private SignupHUDView view;
    private Transform camTrans;
    protected override void Awake()
    {
        base.Awake();
        myTrans = transform;
        view.OnSetVisibility += OnVisiblityChange;
        if (myTrans is RectTransform rect)
        {
            rect.sizeDelta = new Vector2(1920, 1080);
        }
    }
    
    protected override void SetupHelper() { }

    private void OnVisiblityChange(bool visible)
    {
        Position();
    }
    
    public void Position()
    {
        var rawForward = CommonScriptableObjects.cameraForward.Get();
        var forward = new Vector3(rawForward.x, 0, rawForward.z).normalized;
        var pos = CommonScriptableObjects.cameraPosition.Get() + forward;
        myTrans.position = new Vector3(pos.x, 1.2f, pos.z);
        myTrans.forward = forward;
    }
}
