using DCL;
using DCL.Huds;
using GotoPanel;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.Utilities;
using SignupHUD;
using UnityEngine;
using UnityEngine.UI;

public class TeleportPromptHudHelper : VRHUDHelper
{
    [SerializeField]
    private TeleportPromptHUDView view;
    private BaseVariable<bool> dataStoreIsOpen = DataStore.i.exploreV2.isOpen;
    protected override void SetupHelper()
    {
        ConvertUI();
        transform.parent.localScale = 0.0033f * Vector3.one;
        view.content.transform.localScale = Vector3.one;
        myTrans.localPosition = Vector3.zero;
        view.OnSetVisibility += OnVisiblityChange;
    }
    private void OnVisiblityChange(bool visible)
    {
        myTrans.position = Vector3.zero;
        if (!visible) { 
            
            // myTrans.position += 10 * Vector3.down;
            // myTrans.localRotation = Quaternion.identity;
        }
        else 
            Position();
    }
    
    private void Position()
    {
        myTrans.localPosition = Vector3.zero;
        
        var forward = VRHUDController.I.GetForward();
        if (Camera.main != null)
            myTrans.parent.position = Camera.main.transform.position+ 1.9f*forward;

        myTrans.parent.forward = forward;
        myTrans.localPosition = Vector3.zero;
    }
    private void ConvertUI()
    {
        Canvas canvas = view.content.GetComponent<Canvas>();
        
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.overrideSorting = true;
        canvas.sortingOrder = sortingOrder;
        canvas.sortingLayerName = "Menu";
        if (GetComponent<GraphicRaycaster>() == null)
        {
            var caster = gameObject.AddComponent<GraphicRaycaster>();
        }
        if (GetComponent<CanvasUtility>() == null)
            gameObject.AddComponent<CanvasUtility>();
        if (GetComponent<NearInteractionTouchableUnityUI>() == null)
        {
            var inter = gameObject.AddComponent<NearInteractionTouchableUnityUI>();
            inter.EventsToReceive = TouchableEventType.Pointer;
        }
    }
}
