
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledVfx : PooledObject
{
    [SerializeField] float _lifeSpan;
    float _lifeT;

    void OnEnable()
    {
        _lifeT = _lifeSpan;
    }

    void Update()
    {
        _lifeT -= Time.deltaTime;
        if (_lifeT <= 0)
            this.Pooler.Return(this);
    }

}