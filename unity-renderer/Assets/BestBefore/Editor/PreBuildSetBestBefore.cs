using System;
using System.Collections;
using System.Collections.Generic;
//#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;



 class PreBuildSetBestBefore : IPreprocessBuildWithReport
 {
     public int daysOut = 40;
     public  BestBefore.BestBefore bestBefore;
       public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        
        bestBefore = Camera.main.GetComponent<BestBefore.BestBefore>();
        if (bestBefore == null || bestBefore.ExpirationMode != BestBefore.BestBefore.ExpirationModes.FixedDateFromBuild)
            return;
        
      // Do the preprocessing here

          daysOut = bestBefore.DaysOut;
          DateTime today = DateTime.Today;
          DateTime future = today + TimeSpan.FromDays(daysOut);
          bestBefore.Year = future.Year;
          bestBefore.Month = (BestBefore.BestBefore.MonthsOfYear)future.Month;
          bestBefore.Day = future.Day;
          bestBefore.Hour = future.Hour;
          bestBefore.Minute = future.Minute;
          bestBefore.Second = future.Second;

      }
 }
//#endif