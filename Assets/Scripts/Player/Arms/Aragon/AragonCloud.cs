using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AragonCloud : PooledObject
{
    [SerializeField] private Collider _boxCollider;
    [SerializeField] private VisualEffect _vfx;
    private float _delay;

    private bool _isActive;
    const float _maxLifeSpan = 4.0f;
    float _lifeSpan = 0.0f;

    void Awake()
    {
        Disable();
    }

    public void Setup(Vector3 position, Quaternion rotation, float delay)
    {
        transform.position = position;
        transform.rotation = rotation;
        _delay = delay;
        _lifeSpan = _maxLifeSpan;
        _isActive = false;
    }

    void Update()
    {
        if (_isActive!)
        {
            _delay -= Time.deltaTime;
            if (_delay <= 0.0f)
                Enable();
        }
        else
        {
            _lifeSpan -= Time.deltaTime;
            if (_delay <= 0.0f)
            {
                Disable();
                this.Pooler.Return(this);
            }
        }
    }

    private void Enable()
    {
        _boxCollider.enabled = true;
        _vfx.enabled = true;
        _isActive = true;
    }

    private void Disable()
    {
        _boxCollider.enabled = false;
        _vfx.enabled = false;
        _isActive = false;
    }
}
