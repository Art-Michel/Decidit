using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseArmFSM : ArmFSM
{
    protected override void Start()
    {
        AddState(new MuseArmStateIdle());
        AddState(new MuseArmStateActive());
        AddState(new MuseArmStatePrevis());
        AddState(new MuseArmStateRecovery());

        ChangeState(ArmStateList.IDLE);
    }
}
