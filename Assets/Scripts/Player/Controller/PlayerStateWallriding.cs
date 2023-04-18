
public class PlayerStateWallriding : PlayerState
{
    public PlayerStateWallriding() : base(PlayerStatesList.WALLRIDING)
    {

    }

    public override void Begin()
    {
        if (_player.GlobalMomentum.y <= 0)
            _player.CurrentlyAppliedGravity = _player.WallRidingGravity;
        // _player.KillGravity();
        // _player.KillMomentum();
        // _player.ResetAcceleration();
        _player.ResetWallCoyoteTime();
        _player.CheckForJumpBuffer();
    }

    public override void StateUpdate()
    {
        _player.CheckWall();
        _player.ApplyWallridingGravity();
        _player.CheckForNoWallRide();
        _player.CheckForGround();
    }

    public override void Exit()
    {
        _player.ResetAcceleration();
    }

}
