
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
        _player.ApplyJumpingGravity();
        _player.JumpSlideUpSlope();
        _player.CheckForNoCeiling();
        _player.CheckForGround();
    }

    public override void Exit()
    {
        _player.ResetSlopeMovement();
    }

}
