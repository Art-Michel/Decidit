using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AragonCloud : PooledObject
{
    [SerializeField] private Collider _boxCollider;
    [SerializeField] private VisualEffect _vfx;
    [SerializeField] private float _delay;

    [SerializeField] private bool _isActive;
    [SerializeField] private bool _isDisappearing;
    [SerializeField] float _maxLifeSpan = 4.0f;
    [SerializeField] float _lifeSpan = 0.0f;

    void Awake()
    {
        Disable();
    }

    public void Setup(Vector3 position, Quaternion rotation, float delay)
    {
        transform.position = position;
        transform.rotation = rotation;
        Synergies.Instance.ActiveClouds.Add(this);
        _delay = delay;
        _isActive = false;
    }

    void Update()
    {
        if (!_isActive)
        {
            _delay -= Time.deltaTime;
            if (_delay <= 0.0f)
                Enable();
        }
        else
        {
            _lifeSpan -= Time.deltaTime;
            if (_lifeSpan <= 0.0f && !_isDisappearing)
            {
                StartDisappearing();
            }
            if (_isDisappearing && _vfx.aliveParticleCount <= 0)
            {
                Disable();
                this.Pooler.Return(this);
            }

        }
    }

    private void Enable()
    {
        _boxCollider.enabled = true;
        _vfx.Reinit();
        _vfx.Play();
        _lifeSpan = _maxLifeSpan;
        _isActive = true;
    }

    public void StartDisappearing()
    {
        _vfx.Stop();
        _isDisappearing = true;
        _boxCollider.enabled = false;
        _lifeSpan = -1.0f;
    }

    private void Disable()
    {
        _vfx.Stop();
        _isActive = false;
        _isDisappearing = false;
    }

}
