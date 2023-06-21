
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledVfx : PooledObject
{
    [SerializeField] float _lifeSpan;
    float _lifeT;
    // private VFX_Particle _vfxparticle;

    // void Awake()
    // {
    //     TryGetComponent<VFX_Particle>(out _vfxparticle);
    // }

    void OnEnable()
    {
        _lifeT = _lifeSpan;
        // if (_vfxparticle != null)
        //     _vfxparticle.PlayAll();
    }

    void Update()
    {
        _lifeT -= Time.deltaTime;
        if (_lifeT <= 0 && this.Pooler != null)
            this.Pooler.Return(this);
    }

}