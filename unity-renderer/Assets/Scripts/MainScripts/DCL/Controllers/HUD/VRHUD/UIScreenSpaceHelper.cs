using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public sealed class UIScreenSpaceHelper : VRHUDHelper
{
    private SolverHandler handler;
    private RadialView radialView;
    protected override void SetupHelper()
    {
        myTrans.localScale = .001f * Vector3.one;
        handler = gameObject.AddComponent<SolverHandler>();
        handler.UpdateSolvers = false;
        radialView = gameObject.AddComponent<RadialView>();
        radialView.MaxDistance = .75f;
        radialView.MinDistance = .75f;
        radialView.ReferenceDirection = RadialViewReferenceDirection.GravityAligned;
        radialView.MaxViewDegrees = 15f;
        DCLCharacterController.i.OnUpdateFinish += UpdateSolvers;
    }

    private void UpdateSolvers(float deltaTime)
    {
        if (myTrans == null) return;
        handler.GoalPosition = myTrans.position;
        handler.GoalRotation = myTrans.rotation;
        handler.GoalScale = myTrans.localScale;

        radialView.SolverUpdateEntry();
    }
}
