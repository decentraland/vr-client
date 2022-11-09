using DCL;
using DCL.Helpers;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.XR.Management;

public static class CrossPlatformManager
{
    private static bool isVR;
    private static PlatformSettings settings;
    public static bool IsVR
    {
        get
        {
            if (settings == null)
                GetControllerName();
            return isVR;
        }
        private set => isVR = value;
    }
    
    private static LayerMask layerMask;
    public static string GetControllerName()
    {
        if (settings == null)
        {
            settings = Resources.Load<PlatformSettings>($"PlatformSettings");
        }
        string contorllerName;
        IsVR = XRGeneralSettings.Instance.Manager.activeLoader != null;
        if (!IsVR)
        {
            contorllerName = settings.NonVRController;
        }
        else
        {
            SetUpForVR();
            contorllerName = settings.VRController;
        }

        return contorllerName;
    }

    public static bool IsVRPlatform()
    {
        return XRGeneralSettings.Instance.Manager.activeLoader != null;
    }

    private static void SetUpForVR()
    {
        OverrideCursorLock(false);
        DCL.Helpers.Utils.OnCursorLockChanged += OverrideCursorLock;
    }
    
    private static void OverrideCursorLock(bool state)
    {
        Utils.IsCursorLocked = true;
    }
    
    public static Ray GetRay()
    {
        var pos = CoreServices.FocusProvider?.PrimaryPointer?.Result?.StartPoint;
        var index = CoreServices.FocusProvider?.PrimaryPointer?.Result?.RayStepIndex;
        if (!index.HasValue)
            return default;
        var rayStep = CoreServices.FocusProvider?.PrimaryPointer.Rays[index.Value];
        var rayStepValue = rayStep.Value;
        return new Ray(rayStepValue.Origin, rayStepValue.Direction);
    }

    public static void SetCameraForLoading(LayerMask mask)
    {
        var mainCam = Camera.main;
        layerMask = mainCam.cullingMask;
        mainCam.cullingMask = mask;
        mainCam.clearFlags = CameraClearFlags.Color;
        mainCam.backgroundColor = Color.black;
    }

    public static void SetCameraForGame()
    {
        var mainCam = Camera.main;
        mainCam.cullingMask =layerMask;
        mainCam.clearFlags = CameraClearFlags.Skybox;
    }
    
    public static Vector3 GetPoint()
    {
        GetSurfacePoint(out var point, out var norm);

        return point + norm * .25f;
    }

    public static void GetSurfacePoint(out Vector3 point, out Vector3 normal)
    {
        var details = CoreServices.FocusProvider?.PrimaryPointer?.Result?.Details;
        point = details?.Point ?? Vector3.zero;
        normal = details?.Normal ?? Vector3.zero;
    }
}
