
public class PlayerStateGrounded : PlayerState
{
    public PlayerStateGrounded() : base(PlayerStatesList.GROUNDED)
    {

    }

    public override void Begin()
    {
        _player.Land();
        _player.CheckForJumpBuffer();
    }

    public override void StateUpdate()
    {
        _player.CheckForNoGround();
        // _player.CheckForSteepSlope();
    }

    public override void Exit()
    {

    }

}
