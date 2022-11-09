using System.Collections;
using System.Collections.Generic;
using DCL.Huds;
using UnityEngine;

public class ProfileHudHelper : VRHUDHelper
{
    [SerializeField]
    private ProfileHUDView view;
    
    protected override void SetupHelper()
    {
        VRHUDController.I.Register(this, true);
        VRHUDController.I.Reparent(myTrans);
    }

    public override void Hide()
    {
        view.SetStartMenuButtonActive(false);
        view.SetCardAsFullScreenMenuMode(true);
    }

    public override void Show()
    {
        view.SetStartMenuButtonActive(true);
        view.SetCardAsFullScreenMenuMode(false);
    }
}
