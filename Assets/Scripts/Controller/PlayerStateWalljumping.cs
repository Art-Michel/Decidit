using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateWalljumping : PlayerState
{
    public PlayerStateWalljumping() : base(PlayerStatesList.WALLJUMPING)
    {

    }

    public override void Begin()
    {
        
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
