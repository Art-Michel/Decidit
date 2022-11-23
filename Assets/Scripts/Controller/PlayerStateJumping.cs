
public class PlayerStateJumping : PlayerState
{
    public PlayerStateJumping() : base(PlayerStatesList.JUMPING)
    {

    }

    public override void Begin()
    {
        if (_fsm.previousState.Name != PlayerStatesList.JUMPINGUPSLOPE)
            _player.StartJumping();
    }

    public override void StateUpdate()
    {
        _player.ApplyJumpingGravity();
        _player.CheckForCeiling();
        _player.CheckForGround();
    }

    public override void Exit()
    {

    }

}
