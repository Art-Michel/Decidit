
public class PlayerStateSliding : PlayerState
{
    public PlayerStateSliding() : base(PlayerStatesList.SLIDING)
    {

    }

    public override void Begin()
    {

    }

    public override void StateUpdate()
    {
        _player.CheckForNoGround();
        _player.CheckForSteepSlope();
    }

    public override void Exit()
    {
        
    }

}
