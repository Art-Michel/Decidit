using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AragonArmFSM : ArmFSM
{
    protected override void Start()
    {
        AddState(new AragonArmStateIdle());
        AddState(new AragonArmStateActive());
        AddState(new AragonArmStatePrevis());
        AddState(new AragonArmStateRecovery());

        ChangeState(ArmStateList.IDLE);
    }
}
