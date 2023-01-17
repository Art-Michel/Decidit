using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class PlayerHealth : Health
{
    [Foldout("References")]
    [SerializeField] Player _player;
    [Foldout("References")]
    [SerializeField] Image _hpCursor;
    [Foldout("References")]
    [SerializeField] RectTransform _hpBarStart;
    [Foldout("References")]
    [SerializeField] RectTransform _hpBarEnd;
    [Foldout("Stats")]
    [SerializeField]
    [Tooltip("How much Timescale will slow down when player gets hit. Lower is stronger.")]
    private float _playerHurtFreezeStrength = 0.01f;
    [Foldout("Stats")]
    [SerializeField]
    [Tooltip("For how long Timescale will slow down when player gets hit.")]
    private float _playerHurtFreezeDuration = 0.2f;
    [Foldout("Stats")]
    [SerializeField]
    [Tooltip("How much Screen will shake when player gets hit.")]
    private float _playerHurtShakeStrength = 0.3f;
    [Foldout("Stats")]
    [SerializeField]
    [Tooltip("For how long Screen will shake when player gets hit.")]
    private float _playerHurtShakeDuration = 0.3f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void DisplayHealth()
    {
        base.DisplayHealth();
        if (_hpCursor)
            _hpCursor.rectTransform.anchoredPosition = Vector2.Lerp(_hpBarStart.anchoredPosition, _hpBarEnd.anchoredPosition, _hpUi.fillAmount);
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        PlaceHolderSoundManager.Instance.PlayHurt();
        Player.Instance.StartShake(_playerHurtShakeStrength, _playerHurtShakeDuration);
        //GameManager.Instance.StartSlowMo(_playerHurtFreezeStrength, _playerHurtFreezeDuration);
    }

    public override void Knockback(Vector3 direction)
    {
        _player.AddMomentum(direction);
    }
}
