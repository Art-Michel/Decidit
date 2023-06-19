
public class PlayerStateWallriding : PlayerState
{
    public PlayerStateWallriding() : base(PlayerStatesList.WALLRIDING)
    {

    }

    public override void Begin()
    {
        _player.GlobalMomentum.y = 0.0f;
        if (_player.WallrideDragFactor > 0.2f)
            _player.CurrentlyAppliedGravity = _player.WallRidingBaseGravity;
        _player.SetWallridingMovementSpeed();
        _player.WallRideSmokeIntervalT = 0.0f;

        // _player.KillGravity();
        // _player.KillMomentum();
        // _player.ResetAcceleration();
        _player.ResetWallCoyoteTime();
        _player.CheckForJumpBuffer();
    }

    public override void StateUpdate()
    {
        _player.WallRideSmoke();
        _player.CheckWall();
        _player.ApplyWallridingGravity();
        _player.CheckForNoWallRide();
        _player.CheckForGround();
    }

    public override void Exit()
    {
        _player.ResetSlopeMovement();
        _player.ResetAcceleration();
        _player.ResetWallridingMovementSpeed();
    }

}
