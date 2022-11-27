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

    }

    public override void Exit()
    {

    }

}
