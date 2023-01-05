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
    }

    public override void StateUpdate()
    {
        _revolver.UpdateChargeLevel();
    }

    public override void Exit()
    {

    }

}
