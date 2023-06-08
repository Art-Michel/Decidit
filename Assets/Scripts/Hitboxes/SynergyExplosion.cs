using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class SynergyExplosion : Explosion
{
    [Foldout("Synergies")]
    public Synergies.Chants Chant;
    [Foldout("Synergies")]
    [SerializeField] LayerMask _synergiesLayer;

    protected bool _synergized;

    protected override void OnEnable()
    {
        base.OnEnable();

        _synergized = false;
    }

    protected override void CheckForCollision()
    {
        if (!_synergized)
            foreach (Collider collider in Physics.OverlapSphere(transform.position, _radius, _synergiesLayer))
            {
                Debug.Log("mario");
                Synergies.Instance.Synergize(this, collider.transform);
                _synergized = true;
            }
        base.CheckForCollision();
    }
}