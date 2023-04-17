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

    public void Setup(Vector3 position, Quaternion rotation, float delay)
    {
        transform.position = position;
        transform.rotation = rotation;
        _delay = delay;
        _lifeSpan = _maxLifeSpan + delay;
        _isActive = false;
    }

    void Update()
    {
        if (_isActive!)
        {
            _delay -= Time.deltaTime;
            if (_delay <= 0.0f)
            {
                _boxCollider.enabled = true;
                _vfx.enabled = true;
                _isActive = true;
            }
        }
        else
        {

        }
    }
}
