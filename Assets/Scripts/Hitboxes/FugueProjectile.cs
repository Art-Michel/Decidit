using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class FugueProjectile : SynergyProjectile
{
    [SerializeField] bool _centered;
    [SerializeField] ProjectileOscillator[] _objects;
    [SerializeField] Vector3[] _directions;

    public override void Setup(Vector3 position, Vector3 direction, Vector3 cameraDirection)
    {
        base.Setup(position, direction, cameraDirection);
        transform.rotation = Camera.main.transform.parent.rotation;
        SetupOscillatingTrails();
    }

    void SetupOscillatingTrails()
    {
        for (int i = 0; i < _objects.Length; i++)
        {
            Vector3 direction = transform.right * _directions[i].x + transform.up * _directions[i].y + transform.forward * _directions[i].z;
            _objects[i].Setup(direction.normalized, _centered);
        }
    }
}