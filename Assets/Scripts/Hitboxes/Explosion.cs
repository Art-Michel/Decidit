using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Explosion : Hitbox
{
    [SerializeField] private float _lifeSpan = 1f;
    private Projectile _parentProjectile;
    private float _lifeT;
    [SerializeField] private VisualEffect _explosionVfx;
    [SerializeField] private AudioSource _explosionAudioSource;
    [SerializeField] private AudioClip _explosionSfx;

    protected override void Awake()
    {
        base.Awake();
        _parentProjectile = transform.parent.GetComponent<Projectile>();
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
        PlayMuseExplosion();

        ClearBlacklist();
    }

    private void PlayMuseExplosion()
    {
        _explosionAudioSource.PlayOneShot(_explosionSfx, 1f);
    }

    protected override void Update()
    {
        base.Update();
        _lifeT -= Time.deltaTime;
        if (_lifeT <= 0)
        {
            gameObject.SetActive(false);
            _explosionVfx.Play();
            _parentProjectile.Disappear();
        }
    }
}
