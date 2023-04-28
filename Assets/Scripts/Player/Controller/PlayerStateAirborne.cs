
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
        _player.CheckWall();
        _player.CheckForWallride();

        _player.ApplyAirborneGravity();
        _player.CoyoteTimeCooldown();
        _player.WallCoyoteTimeCooldown();

        _player.CheckForSteepSlope();
        _player.CheckForGround();
        _player.FailSafe();
    }

    public override void Exit()
    {
        //SoundManager.Instance.PlaySound("event:/SFX_Controller/CharactersNoises/Landing", 1f, _player.gameObject);
    }

}
