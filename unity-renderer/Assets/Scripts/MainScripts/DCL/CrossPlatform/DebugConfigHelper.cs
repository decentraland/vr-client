using System;
using System.Text;
using DCL;
using UnityEngine;

[RequireComponent(typeof(DebugConfigComponent))]
public class DebugConfigHelper : MonoBehaviour
{
    private const string OrgUrl = "http://play.decentraland.org/?";
    private const string ZoneUrl = "http://play.decentraland.zone/?";
    private const string LocalUrl = "http://localhost:8080/?";
    
    [SerializeField] 
    private DebugConfigComponent debugConfigComponent;
    private void Awake()
    {
        var urlType = debugConfigComponent.baseUrlMode;
        debugConfigComponent.baseUrlMode = DebugConfigComponent.BaseUrl.CUSTOM;
        var builder = new StringBuilder();
        switch (urlType)
        {
            case DebugConfigComponent.BaseUrl.ZONE:
                builder.Append(ZoneUrl + GetClientType());
                break;
            case DebugConfigComponent.BaseUrl.ORG:
                builder.Append(OrgUrl + GetClientType());
                break;
            case DebugConfigComponent.BaseUrl.LOCAL_HOST:
                builder.Append(LocalUrl + GetClientType());
                break;
            case DebugConfigComponent.BaseUrl.CUSTOM:
                builder.Append(debugConfigComponent.customURL + GetClientType());
                break;
        }

        debugConfigComponent.customURL = builder.ToString();
    }

    private string GetClientType()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        return "VR_TYPE=desktop&";
#else
        return "VR_TYPE=android&";
#endif
    }
}
