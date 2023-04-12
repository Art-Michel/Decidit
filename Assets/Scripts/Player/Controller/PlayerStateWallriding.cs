
public class PlayerStateWallriding : PlayerState
{
    public PlayerStateWallriding() : base(PlayerStatesList.WALLRIDING)
    {

    }

    public override void Begin()
    {
        _player.KillMomentum();
        _player.ResetWallCoyoteTime();
        _player.CurrentlyAppliedGravity = _player.WallRidingGravity;
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
    }

}
