
public class PlayerStateWalljumping : PlayerState
{
    public PlayerStateWalljumping() : base(PlayerStatesList.WALLJUMPING)
    {

    }

    public override void Begin()
    {
        _player.JustWalljumped = true;
        _player.StartWalljumping();
    }

    public override void StateUpdate()
    {
        // _player.ApplyJumpingGravity();
        // _player.CheckForCeiling();
    }

    public override void Exit()
    {

    }

}
