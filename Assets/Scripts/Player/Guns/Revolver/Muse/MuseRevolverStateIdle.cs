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
        _revolver.Sway();
    }

    public override void Exit()
    {
        _revolver.StopSwaying();
    }

}
