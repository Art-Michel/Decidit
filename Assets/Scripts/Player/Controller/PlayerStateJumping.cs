
using System.Numerics;
public class PlayerStateJumping : PlayerState
{
    public PlayerStateJumping() : base(PlayerStatesList.JUMPING)
    {

    }

    public override void Begin()
    {
        if (_fsm.PreviousState.Name != PlayerStatesList.JUMPINGUPSLOPE)
            _player.StartJumping();
    }

    public override void StateUpdate()
    {
        if (_fsm.PreviousState.Name != PlayerStatesList.WALLJUMPING)
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

    }

}
