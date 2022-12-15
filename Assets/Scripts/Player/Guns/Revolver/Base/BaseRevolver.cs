using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BaseRevolver : Revolver
{
    [Header("References")]
    [SerializeField] Pooler _vfxPooler;

    [Header("Stats")]
    [SerializeField] int _hitscanDamage = 25;
    [SerializeField] float _hitscanMaxRange = 100f;

    public override void Shoot()
    {
        if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _hitscanMaxRange, _mask) && hit.transform.parent.TryGetComponent<Health>(out Health health))
        {
            if (hit.transform.CompareTag("WeakHurtbox"))
                (health as EnemyHealth).TakeCriticalDamage(_hitscanDamage, hit.point, hit.normal);
            else
                (health as EnemyHealth).TakeDamage(_hitscanDamage, hit.point, hit.normal);
        }

        //TODO pool un vfx de ligne
        //TODO VFX.pos1 = _canon.position
        //TODO VFX.pos2 = hit.point
        PlaceHolderSoundManager.Instance.PlayRevolverShot();
        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
        _muzzleFlash.Play();
    }
}
