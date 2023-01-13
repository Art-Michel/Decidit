using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmStatePrevis : ArmState
{
    public ArmStatePrevis() : base(ArmStateList.PREVIS)
    {

    }

    public override void Begin()
    {
        _arm.StartPrevis();
    }

    public override void StateUpdate()
    {

    }

    public override void Exit()
    {

    }

}
