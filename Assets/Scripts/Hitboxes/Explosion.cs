using System;
using System.Collections;
using System.Collections.Generic;
using CameraShake;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class Explosion : Hitbox
{
    [Foldout("Explosion properties")]
    [SerializeField] private float _lifeSpan = 1f;
    [Foldout("Explosion properties")]
    [SerializeField] private float _hitboxSpan = .2f;
    // [Foldout("Explosion properties")]
    // [SerializeField] private float _shakeIntensity = 1;
    // [Foldout("Explosion properties")]
    // [SerializeField] private float _shakeRange = 50f;
    // [Foldout("Explosion properties")]
    // [SerializeField] private float _shakeDuration = .5f;
    [SerializeField] private PerlinShake.Params _shake;

    [Foldout("Explosion References")]
    [SerializeField] private Light _light;
    [Foldout("Explosion References")]
    [SerializeField] private GameObject _windBox;
    [Foldout("Explosion References")]
    [SerializeField] private VFX_Particle _explosionVfx;
    [Foldout("Explosion References")]
    [SerializeField] private AudioSource _explosionAudioSource;
    [Foldout("Explosion References")]
    [SerializeField] private AudioClip _explosionSfx;


    private bool _hitboxIsActive;
    private Projectile _parentProjectile;
    private float _lifeT;
    private float _hitboxT;

    private float _lightStrength;

    private float _initialKnockbackForce;
    private float _initialDamage;

    protected override void Awake()
    {
        base.Awake();
        _parentProjectile = transform.parent.GetComponent<Projectile>();
        _initialKnockbackForce = _knockbackForce;
        _initialDamage = Damage;
    }

    void OnEnable()
    {
        _lightStrength = _light.intensity;
        Reset();
    }

    private void Reset()
    {
        if (_windBox)
            _windBox.SetActive(true);

        _hitboxIsActive = true;
        _light.intensity = _lightStrength;
        _lifeT = _lifeSpan;
        _hitboxT = _hitboxSpan;
        _knockbackForce = _initialKnockbackForce;
        Damage = (int)_initialDamage;
        //!SFX Explosion 
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/MuseMalade/Explosion", 5f, gameObject);
        StartExplosionShake();
        ClearBlacklist();
    }

    private void PlayMuseExplosion()
    {
        _explosionAudioSource.PlayOneShot(_explosionSfx, 1f);
    }

    private void StartExplosionShake()
    {
        // float intensity = _shakeIntensity * Mathf.InverseLerp(_shakeRange, 0, Vector3.Distance(Player.Instance.transform.position, transform.position));
        // float duration = _shakeDuration * Mathf.InverseLerp(_shakeRange, 0, Vector3.Distance(Player.Instance.transform.position, transform.position));
        Player.Instance.StartPerlinShake(_shake, transform.position);

    }

    protected override void Update()
    {
        if (_hitboxIsActive)
            base.Update();

        //Object life span (equal to duration of the VFX)
        _lifeT -= Time.deltaTime;
        if (_lifeT <= 0)
        {
            gameObject.SetActive(false);
            _parentProjectile.StartDisappearing();
        }

        //Explosion life span
        _hitboxT -= Time.deltaTime;
        if (_hitboxT <= 0 && _hitboxIsActive)
        {
            DisableHitboxes();
        }

        //Explosion gets weaker over time
        _knockbackForce = _initialKnockbackForce * Mathf.InverseLerp(0f, _hitboxSpan, _hitboxT);
        Damage = Mathf.RoundToInt(_initialDamage * Mathf.InverseLerp(0f, _hitboxSpan, _hitboxT));
        _light.intensity = _lightStrength * Mathf.InverseLerp(0, _lifeSpan, _lifeT);

        //Check for collisions with hitbox
    }

    private void DisableHitboxes()
    {
        _hitboxIsActive = false;
        if (_windBox)
            _windBox.SetActive(false);
    }
}
