using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class AragonCloud : PooledObject
{
    [Foldout("Refs")]
    [SerializeField] private Collider _boxCollider;
    [Foldout("Refs")]
    [SerializeField] private VisualEffect _vfx;

    [Foldout("Spawn")]
    [SerializeField] private float _spawnDelay;
    [Foldout("Spawn")]
    [SerializeField] private float _delayT;

    [Foldout("Despawn")]
    [SerializeField] private float _deathSpan = 1.05f;
    [Foldout("Despawn")]
    [SerializeField] private float _deathSpanT = 0.0f;

    [Foldout("State")]
    [SerializeField] private bool _isNormal;
    [Foldout("State")]
    [SerializeField] private bool _isPoisonous;
    [Foldout("State")]
    [SerializeField] private bool _isWooshing;
    [Foldout("State")]
    [SerializeField] private bool _isDying;

    [Foldout("Vibe")]
    [SerializeField] float _normalLifeSpan = 4.0f;
    [Foldout("Vibe")]
    [SerializeField] float _lifeSpanT = 0.0f;

    [Foldout("Poison")]
    [SerializeField] float _greenTransitionSpeed = 0.3f;
    [Foldout("Poison")]
    [SerializeField] float _greenTransitionDelay = 0.0f;
    [Foldout("Poison")]
    [SerializeField] float _poisonLifeSpan = 4.0f;
    [Foldout("Poison")]
    [SerializeField] Hitbox _poisonBox;

    [Foldout("Swoosh")]
    [SerializeField] float _eylauLifeSpan = 1.0f;
    [Foldout("Swoosh")]
    [SerializeField] float _eylauTransitionSpeed = 0.3f;


    public void Setup(Vector3 position, Quaternion rotation, float delay)
    {
        transform.position = position;
        transform.rotation = rotation;
        Synergies.Instance.ActiveClouds.Add(this);

        _spawnDelay = delay;
        _delayT = 0.0f;
        Enable();


        _vfx.SetFloat("WooshingMultiplier", 0);
        _vfx.SetFloat("WooshingMultiplierMin", 0);
        _vfx.SetFloat("WooshingMultiplierMax", 0);
        _vfx.SetFloat("SmokeSpawnRate", 15);
        _vfx.SetFloat("SparksSpawnRate", 12);

        _vfx.SetFloat("Fugue To Muse", 0);
        _vfx.SetFloat("Fugue To Eylaw", 0);
    }

    private void Enable()
    {
        _boxCollider.enabled = true;
        _vfx.Reinit();
        _vfx.Play();

        _isNormal = true;
        _isPoisonous = false;
        _poisonBox.enabled = false;
        _isWooshing = false;
        _isDying = false;

        _lifeSpanT = 0.0f;
    }

    void Update()
    {
        if (_isNormal)
        {
            _lifeSpanT += Time.deltaTime;
            if (_lifeSpanT >= _normalLifeSpan)
            {
                StartDisappearing();
            }
        }
        else if (_isWooshing)
        {
            _lifeSpanT += Time.deltaTime;
            float colorLerp = Mathf.InverseLerp(0, _eylauTransitionSpeed, _lifeSpanT);
            _vfx.SetFloat("Fugue To Eylaw", colorLerp);

            if (_lifeSpanT >= _eylauLifeSpan)
            {
                StartDisappearing();
            }
        }
        else if (_isPoisonous)
        {
            _lifeSpanT += Time.deltaTime;
            float colorLerp = Mathf.InverseLerp(_greenTransitionDelay, _greenTransitionDelay + _greenTransitionSpeed, _lifeSpanT);
            _vfx.SetFloat("Fugue To Muse", colorLerp);

            if (_lifeSpanT >= _poisonLifeSpan)
            {
                StartDisappearing();
            }
        }
        else if (_isDying)
        {
            _deathSpanT += Time.deltaTime;
            if (_deathSpanT >= _deathSpan)
                this.Pooler.Return(this);
        }
        else
        {
            _delayT += Time.deltaTime;
            if (_delayT >= _spawnDelay)
            {
                Enable();
            }
        }
    }

    public void Swoosh()
    {
        _isNormal = false;
        _isWooshing = true;
        _lifeSpanT = 0.0f;

        _boxCollider.enabled = false;
        _vfx.SetFloat("WooshingMultiplierMin", 15);
        _vfx.SetFloat("WooshingMultiplierMax", 20);
        _vfx.SetFloat("SmokeSpawnRate", 25);
        _vfx.SetFloat("SparksSpawnRate", 20);
        transform.Rotate(new Vector3(0, 0, 90));
    }

    public void Poisonify(float delay)
    {
        _isNormal = false;
        _isPoisonous = true;
        _lifeSpanT = 0.0f;

        _boxCollider.enabled = false;
        _greenTransitionDelay = delay;
        _poisonBox.enabled = true;

        // StartDisappearing(); //TODO
    }

    public void StartDisappearing()
    {
        _isNormal = false;
        _isWooshing = false;
        _isPoisonous = false;
        _poisonBox.enabled = false;
        _isDying = true;

        _deathSpanT = 0.0f;
        _vfx.Stop();
        _boxCollider.enabled = false;
    }

}