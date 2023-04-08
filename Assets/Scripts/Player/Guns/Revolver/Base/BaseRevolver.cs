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
        base.Shoot();
        var vfx = _trailVfxPooler.Get().GetComponent<TwoPosTrail>();
        if (Physics.Raycast(_camera.position, (_currentlyAimedAt - _camera.position), out RaycastHit hit, _hitscanMaxRange, _mask + _secondaryMask))
        {
            vfx.SetPos(_canonPosition.position, hit.point);

            //wall or ground
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                GameObject impactVfx = _impactVfxPooler.Get().gameObject;
                impactVfx.transform.position = hit.point + hit.normal * 0.05f;
                impactVfx.transform.forward = -hit.normal;
                SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/BaseShoot/BaseShootImpactObject", 1f, impactVfx.gameObject);
            }

            //flesh
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Flesh"))
            {
                GameObject splashVfx = _fleshSplashVfxPooler.Get().gameObject;
                splashVfx.transform.position = hit.point + hit.normal * 0.05f;
                splashVfx.transform.forward = hit.normal;
                SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/BaseShoot/BaseShootImpactFlesh", 1f, splashVfx.gameObject);
            }

            // = enemy hurtbox
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyHurtbox") && hit.transform.TryGetComponent<Hurtbox>(out Hurtbox hurtbox))
            {
                if (hit.transform.CompareTag("WeakHurtbox"))
                    (hurtbox.HealthComponent as EnemyHealth).TakeCriticalDamage(_hitscanDamage, hit.point, hit.normal);
                else
                    (hurtbox.HealthComponent as EnemyHealth).TakeDamage(_hitscanDamage, hit.point, hit.normal);
            }
        }

        else
            vfx.SetPos(_canonPosition.position, _canonPosition.position + _camera.forward * _hitscanMaxRange);

        vfx.Play();

        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/BaseShoot/BaseShoot", 1f, gameObject);
        Player.Instance.StartKickShake(_shootShake, transform.position);

        _muzzleFlash.PlayAll();

        if (!sh1)
        {
            sh1 = true;
            Animator.CrossFade("shoot", 0, 0);
        }
        else
        {
            sh1 = false;
            Animator.CrossFade("shooot", 0, 0);
        }
    }

    public void SpawnTrail()
    {

    }
}
