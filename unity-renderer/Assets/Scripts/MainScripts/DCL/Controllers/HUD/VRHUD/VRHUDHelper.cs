using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.Utilities;
using UnityEngine;
using UnityEngine.UI;

public abstract class VRHUDHelper : MonoBehaviour
{
    [SerializeField]
    public int sortingOrder;
    [SerializeField]
    protected ShowHideAnimator showHideAnimator;
    
    protected Transform myTrans;

    protected virtual void Awake()
    {
        if (!CrossPlatformManager.IsVR)
        {
            enabled = false;
            return;
        }
        myTrans = transform;
    }

    private void Start()
    {
        ConvertUI();
        SetupHelper();
    }

    protected abstract void SetupHelper();

    private void ConvertUI()
    {
        Canvas canvas = GetComponent<Canvas>();
        
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

    public void Hide(Vector3 pos) => myTrans.position += pos;

    public virtual void Hide()
    {
        if (showHideAnimator)
            showHideAnimator.Hide();
    }

    public void ResetHud()
    {
        myTrans.localPosition = Vector3.zero;
        myTrans.localRotation = Quaternion.identity;
    }
    public virtual void Show() 
    { 
        if (showHideAnimator)
            showHideAnimator.Show(true);
        
    }
}