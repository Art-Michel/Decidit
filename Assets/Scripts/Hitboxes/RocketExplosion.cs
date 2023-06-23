using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketExplosion : Explosion
{
    protected override void Hit(Transform targetCollider)
    {
        Debug.Log("ExplHit");
        EnemyHealth enemyHealth = targetCollider.GetComponent<Hurtbox>().HealthComponent as EnemyHealth;
        if (enemyHealth)
            enemyHealth.GetSick();
        base.Hit(targetCollider);
    }
}
