using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateJumping : PlayerState
{
    public PlayerStateJumping() : base(PlayerStatesList.JUMPING)
    {

    }

    public override void Begin()
    {
        _player.StartJumping();
    }

    public override void StateUpdate()
    {
        _player.CheckForGround();
        _player.ApplyJumpingGravity();
        _player.CheckForCeiling();
    }

    public override void Exit()
    {
        
    }

}
