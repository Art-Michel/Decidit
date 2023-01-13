using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauArmStateActive : ArmState
{
    public EylauArmStateActive() : base(ArmStateList.ACTIVE)
    {

    }

    public override void Begin()
    {
        _arm.StartActive();
    }

    public override void StateUpdate()
    {

    }

    public override void Exit()
    {

    }

}
