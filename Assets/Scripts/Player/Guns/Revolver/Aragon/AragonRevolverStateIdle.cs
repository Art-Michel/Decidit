using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AragonRevolverStateIdle : RevolverState
{
    public AragonRevolverStateIdle() : base(RevolverStateList.IDLE)
    {

    }

    public override void Begin()
    {
        _revolver.EmptyReloadUI();
        _revolver.Animator.CrossFade("idle", 0.0f, 0);
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
