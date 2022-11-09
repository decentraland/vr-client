using SignupHUD;
using UnityEngine;

public class TransactionListHudHelper : VRHUDHelper
{
    [SerializeField]
    private TransactionListHUDView view;
    private Transform camTrans;
    protected override void Awake()
    {
        base.Awake();
       
        if (myTrans is RectTransform rect)
        {
            rect.sizeDelta = new Vector2(1920, 1080);
        }
    }
    
    protected override void SetupHelper() { }

    private void OnEnabled()
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
