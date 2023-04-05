
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
        _player.CoyoteTimeCooldown();
        _player.CheckForSteepSlope();
        _player.CheckForGround();
    }

    public override void Exit()
    {
        //SoundManager.Instance.PlaySound("event:/SFX_Controller/CharactersNoises/Landing", 1f, _player.gameObject);
    }

}
