using System;
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

    [Foldout("References")]
    [SerializeField] Image _lowHpVignette;

    [Foldout("References")]
    [SerializeField] Image _probVignette;
    [Foldout("Properties")]
    [SerializeField] AnimationCurve _vignetteAlphaOnProb;

    [Foldout("References")]
    [SerializeField] Image _healVignette;
    [Foldout("Properties")]
    [SerializeField] AnimationCurve _vignetteAlphaOnHeal;
    private float _healVignetteT;
    private bool _isHealing;
    private const float _healVignetteSpeed = 2.0f;

    [Foldout("Stats")]
    [SerializeField]
    [Tooltip("How much Screen will shake when player gets hit.")]
    private float _playerHurtShakeMaxStrength = 0.3f;
    [Foldout("Stats")]
    [SerializeField]
    [Tooltip("For how long Screen will shake when player gets hit.")]
    private float _playerHurtShakeDuration = 0.3f;

    //* Unused!
    // [Foldout("Stats")]
    // [SerializeField]
    // [Tooltip("How much Timescale will slow down when player gets hit. Lower is stronger.")]
    // private float _playerHurtFreezeStrength = 0.01f;
    // [Foldout("Stats")]
    // [SerializeField]
    // [Tooltip("For how long Timescale will slow down when player gets hit.")]
    // private float _playerHurtFreezeDuration = 0.2f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        if (_isHealing) HandleHealVignette();
    }

    protected override void DisplayHealth()
    {
        base.DisplayHealth();
        if (_hpCursor)
            _hpCursor.rectTransform.anchoredPosition = Vector2.Lerp(_hpBarStart.anchoredPosition, _hpBarEnd.anchoredPosition, _hpUi.fillAmount);
    }

    public override void TakeDamage(int amount)
    {
        if (amount <= 1 || IsInvulnerable)
            return;

        base.TakeDamage(amount);
        ////PlaceHolderSoundManager.Instance.PlayHurt();
        ////SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/HitMarker", 1f);
        //cool magic numbers
        float shakeIntensity = _playerHurtShakeMaxStrength * Mathf.InverseLerp(0.0f, 40.0f, amount + 10.0f);
        Player.Instance.StartShake(shakeIntensity, _playerHurtShakeDuration);

        HandleLowHpVignette();
    }

    private void HandleLowHpVignette()
    {
        //* vignette starts being visible at [25%]HP and is at full opacity at [10%]hp
        float value = Mathf.Lerp(1.0f, 0.0f, Mathf.InverseLerp(_maxHp * 0.1f, _maxHp * 0.25f, _hp));
        _lowHpVignette.color = new Color(1.0f, 1.0f, 1.0f, value);
    }

    public override void ProbRegen(int amount = 10)
    {
        if (_hp < _probHp)
        {
            base.ProbRegen(amount);
            SoundManager.Instance.PlaySound("event:/SFX_Controller/CharactersNoises/BaseHeal", 2f, gameObject);
            StartHealVignette();
        }
    }

    private void StartHealVignette()
    {
        _isHealing = true;
        _healVignetteT = 0.0f;
    }

    private void HandleHealVignette()
    {
        if (_healVignetteT <= 1)
        {
            _healVignetteT += Time.deltaTime * _healVignetteSpeed;
            float alpha = _vignetteAlphaOnHeal.Evaluate(_healVignetteT);
            _healVignette.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }
        else
        {
            _healVignetteT = 1.0f;
            _healVignette.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            _isHealing = false;
        }
    }

    protected override void DisplayProbHealth()
    {
        base.DisplayProbHealth();
        float alpha = _vignetteAlphaOnProb.Evaluate(_probHp - _hp);
        _probVignette.color = new Color(1.0f, 1.0f, 1.0f, alpha);
    }

    public override void Knockback(Vector3 direction)
    {
        _player.AddMomentum(direction);
    }

    protected override void Death()
    {
        PlayerManager.Instance.StartDying();
    }

}
