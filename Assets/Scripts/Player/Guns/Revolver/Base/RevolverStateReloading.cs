using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverStateReloading : RevolverState
{
    public RevolverStateReloading() : base(RevolverStateList.RELOADING)
    {

    }

    public override void Begin()
    {
        //animation
        _revolver.StartReload();
    }

    public override void StateUpdate()
    {
        _revolver.Reloading();
    }

    public override void Exit()
    {
        _revolver.DisableReloadUI();
    }

}
