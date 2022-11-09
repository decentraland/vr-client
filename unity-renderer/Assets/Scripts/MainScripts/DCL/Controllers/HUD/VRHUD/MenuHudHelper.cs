using DCL.Huds;
using UnityEngine;

public class MenuHudHelper : VRHUDHelper
{
    [SerializeField]
    private bool submenu;
    
    protected override void SetupHelper()
    {
        VRHUDController.I.Register(this, submenu);
        VRHUDController.I.Reparent(myTrans);
    }
}
