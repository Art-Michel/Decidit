using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauRevolverStateReloading : RevolverState
{
    public EylauRevolverStateReloading() : base(RevolverStateList.RELOADING)
    {

    }

    public override void Begin()
    {
        //animation
        _revolver.StartReload();
        _revolver.Animator.CrossFade("reload", 0.0f, 0);

        //* Comment line below if we want to reset charge level on reload
        // if (_fsm.PreviousState.Name == RevolverStateList.RELOADING)
        _revolver.ResetChargeLevel();
    }

    public override void StateUpdate()
    {
        _revolver.Reloading();
    }

    public override void Exit()
    {
        _revolver.EmptyReloadUI();

    }
}