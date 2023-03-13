using System.Globalization;
using System;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class PlayerHealth : Health
{
    public static PlayerHealth Instance;

    [Foldout("References")]
    [SerializeField] Player _player;

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
    private const float _healVignetteSpeed = 1.5f;

    [Foldout("References")]
    [SerializeField] Image _damageVignette;
    [Foldout("Properties")]
    [SerializeField] AnimationCurve _vignetteAlphaOnDamage;
    private float _damageVignetteT;
    private bool _isBeingDamaged;
    private const float _damageVignetteSpeed = 2.5f;
    private float _currentDamageVignetteMaxAlpha;

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
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        Instance = GetComponent<PlayerHealth>();
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        if (_isHealing) HandleHealVignette();
        if (_isBeingDamaged) HandleDamageVignette();
    }

    public override void TakeDamage(int amount)
    {
        if (amount <= 1 || IsInvulnerable)
            return;

        base.TakeDamage(amount);
        SoundManager.Instance.PlaySound("event:/SFX_Controller/CharactersNoises/DamageTaken", 4f, gameObject);

        //cool magic numbers proportionnal screenshake when getting hurt
        float shakeIntensity = _playerHurtShakeMaxStrength * Mathf.InverseLerp(0.0f, 40.0f, amount + 10.0f);
        Player.Instance.StartShake(shakeIntensity, _playerHurtShakeDuration);

        HandleLowHpVignette();
        StartDamageVignette(amount);
        DisplayProbHealth();
    }

    public void ResetProbStartup()
    {
        _probationStartup = _probationMaxStartup;
    }

    private void HandleLowHpVignette()
    {
        //* vignette starts being visible at [25%]HP and is at full opacity at [10%]hp
        float value = Mathf.Lerp(1.0f, 0.0f, Mathf.InverseLerp(_maxHp * 0.1f, _maxHp * 0.25f, _hp));
        _lowHpVignette.color = new Color(1.0f, 1.0f, 1.0f, value);
    }

    public override void ProbRegen(int i = 100)
    {
        if (_hp < _probHp)
        {
            base.ProbRegen(100);
            SoundManager.Instance.PlaySound("event:/SFX_Controller/CharactersNoises/BaseHeal", 3f, gameObject);
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

    private void StartDamageVignette(float damage)
    {
        _isBeingDamaged = true;
        _damageVignetteT = 0.0f;

        //*Minimum vignette intensity from 5hp lost (30% opacity) to 20 hp lost (100% opacity)
        _currentDamageVignetteMaxAlpha = Mathf.Lerp(0.3f, 1.0f, Mathf.InverseLerp(5.0f, 20.0f, damage));
    }

    private void HandleDamageVignette()
    {
        if (_damageVignetteT <= 1)
        {
            _damageVignetteT += Time.deltaTime * _damageVignetteSpeed;
            float alpha = _vignetteAlphaOnDamage.Evaluate(_damageVignetteT);
            _damageVignette.color = new Color(1.0f, 0.45f, 0.0f, alpha * _currentDamageVignetteMaxAlpha);
        }
        else
        {
            _damageVignetteT = 1.0f;
            _damageVignette.color = new Color(0.7f, 0.0f, 0.0f, 0.0f);
            _isBeingDamaged = false;
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

    protected virtual void OnDestroy()
    {
        Instance = null;
    }
}
