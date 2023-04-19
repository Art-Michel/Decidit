using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : PooledObject
{
    [SerializeField] private float _activeTime = 2.0f;
    // [SerializeField] private float _lifeSpan = 1.0f;
    [SerializeField] private float _radius = 12.0f;
    [SerializeField] private float _force = 10.0f;
    [SerializeField] private ParticleSystem[] _vfxs;

    private float _activeT;
    private bool _isActive;

    public void Setup()
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Synergies/AragonOnEyleau/Sound", 1f, gameObject);
        foreach (ParticleSystem vfx in _vfxs)
        {
            vfx.Play();
        }

        _isActive = true;
        _activeT = _activeTime;

        CheckCollisions();
    }

    void Update()
    {
        if (_isActive)
        {
            _activeT -= Time.deltaTime;
            if (_activeT <= 0.0f)
                Disable();
        }
        else
        {
            bool b = true;
            foreach (ParticleSystem vfx in _vfxs)
            {
                b = b && vfx.particleCount <= 0;
            }
            if (b)
                Pooler.Return(this);
        }
    }

    private void CheckCollisions()
    {

    }

    private void Disable()
    {
        _isActive = false;
        foreach (ParticleSystem vfx in _vfxs)
            vfx.Stop();
    }
}
