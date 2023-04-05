
public class PlayerStateJumping : PlayerState
{
    public PlayerStateJumping() : base(PlayerStatesList.JUMPING)
    {

    }

    public override void Begin()
    {
        if (_fsm.PreviousState.Name != PlayerStatesList.JUMPINGUPSLOPE)
            _player.StartJumping();
    }

    public override void StateUpdate()
    {
        _player.ApplyJumpingGravity();
        _player.CheckForCeiling();
    }

    public override void Exit()
    {
        
    }

}
