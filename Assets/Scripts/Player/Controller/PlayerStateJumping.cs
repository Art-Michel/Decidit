
using System.Numerics;
using UnityEngine;

public class PlayerStateJumping : PlayerState
{
    public PlayerStateJumping() : base(PlayerStatesList.JUMPING)
    {

    }

    public override void Begin()
    {
        if (_fsm.PreviousState.Name != PlayerStatesList.JUMPINGUPSLOPE)
            _player.StartJumping();

        Debug.Log(_fsm.PreviousState.Name);
    }

    public override void StateUpdate()
    {
        if (!_player.JustWalljumped)
        {
            _player.CheckWall();
            _player.CheckForJumpingWallride();
            _player.WallCoyoteTimeCooldown();
        }

        _player.ApplyJumpingGravity();
        _player.CheckForCeiling();
    }

    public override void Exit()
    {
        _player.JustWalljumped = false;

    }

}
