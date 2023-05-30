using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseArmStatePrevis : ArmState
{
    public MuseArmStatePrevis() : base(ArmStateList.PREVIS)
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
        _arm.StopPrevis();
    }

}
