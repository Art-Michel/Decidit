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
        var vfx = _vfxPooler.Get().GetComponent<TwoPosTrail>();
        if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _hitscanMaxRange, _mask) && hit.transform.parent.TryGetComponent<Health>(out Health health))
        {
            vfx.SetPos(_canonPosition.position, hit.point);
            if (hit.transform.CompareTag("WeakHurtbox"))
                (health as EnemyHealth).TakeCriticalDamage(_hitscanDamage, hit.point, hit.normal);
            else
                (health as EnemyHealth).TakeDamage(_hitscanDamage, hit.point, hit.normal);
        }
        else
            vfx.SetPos(_canonPosition.position, _canonPosition.position + _camera.forward * _hitscanMaxRange);

        vfx.Play();

        PlaceHolderSoundManager.Instance.PlayRevolverShot();
        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);

        //va te faire enculer unity
        _muzzleFlash.PlayAll();

    }

    public void SpawnTrail()
    {

    }
}
