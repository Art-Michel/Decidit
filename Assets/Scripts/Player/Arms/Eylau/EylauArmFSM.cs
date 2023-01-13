using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauArmFSM : ArmFSM
{
    protected override void Start()
    {
        AddState(new EylauArmStateIdle());
        AddState(new EylauArmStateActive());
        AddState(new EylauArmStatePrevis());
        AddState(new EylauArmStateRecovery());

        ChangeState(ArmStateList.IDLE);
    }
}
