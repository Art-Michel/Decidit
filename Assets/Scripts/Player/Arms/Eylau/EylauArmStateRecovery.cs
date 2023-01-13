using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauArmStateRecovery : ArmState
{
    public EylauArmStateRecovery() : base(ArmStateList.RECOVERY)
    {

    }

    public override void Begin()
    {
        _fsm.ChangeState(ArmStateList.IDLE);

    }

    public override void StateUpdate()
    {

    }

    public override void Exit()
    {

    }

}
