using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AragonCloud : PooledObject
{
    [SerializeField] private Collider _boxCollider;
    [SerializeField] private VisualEffect _vfx;
    [SerializeField] private float _spawnDelay;

    [SerializeField] private bool _isNormal;
    [SerializeField] private bool _isPoisonous;
    [SerializeField] private bool _isWooshing;
    [SerializeField] float _maxLifeSpan = 4.0f;
    [SerializeField] float _lifeSpanT = 0.0f;

    [SerializeField] VisualEffect _vfxGraph;

    void Awake()
    {
        Disable();
    }

    public void Setup(Vector3 position, Quaternion rotation, float delay)
    {
        transform.position = position;
        transform.rotation = rotation;
        Synergies.Instance.ActiveClouds.Add(this);
        _spawnDelay = delay;
        _lifeSpanT = 0.0f;
    }

    void Update()
    {
        if (_isNormal)
        {

        }
        else if (_isWooshing)
        {

        }
        else if (_isPoisonous)
        {

        }
    }

    public void Swoosh(float delay)
    {
        _isNormal = false;
        _isWooshing = true;
        StartDisappearing();
    }

    public void Poisonify(float delay)
    {
        _isNormal = false;
        _isPoisonous = true;
        StartDisappearing();
    }

    private void Enable()
    {
        _boxCollider.enabled = true;
        _vfx.Reinit();
        _vfx.Play();
        _lifeSpanT = _maxLifeSpan;
    }

    public void StartDisappearing()
    {
        _vfx.Stop();
        _boxCollider.enabled = false;
        _lifeSpanT = -1.0f;
    }

    private void Disable()
    {
        _vfx.Stop();
    }
}