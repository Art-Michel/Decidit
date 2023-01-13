using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauArmStateIdle : ArmState
{
    public EylauArmStateIdle() : base(ArmStateList.IDLE)
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
