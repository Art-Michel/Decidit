using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAirborne : PlayerState
{
    public PlayerStateAirborne() : base(PlayerStatesList.AIRBORNE)
    {

    }

    public override void Begin()
    {
        _player.StartFalling();
    }

    public override void StateUpdate()
    {
        _player.CheckForGround();
        _player.ApplyAirborneGravity();
        _player.CheckForCeiling();
    }

    public override void Exit()
    {

    }

}
