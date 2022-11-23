
public class PlayerStateWalljumping : PlayerState
{
    public PlayerStateWalljumping() : base(PlayerStatesList.WALLJUMPING)
    {

    }

    public override void Begin()
    {
        
    }

    public override void StateUpdate()
    {
        _player.JumpCooldown();
        _player.ApplyJumpingGravity();
        _player.CheckForGround();
        _player.CheckForCeiling();
    }

    public override void Exit()
    {
        
    }

}
