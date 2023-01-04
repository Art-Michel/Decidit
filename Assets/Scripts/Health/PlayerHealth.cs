using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class PlayerHealth : Health
{
    [SerializeField] Image _hpCursor;
    [SerializeField] RectTransform _hpBarStart;
    [SerializeField] RectTransform _hpBarEnd;
    [SerializeField]
    [Tooltip("How much Timescale will slow down when player gets hit. Lower is stronger.")]
    private float _playerHurtFreezeStrength = 0.01f;
    [SerializeField]
    [Tooltip("For how long Timescale will slow down when player gets hit.")]
    private float _playerHurtFreezeDuration = 0.2f;
    [SerializeField]
    [Tooltip("How much Screen will shake when player gets hit.")]
    private float _playerHurtShakeStrength = 0.3f;
    [SerializeField]
    [Tooltip("For how long Screen will shake when player gets hit.")]
    private float _playerHurtShakeDuration = 0.3f;


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
        GameManager.Instance.StartSlowMo(_playerHurtFreezeStrength, _playerHurtFreezeDuration);
    }
}
