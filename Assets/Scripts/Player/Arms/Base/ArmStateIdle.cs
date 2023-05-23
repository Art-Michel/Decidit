using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmStateIdle : ArmState
{
    public ArmStateIdle() : base(ArmStateList.IDLE)
    {

    }

    public override void Begin()
    {
        _arm.StartIdle();
    }

    public override void StateUpdate()
    {
        // _arm.Sway();
    }

    public override void Exit()
    {
        _arm.StopSwaying();
    }

}
