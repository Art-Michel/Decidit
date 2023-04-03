using System;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using NaughtyAttributes;
using CameraShake;

public class EylauRevolver : Revolver
{
    [Foldout("References")]
    [SerializeField] private Pooler _unchargedProjectilePooler;
    [Foldout("References")]
    [SerializeField] private Pooler _chargedProjectilePooler;
    [Foldout("References")]
    [SerializeField] private Image _chargeUi;

    [SerializeField] private BounceShake.Params _laserShake;
    [Foldout("Stats")]
    [SerializeField] private float _chargeSpeed = 1f;
    [Foldout("Stats")]
    [SerializeField] private float _chargedWeaponShakeIntensity;
    [Foldout("Stats")]
    [SerializeField] private float _laserAdditionalRecoil = .3f;
    // [Foldout("Stats")]
    // [SerializeField] private int _laserMaxRange;
    // [Foldout("Stats")]
    // [SerializeField] private int _laserDamage;
    // [Foldout("Stats")]
    // [SerializeField] private bool _laserShouldPierce;

    private float _currentCharge;
    private bool _charged;
    private Vector3 _shakenDirection = Vector3.zero;
    private FMOD.Studio.EventInstance loopInstance;

    public override void UpdateChargeLevel()
    {
        if (_charged)
        {
            transform.localPosition -= _shakenDirection;
            _shakenDirection = new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), 0).normalized * _chargedWeaponShakeIntensity;
            transform.localPosition += _shakenDirection;
        }
        else
        {
            _currentCharge = Mathf.Clamp(_currentCharge + Time.deltaTime * _chargeSpeed, 0, 1);
            _chargeUi.fillAmount = _currentCharge;

            if (_currentCharge >= 1)
                GetCharged();
        }
    }

    private void GetCharged()
    {
        loopInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Controller/Shoots/CimetièreEyleau/MaxChargedLoop");
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/CimetièreEyleau/MaxCharged", 1f, gameObject);
        loopInstance.start();
        //jouer en boucle un son quand il est chargé genre vwooooom
        _charged = true;
    }

    public override void Shoot()
    {
        PooledObject shot = null;
        if (!_charged)
        {
            if (!sh1)
            {
                sh1 = true;
                Animator.CrossFade("unShot", 0, 0);
            }
            else
            {
                sh1 = false;
                Animator.CrossFade("unShot2", 0, 0);
            }

            loopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            loopInstance.release();
            SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/CimetièreEyleau/BasedShoot", 1f, gameObject);
            shot = _unchargedProjectilePooler.Get();
            shot.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized);

            Player.Instance.StartKickShake(_shootShake, transform.position);
            _muzzleFlash.PlayAll();
        }

        else
        {
            Animator.CrossFade("chShot", 0, 0);
            // if (_laserShouldPierce)
            //     PiercingLaser();
            // else
            //Laser();
            loopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            loopInstance.release();
            SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/CimetièreEyleau/ChargedShoot", 1f, gameObject);
            shot = _chargedProjectilePooler.Get();
            shot.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized);


            Player.Instance.StartBounceShake(_laserShake, transform.position);
            _muzzleFlash.PlayAll();
            //Additionnal ammo cost
        }

    }

    // private void Laser()
    // {
    //     var vfx = _laserVfxPooler.Get().GetComponent<TwoPosTrail>();
    //     if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _laserMaxRange, _mask))
    //     {
    //         vfx.SetPos(_canonPosition.position, hit.point);

    //         //wall or ground
    //         if (hit.transform.gameObject.layer == 9)
    //         {
    //             GameObject impactVfx = _impactVfxPooler.Get().gameObject;
    //             impactVfx.transform.position = hit.point + hit.normal * 0.05f;
    //             impactVfx.transform.forward = -hit.normal;
    //         }

    //         //flesh
    //         if (hit.transform.gameObject.layer == 18)
    //         {
    //             GameObject splashVfx = _fleshSplashVfxPooler.Get().gameObject;
    //             splashVfx.transform.position = hit.point + hit.normal * 0.05f;
    //             splashVfx.transform.forward = hit.normal;
    //         }

    //         // = enemy hurtbox
    //         else if (hit.transform.gameObject.layer == 15 && hit.transform.parent.TryGetComponent<Health>(out Health health))
    //         {
    //             vfx.SetPos(_canonPosition.position, hit.point);
    //             if (hit.transform.CompareTag("WeakHurtbox"))
    //                 (health as EnemyHealth).TakeCriticalDamage(_laserDamage, hit.point, hit.normal);
    //             else
    //                 (health as EnemyHealth).TakeDamage(_laserDamage, hit.point, hit.normal);
    //         }
    //     }

    //     else
    //         vfx.SetPos(_canonPosition.position, _canonPosition.position + _camera.forward * _laserMaxRange);

    //     vfx.Play();
    // }

    // private void PiercingLaser()
    // {
    //     var vfx = _laserVfxPooler.Get().GetComponent<TwoPosTrail>();
    //     vfx.SetPos(_canonPosition.position, _canonPosition.position + _camera.forward * _laserMaxRange);
    //     vfx.Play();

    //     RaycastHit[] hits = Physics.RaycastAll(_camera.position, _camera.forward, _laserMaxRange, _mask);
    //     foreach (RaycastHit hit in hits)
    //     {
    //         //wall or ground
    //         if (hit.transform.gameObject.layer == 9)
    //         {
    //             GameObject impactVfx = _impactVfxPooler.Get().gameObject;
    //             impactVfx.transform.position = hit.point + hit.normal * 0.05f;
    //             impactVfx.transform.forward = -hit.normal;
    //         }

    //         //flesh
    //         if (hit.transform.gameObject.layer == 18)
    //         {
    //             GameObject splashVfx = _fleshSplashVfxPooler.Get().gameObject;
    //             splashVfx.transform.position = hit.point + hit.normal * 0.05f;
    //             splashVfx.transform.forward = hit.normal;
    //         }

    //         // = enemy hurtbox
    //         else if (hit.transform.gameObject.layer == 15 && hit.transform.parent.TryGetComponent<Health>(out Health health))
    //         {
    //             vfx.SetPos(_canonPosition.position, hit.point);
    //             if (hit.transform.CompareTag("WeakHurtbox"))
    //                 (health as EnemyHealth).TakeCriticalDamage(_laserDamage, hit.point, hit.normal);
    //             else
    //                 (health as EnemyHealth).TakeDamage(_laserDamage, hit.point, hit.normal);
    //         }
    //     }
    // }

    public override void StartRecoil()
    {
        base.StartRecoil();
        if (_charged)
        {
            _recoilT += _laserAdditionalRecoil;
            LowerAmmoCount();
        }

        ResetChargeLevel();
    }

    public override void LowerAmmoCount()
    {
        base.LowerAmmoCount();
        ResetChargeLevel();
    }

    public override void ResetChargeLevel()
    {
        loopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        loopInstance.release();
        _charged = false;
        _currentCharge = 0f;
        _chargeUi.fillAmount = _currentCharge;
        transform.localPosition -= _shakenDirection;
        _shakenDirection = Vector3.zero;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //ResetChargeLevel();
    }
}
