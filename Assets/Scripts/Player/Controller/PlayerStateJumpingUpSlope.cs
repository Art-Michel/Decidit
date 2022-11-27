
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
