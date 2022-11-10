using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateWallriding : PlayerState
{
    public PlayerStateWallriding() : base(PlayerStatesList.WALLRIDING)
    {

    }

    public override void Begin()
    {

    }

    public override void StateUpdate()
    {
        _player.CheckForNoGround();
        _player.CheckForSteepSlope();
        _player.CheckForGround();
    }

    public override void Exit()
    {

    }

}
