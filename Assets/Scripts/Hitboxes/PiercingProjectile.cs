using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingProjectile : Projectile
{
    protected override void FixedUpdate()
    {
        float range = (_direction * _speed).magnitude;
        foreach (RaycastHit collider in Physics.SphereCastAll(transform.position, _radius, _direction, range, _shouldCollideWith))
            CheckForHit(collider.transform);
        transform.position += _direction * _speed;
    }
}
