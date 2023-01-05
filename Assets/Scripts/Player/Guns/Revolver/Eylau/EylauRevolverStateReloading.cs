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
        _revolver.ResetChargeLevel();
    }

    public override void StateUpdate()
    {
        _revolver.Reloading();
        Debug.Log("yahoo!");
    }

    public override void Exit()
    {

    }

}
