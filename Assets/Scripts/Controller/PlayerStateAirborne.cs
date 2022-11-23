
public class PlayerStateAirborne : PlayerState
{
    public PlayerStateAirborne() : base(PlayerStatesList.AIRBORNE)
    {

    }

    public override void Begin()
    {
        _player.StartFalling();
    }

    public override void StateUpdate()
    {
        _player.ApplyAirborneGravity();
        _player.UpdateCoyoteTime();
        _player.CheckForSteepSlope();
        _player.CheckForGround();
    }

    public override void Exit()
    {

    }

}
