using System;
using System.Collections;
using System.Collections.Generic;
using DCL;
using DCL.SettingsCommon.SettingsControllers.BaseControllers;
using DCL.SettingsCommon.SettingsControllers.SpecificControllers;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Logger = UnityEngine.Logger;


public class PerformanceController : MonoBehaviour
{
   
    private Camera camera;
    private SettingsControlController renderingScaleSettingController;
    private SettingsControlController sceneLoadRadiusSettingController;
    // private long totalAllocatedMemoryLong;
    // private long monoUsedSizeLong;
    // private long allocatedMemoryForGraphicsDriver;
    // private long totalUnusedReservedMemoryLong;
    // private long totalMemory;

    [SerializeField] private int lowFPSThreshold = 60;
    [SerializeField] private int acceptableFPSThreshold = 68;
    [SerializeField] private int minimumClipPlaneDistance = 50;
    [SerializeField] private int normalClipPlaneDistance = 1000;
    
    private int frameCount = 300;

    // #if UNITY_ANDROID && !UNITY_EDITOR
    // private long MaxMemoryAllowed = 1800000000;
    // #else
    // private long MaxMemoryAllowed = 5800000000;
    // #endif
    // Start is called before the first frame update
   
    private UniversalRenderPipelineAsset lightweightRenderPipelineAsset = null;
    void Start()
    {
       
        // renderingScaleSettingController = ScriptableObject.CreateInstance<RenderingScaleControlController>();
        // renderingScaleSettingController.Initialize();
        // sceneLoadRadiusSettingController = ScriptableObject.CreateInstance<ScenesLoadRadiusControlController>();
        // sceneLoadRadiusSettingController.Initialize();
        StartCoroutine(CheckPerformace());

    }
   

    // Update is called once per frame
    void Update()
    {
        
    }

    private WaitForSeconds waitTimeCheck = new WaitForSeconds(0.5f);

    private IEnumerator CheckPerformace()
    {
        while (true)
        {
            //yield return waitTimeCheck;
            DateTime startTime = DateTime.Now;
            for (int i = 0; i < frameCount; i++)
            {
                yield return null;
                
            }
            
            TimeSpan frameSpan = (DateTime.Now - startTime);

            double fps =  1 / frameSpan.TotalSeconds*frameCount ;
            Debug.Log($"maxDownloads {DataStore.i.performance.maxDownloads.Get()}, fps {fps}");
            // totalAllocatedMemoryLong = Profiler.GetTotalAllocatedMemoryLong();
            // monoUsedSizeLong = Profiler.GetMonoUsedSizeLong(); 
            // allocatedMemoryForGraphicsDriver = Profiler.GetAllocatedMemoryForGraphicsDriver();
            // totalUnusedReservedMemoryLong = Profiler.GetTotalUnusedReservedMemoryLong();
            // totalMemory = totalAllocatedMemoryLong+monoUsedSizeLong+allocatedMemoryForGraphicsDriver;

            // Debug.Log($"Performance: fps{fps}, Memory tot{totalMemory/1000000} ,alloc{totalAllocatedMemoryLong/1000000},mono{monoUsedSizeLong/1000000},gfx{allocatedMemoryForGraphicsDriver/1000000}, unusedres{totalUnusedReservedMemoryLong/1000000}");
            // if ( totalMemory > MaxMemoryAllowed) OnLowMemory();

            // if (totalMemory < (.73 * MaxMemoryAllowed))
            // {
            //     RestoreSettings();
            // }

            //if(fps < lowFPSThreshold) OnLowFrameRate((int)fps);
            //else if (fps > acceptableFPSThreshold) GoodFrameRate();
        }
    }
    
    
 
    
    private void OnLowFrameRate(int fps)
    {
        camera = Camera.main;
        if (camera.farClipPlane > minimumClipPlaneDistance)
            camera.farClipPlane = Math.Max(minimumClipPlaneDistance, camera.farClipPlane * (float)(fps/acceptableFPSThreshold));
        DataStore.i.performance.maxDownloads.Set(Math.Max(150, DataStore.i.performance.maxDownloads.Get()*(int)(fps/75)));
        // object newValue = renderingScaleSettingController.GetStoredValue();
        // float.TryParse(newValue.ToString(), out float newFloat);
        // if (newFloat > 0.8f)
        // {
        //     renderingScaleSettingController.UpdateSetting(Math.Max(0.8f, newFloat*(float)(fps/75)));
        // }
        //Debug.Log($"Low FrameRate.  Changed paramaters: farclipplane {camera.farClipPlane}, renderscale {newFloat*(float)(fps/75)} ");

       
    }
    private void GoodFrameRate()
    {
        camera = Camera.main;
        if (camera.farClipPlane < normalClipPlaneDistance)
            camera.farClipPlane = camera.farClipPlane * 1.15f;
        DataStore.i.performance.maxDownloads.Set(Math.Min(999, (int)(DataStore.i.performance.maxDownloads.Get()*1.15)));
        // object newValue = renderingScaleSettingController.GetStoredValue();
        // float.TryParse(newValue.ToString(), out float newFloat);
        // if (newFloat < 1f)
        // {
        //     renderingScaleSettingController.UpdateSetting(Math.Min(1,(newFloat*1.15f)));
        // }
        //Debug.Log($"FrameRate Good.  Changed paramaters: farclipplane {camera.farClipPlane}, renderscale {newFloat*1.1f}, ");
       
    }
}

