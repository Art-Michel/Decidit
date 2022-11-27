using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseRevolverStateIdle : RevolverState
{
    public MuseRevolverStateIdle() : base(RevolverStateList.IDLE)
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
