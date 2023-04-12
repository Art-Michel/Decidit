
public class PlayerStateJumpingUpSlope : PlayerState
{
    public PlayerStateJumpingUpSlope() : base(PlayerStatesList.JUMPINGUPSLOPE)
    {

    }

    public override void Begin()
    {

    }

    public override void StateUpdate()
    {
        if (_fsm.PreviousState.Name != PlayerStatesList.WALLJUMPING)
        {
            _player.CheckWall();
            _player.CheckForJumpingWallride();
            _player.WallCoyoteTimeCooldown();
        }
        _player.JumpSlideUpSlope();
        _player.CheckForCeiling();
        _player.CheckForNoCeiling();
        _player.ApplyJumpingGravity();
    }

    public override void Exit()
    {
        _player.ResetSlopeMovement();
    }

}
