using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using NaughtyAttributes;

public class VFX_Particle : MonoBehaviour
{
    [SerializeField]
    private VisualEffect _vfx;
    [SerializeField]
    private List<ParticleSystem> _particleSystems;

    [Button]
    public void PlayAll()
    {
        _vfx.Play();
        foreach (ParticleSystem system in _particleSystems)
            system.Play();
    }
}