using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class SynergyHitbox : Hitbox
{
    [Foldout("Synergies")]
    public Synergies.Chants Chant;
    [Foldout("Synergies")]
    [SerializeField] LayerMask _synergiesLayer;

    protected bool _synergized;

    void OnEnable()
    {
        _synergized = false;
    }

    protected override void CheckForCollision()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, _radius, _synergiesLayer))
        {
            Synergies.Instance.Synergize(this, collider.transform);
            _synergized = true;
        }
        base.CheckForCollision();
    }
}
