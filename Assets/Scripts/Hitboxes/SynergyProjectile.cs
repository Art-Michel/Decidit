using System.Runtime.Serialization.Formatters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SynergyProjectile : Projectile
{
    [Foldout("Synergies")]
    public Synergies.Chants Chant;
    [Foldout("Synergies")]
    [SerializeField] LayerMask _synergiesLayer;

    protected override void CheckForCollision()
    {
        if (Physics.Raycast(_lasterFramePosition, _spaceTraveledLast2Frames.normalized, out RaycastHit hit, _spaceTraveledLast2Frames.magnitude, _synergiesLayer))
        {
            Synergies.Instance.Synergize(this, hit.transform);
        }
        base.CheckForCollision();
    }
}
