using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AragonArmStatePrevis : ArmState
{
    public AragonArmStatePrevis() : base(ArmStateList.PREVIS)
    {

    }

    public override void Begin()
    {
        _arm.StartPrevis();
    }

    public override void StateUpdate()
    {
        _arm.UpdatePrevis();
    }

    public override void Exit()
    {
        _arm.StopPrevis();
    }

}
