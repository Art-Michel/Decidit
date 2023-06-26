using System;
using UnityEngine;

public class BulletHealth : Health
{
    public override bool TakeDamage(float damage)
    {
        return true;
        //Explose
    }
}
