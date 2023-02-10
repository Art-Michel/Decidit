using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauRevolverStateIdle : RevolverState
{
    public EylauRevolverStateIdle() : base(RevolverStateList.IDLE)
    {

    }

    public override void Begin()
    {
        _revolver.CheckBuffer();
        EylauRevolver _eyRev = _revolver as EylauRevolver;
    }

    public override void StateUpdate()
    {
        _revolver.UpdateChargeLevel();
        _revolver.Sway();
    }

    public override void Exit()
    {
        _revolver.StopSwaying();
    }

}
