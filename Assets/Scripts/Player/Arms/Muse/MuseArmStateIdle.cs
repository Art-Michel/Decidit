using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseArmStateIdle : ArmState
{
    public MuseArmStateIdle() : base(ArmStateList.IDLE)
    {

    }

    public override void Begin()
    {
        _arm.StartIdle();
    }

    public override void StateUpdate()
    {

    }

    public override void Exit()
    {

    }

}
