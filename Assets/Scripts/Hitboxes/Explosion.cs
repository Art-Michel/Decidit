using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Explosion : Hitbox
{
    private Projectile _parentProjectile;
    private float _lifeT;

    private float _initialKnockbackForce;
    private float _initialDamage;
    [SerializeField] private float _lifeSpan = 1f;
    [SerializeField] private VFX_Particle _explosionVfx;
    [SerializeField] private AudioSource _explosionAudioSource;
    [SerializeField] private AudioClip _explosionSfx;
    [SerializeField] private float _shakeIntensity;
    [SerializeField] private float _shakeRange;
    [SerializeField] private float _shakeDuration;

    protected override void Awake()
    {
        base.Awake();
        _parentProjectile = transform.parent.GetComponent<Projectile>();
        _initialKnockbackForce = _knockbackForce;
        _initialDamage = _damage;
    }

    void Start()
    {
        Reset();
    }

    void OnEnable()
    {
        Reset();
    }

    private void Reset()
    {
        _lifeT = _lifeSpan;
        _knockbackForce = _initialKnockbackForce;
        _damage = (int)_initialDamage;
        PlayMuseExplosion();
        StartExplosionShake();

        ClearBlacklist();
    }

    private void PlayMuseExplosion()
    {
        _explosionAudioSource.PlayOneShot(_explosionSfx, 1f);
    }

    private void StartExplosionShake()
    {
        float intensity = _shakeIntensity * Mathf.InverseLerp(_shakeRange, 0, Vector3.Distance(Player.Instance.transform.position, transform.position));
        float duration = _shakeDuration * Mathf.InverseLerp(_shakeRange, 0, Vector3.Distance(Player.Instance.transform.position, transform.position));
        Player.Instance.StartShake(intensity, duration);

    }

    protected override void Update()
    {
        base.Update();
        _lifeT -= Time.deltaTime;
        if (_lifeT <= 0)
        {
            gameObject.SetActive(false);
            // if (_explosionVfx)
            // {
            //     _explosionVfx.PlayAll();
            // }
            _parentProjectile.Disappear();
        }

        //Explosion gets weaker over time
        _knockbackForce = _initialKnockbackForce * Mathf.InverseLerp(_lifeSpan, 1f, _lifeT);
        _damage = Mathf.RoundToInt(_initialDamage * Mathf.InverseLerp(_lifeSpan, 1f, _lifeT));
    }
}
