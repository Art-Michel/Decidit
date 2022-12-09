using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodVFX : PooledObject
{
    private void OnParticleSystemStopped()
    {
        this._pooler.Return(this);
    }
}
