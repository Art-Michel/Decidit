using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class FugueProjectile : SynergyProjectile
{
    [SerializeField] bool _centered;
    [SerializeField] ProjectileOscillator[] _objects;
    [SerializeField] Vector3[] _directions;
    [SerializeField] float[] _offsets;

    void OnEnable()
    {
        for (int i = 0; i < _objects.Length; i++)
        {
            _objects[i].Setup(_directions[i], _centered, _offsets[i]);
        }
    }

}