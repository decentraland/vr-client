using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = System.Diagnostics.Debug;

public class PointerHelper : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;
    private RectTransform referenceTrans;
    private readonly Vector3[] corners = new Vector3[4];
    private Vector3 origin;
    private bool isVR;

    private void Awake()
    {
        isVR = CrossPlatformManager.IsVRPlatform();
    }

    public Vector3 GetPointerPos()
    {
        if (!isVR)
            return Input.mousePosition;
        UpdateOrigin();
        Vector3 localPointerPos = default;
        var hit = CoreServices.FocusProvider?.PrimaryPointer?.Result?.Details.Point;
        
        Vector3 point = ToLocalSpace(hit ?? Vector3.zero);
        localPointerPos = point - origin;

        return localPointerPos + offset;
    }
    
    private Vector3 ToLocalSpace(Vector3 hitpoint)
    {
        return referenceTrans.worldToLocalMatrix.MultiplyPoint(hitpoint);
    }
    
    private void UpdateOrigin()
    {
        if (referenceTrans != null)
            referenceTrans.GetLocalCorners(corners);
        origin = corners[0];
    }

    public void UpdateCorners(Vector3[] refCorners, RectTransform rectTrans, ref Vector3 worldCoordsOriginInMap)
    {
        rectTrans.GetWorldCorners(refCorners);
        if (isVR)
            worldCoordsOriginInMap = rectTrans.worldToLocalMatrix.MultiplyPoint(refCorners[0]);
        else
            worldCoordsOriginInMap = refCorners[0];
        if (referenceTrans == null) referenceTrans = rectTrans;
    }

    public bool IsCursorOverMapChunk(int layer)
    {
        if (!isVR)
            return IsMouseOverUI(layer);
        return IsPointerOnUI(layer);
    }
    
    private bool IsPointerOnUI(int layer)
    {
        if(CoreServices.FocusProvider?.PrimaryPointer == null) return false;
        var target = CoreServices.FocusProvider?.PrimaryPointer.Result.CurrentPointerTarget;
        return target != null && target.layer == layer;
    }
    
    private bool IsMouseOverUI(int layer)
    {
        PointerEventData uiRaycastPointerEventData = new PointerEventData(EventSystem.current);
        uiRaycastPointerEventData.position = Input.mousePosition;
        List<RaycastResult> uiRaycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(uiRaycastPointerEventData, uiRaycastResults);

        return uiRaycastResults.Count > 0 && uiRaycastResults[0].gameObject.layer == layer;
    }
}