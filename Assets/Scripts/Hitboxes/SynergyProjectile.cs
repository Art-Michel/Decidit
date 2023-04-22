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


    private bool _synergized;

    public override void Setup(Vector3 position, Vector3 direction, Vector3 cameraDirection)
    {
        base.Setup(position, direction, cameraDirection);
        _synergized = false;
    }

    protected override void CheckForCollision()
    {
        if (!_synergized)
        {
            if (Physics.SphereCast(_lasterFramePosition, _radius, _spaceTraveledLast2Frames.normalized, out RaycastHit hit, _spaceTraveledLast2Frames.magnitude, _synergiesLayer))
            {
                Synergies.Instance.Synergize(this, hit.transform);
                _synergized = true;
            }
        }

        base.CheckForCollision();
    }
}
