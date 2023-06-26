using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AragonRevolverStateReloading : RevolverState
{
    public AragonRevolverStateReloading() : base(RevolverStateList.RELOADING)
    {

    }

    public override void Begin()
    {
        //animation
        _revolver.StartReloadFugue();
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
