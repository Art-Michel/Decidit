using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class BaseRevolver : Revolver
{
    [Foldout("Stats")]
    [SerializeField] int _hitscanDamage = 25;
    [Foldout("Stats")]
    [SerializeField] float _hitscanMaxRange = 100f;

    public override void Shoot()
    {
        var vfx = _trailVfxPooler.Get().GetComponent<TwoPosTrail>();
        if (Physics.Raycast(_camera.position, (_currentlyAimedAt - _camera.position), out RaycastHit hit, _hitscanMaxRange, _mask + _secondaryMask))
        {
            vfx.SetPos(_canonPosition.position, hit.point);

            //wall or ground
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/BaseShoot/BaseShootImpactObject", 1f, gameObject);
                GameObject impactVfx = _impactVfxPooler.Get().gameObject;
                impactVfx.transform.position = hit.point + hit.normal * 0.05f;
                impactVfx.transform.forward = -hit.normal;
            }

            //flesh
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Flesh"))
            {
                SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/BaseShoot/BaseShootImpactFlesh", 1f, gameObject);
                GameObject splashVfx = _fleshSplashVfxPooler.Get().gameObject;
                splashVfx.transform.position = hit.point + hit.normal * 0.05f;
                splashVfx.transform.forward = hit.normal;
            }

            // = enemy hurtbox
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyHurtbox") && hit.transform.parent.TryGetComponent<Health>(out Health health))
            {
                if (hit.transform.CompareTag("WeakHurtbox"))
                    (health as EnemyHealth).TakeCriticalDamage(_hitscanDamage, hit.point, hit.normal);
                else
                    (health as EnemyHealth).TakeDamage(_hitscanDamage, hit.point, hit.normal);
            }
        }

        else
            vfx.SetPos(_canonPosition.position, _canonPosition.position + _camera.forward * _hitscanMaxRange);

        vfx.Play();

        ////PlaceHolderSoundManager.Instance.PlayRevolverShot();
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/BaseShoot/BaseShoot", 1f, gameObject);
        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);

        //va te faire enculer unity
        _muzzleFlash.PlayAll();

    }

    public void SpawnTrail()
    {

    }
}
