using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverStateIdle : RevolverState
{
    public RevolverStateIdle() : base(RevolverStateList.IDLE)
    {

    }

    public override void Begin()
    {
        _revolver.CheckBuffer();
        _revolver.Animator.CrossFade("idle", 0.0f, 0);
        _revolver.EmptyReloadUI();
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
