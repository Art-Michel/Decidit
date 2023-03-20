using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketExplosion : Explosion
{
    protected override void Hit(Transform targetCollider)
    {
        base.Hit(targetCollider);
        if (targetCollider.parent.TryGetComponent<EnemyHealth>(out EnemyHealth health))
        {
            health.GetSick();
        }
    }
}
