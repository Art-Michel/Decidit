using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MuseEylauExplosions : PooledObject
{
    float _delay = 0.0f;
    float _lifeSpan = 0.0f;
    [SerializeField] VisualEffect _vfx;
    [SerializeField] Hitbox _hitbox;
    bool _hasExploded;


    public void Setup(Vector3 position, float delay)
    {
        transform.position = position;
        _delay = delay;
        _hasExploded = false;
        _lifeSpan = 2.0f;
        _hitbox.enabled = false;
    }

    void Update()
    {
        if (!_hasExploded)
        {
            _delay -= Time.deltaTime;
            if (_delay <= 0.0f)
            {
                _hasExploded = true;
                _vfx.Play();
                _hitbox.enabled = true;
            }
        }
        else
        {
            _lifeSpan -= Time.deltaTime;
            if (_lifeSpan <= 0.0f)
            {
                _vfx.Reinit();
                base.Pooler.Return(this);
            }
        }
    }
}