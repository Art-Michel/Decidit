using System;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using NaughtyAttributes;

public class EylauRevolver : Revolver
{
    [Foldout("References")]
    [SerializeField] private Pooler _projectilePooler;
    [Foldout("References")]
    [SerializeField] private Pooler _laserVfxPooler;
    [Foldout("References")]
    [SerializeField] private Image _chargeUi;

    [Foldout("Screenshake values")]
    [SerializeField] private float _bulletShakeIntensity = .9f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _bulletShakeDuration = .9f;

    [Foldout("Screenshake values")]
    [SerializeField] private float _laserShakeIntensity = 2f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _laserShakeDuration = 2f;

    [Foldout("Stats")]
    [SerializeField] private float _chargeSpeed = 1f;
    [Foldout("Stats")]
    [SerializeField] private float _chargedWeaponShakeIntensity;
    [Foldout("Stats")]
    [SerializeField] private float _laserAdditionalRecoil = .3f;
    [Foldout("Stats")]
    [SerializeField] private int _laserMaxRange;
    [Foldout("Stats")]
    [SerializeField] private int _laserDamage;
    [Foldout("Stats")]
    [SerializeField] private bool _laserShouldPierce;

    private float _currentCharge;
    private bool _charged;
    private Vector3 _shakenDirection = Vector3.zero;

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
                _charged = true;
        }
    }

    public override void Shoot()
    {
        PooledObject shot = null;
        if (!_charged)
        {
            shot = _projectilePooler.Get();
            shot.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized);

            Player.Instance.StartShake(_bulletShakeIntensity, _bulletShakeDuration);
            _muzzleFlash.PlayAll();
        }

        else
        {
            if (_laserShouldPierce)
                PiercingLaser();
            else
                Laser();

            Player.Instance.StartShake(_laserShakeIntensity, _laserShakeDuration);
            _muzzleFlash.PlayAll();
        }

    }

    private void Laser()
    {

        var vfx = _laserVfxPooler.Get().GetComponent<TwoPosTrail>();
        if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _laserMaxRange, _mask) && hit.transform.parent.TryGetComponent<Health>(out Health health))
        {
            vfx.SetPos(_canonPosition.position, hit.point);
            if (hit.transform.CompareTag("WeakHurtbox"))
                (health as EnemyHealth).TakeCriticalDamage(_laserDamage, hit.point, hit.normal);
            else
                (health as EnemyHealth).TakeDamage(_laserDamage, hit.point, hit.normal);
        }
        else
            vfx.SetPos(_canonPosition.position, _canonPosition.position + _camera.forward * _laserMaxRange);
    }

    private void PiercingLaser()
    {

        var vfx = _laserVfxPooler.Get().GetComponent<TwoPosTrail>();
        RaycastHit[] hits = Physics.RaycastAll(_camera.position, _camera.forward, _laserMaxRange, _mask);
        foreach (RaycastHit hit in hits)
        {
            hit.transform.parent.TryGetComponent<Health>(out Health health);
            if (health)
            {

                if (hit.transform.CompareTag("WeakHurtbox"))
                    (health as EnemyHealth).TakeCriticalDamage(_laserDamage, hit.point, hit.normal);
                else
                    (health as EnemyHealth).TakeDamage(_laserDamage, hit.point, hit.normal);
            }
        }
        vfx.SetPos(_canonPosition.position, _canonPosition.position + _camera.forward * _laserMaxRange);
    }

    public override void StartRecoil()
    {
        base.StartRecoil();
        if (_charged)
            _recoilT += _laserAdditionalRecoil;

        ResetChargeLevel();
    }

    public override void LowerAmmoCount()
    {
        base.LowerAmmoCount();
        ResetChargeLevel();
    }

    public override void ResetChargeLevel()
    {
        _charged = false;
        _currentCharge = 0f;
        _chargeUi.fillAmount = _currentCharge;
        transform.localPosition -= _shakenDirection;
        _shakenDirection = Vector3.zero;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetChargeLevel();
    }
}
