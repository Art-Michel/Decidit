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
        _arm.StartRecovery();
    }

    public override void StateUpdate()
    {
        _arm.UpdateCooldown();
    }

    public override void Exit()
    {

    }

}
