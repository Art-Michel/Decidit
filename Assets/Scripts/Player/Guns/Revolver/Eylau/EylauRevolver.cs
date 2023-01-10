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
    [SerializeField] private Pooler _projectile0Pooler;
    [Foldout("References")]
    [SerializeField] private Pooler _projectile1Pooler;
    [Foldout("References")]
    [SerializeField] private Pooler _projectile2Pooler;
    [Foldout("References")]
    [SerializeField] private Pooler _projectile3Pooler;
    [Foldout("References")]
    [SerializeField] private Pooler _projectile4Pooler;
    [Foldout("References")]
    [SerializeField] private Pooler _projectile5Pooler;
    [Foldout("References")]
    [SerializeField] private Image _chargeUi;

    [Foldout("Recoils values")]
    [SerializeField] private float _additionalRecoilCharge0 = .1f;
    [Foldout("Recoils values")]
    [SerializeField] private float _additionalRecoilCharge1 = .2f;
    [Foldout("Recoils values")]
    [SerializeField] private float _additionalRecoilCharge2 = .3f;
    [Foldout("Recoils values")]
    [SerializeField] private float _additionalRecoilCharge3 = .4f;
    [Foldout("Recoils values")]
    [SerializeField] private float _additionalRecoilCharge4 = .5f;
    [Foldout("Recoils values")]
    [SerializeField] private float _additionalRecoilCharge5 = .6f;

    [Foldout("Screenshake values")]
    [SerializeField] private float _shakeIntensity0 = .9f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _shakeIntensity1 = 1f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _shakeIntensity2 = 1.1f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _shakeIntensity3 = 1.2f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _shakeIntensity4 = 1.3f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _shakeIntensity5 = 1.6f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _shakeDuration0 = .9f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _shakeDuration1 = 1f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _shakeDuration2 = 1.1f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _shakeDuration3 = 1.2f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _shakeDuration4 = 1.3f;
    [Foldout("Screenshake values")]
    [SerializeField] private float _shakeDuration5 = 1.6f;

    [Foldout("Projectiles")]
    [SerializeField] private GameObject _bulletPrefab0;
    [Foldout("Projectiles")]
    [SerializeField] private GameObject _bulletPrefab1;
    [Foldout("Projectiles")]
    [SerializeField] private GameObject _bulletPrefab2;
    [Foldout("Projectiles")]
    [SerializeField] private GameObject _bulletPrefab3;
    [Foldout("Projectiles")]
    [SerializeField] private GameObject _bulletPrefab4;
    [Foldout("Projectiles")]
    [SerializeField] private GameObject _bulletPrefab5;
    [Foldout("Projectiles")]
    [SerializeField] private Pooler _vfxPooler;

    [Foldout("Stats")]
    [SerializeField] private float _chargeSpeed = 2f;
    [Foldout("Stats")]
    [SerializeField] private int _hitscanMaxRange;
    [Foldout("Stats")]
    [SerializeField] private int _hitscanDamage;
    [Foldout("Stats")]
    [SerializeField] private bool _laserShouldPierce;
    private float _currentCharge;
    private int _currentChargeStep;
    private const int _maxCharge = 5;

    public override void UpdateChargeLevel()
    {
        _currentCharge = Mathf.Clamp(_currentCharge + Time.deltaTime * _chargeSpeed, 0, _maxCharge);
        _chargeUi.fillAmount = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, _maxCharge, _currentCharge));

        if (_currentCharge >= _currentChargeStep + 1)
        {
            _currentChargeStep += 1;
            PlaceHolderSoundManager.Instance.PlayEylauCharge(_currentChargeStep);
        }
    }

    public override void Shoot()
    {
        PooledObject shot = null;
        switch (_currentChargeStep)
        {
            case 0:
                shot = _projectile0Pooler.Get();
                Player.Instance.StartShake(_shakeIntensity0, _shakeDuration0);
                break;
            case 1:
                shot = _projectile1Pooler.Get();
                Player.Instance.StartShake(_shakeIntensity1, _shakeDuration1);
                break;
            case 2:
                shot = _projectile2Pooler.Get();
                Player.Instance.StartShake(_shakeIntensity2, _shakeDuration2);
                break;
            case 3:
                shot = _projectile3Pooler.Get();
                Player.Instance.StartShake(_shakeIntensity3, _shakeDuration3);
                break;
            case 4:
                shot = _projectile4Pooler.Get();
                Player.Instance.StartShake(_shakeIntensity4, _shakeDuration4);
                break;
            case 5:
                if (_laserShouldPierce)
                    PiercingLaser();
                else
                    Laser();
                Player.Instance.StartShake(_shakeIntensity5, _shakeDuration5);
                break;
        }

        if (shot != null)
            shot.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized);

        PlaceHolderSoundManager.Instance.PlayEylauShot(_currentChargeStep);
        _muzzleFlash.Play();
    }

    private void Laser()
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
    }
    private void PiercingLaser()
    {

        var vfx = _vfxPooler.Get().GetComponent<TwoPosTrail>();
        RaycastHit[] hits = Physics.RaycastAll(_camera.position, _camera.forward, _hitscanMaxRange, _mask);
        foreach (RaycastHit hit in hits)
        {
            hit.transform.parent.TryGetComponent<Health>(out Health health);
            if (health)
            {

                if (hit.transform.CompareTag("WeakHurtbox"))
                    (health as EnemyHealth).TakeCriticalDamage(_hitscanDamage, hit.point, hit.normal);
                else
                    (health as EnemyHealth).TakeDamage(_hitscanDamage, hit.point, hit.normal);
            }
        }
        vfx.SetPos(_canonPosition.position, _canonPosition.position + _camera.forward * _hitscanMaxRange);
    }

    public override void StartRecoil()
    {
        base.StartRecoil();
        switch (_currentChargeStep)
        {
            case 0:
                _recoilT += _additionalRecoilCharge0;
                break;
            case 1:
                _recoilT += _additionalRecoilCharge1;
                break;
            case 2:
                _recoilT += _additionalRecoilCharge2;
                break;
            case 3:
                _recoilT += _additionalRecoilCharge3;
                break;
            case 4:
                _recoilT += _additionalRecoilCharge4;
                break;
            case 5:
                _recoilT += _additionalRecoilCharge5;
                break;
        }
    }

    public override void LowerAmmoCount()
    {
        base.LowerAmmoCount();
        ResetChargeLevel();
    }

    public override void ResetChargeLevel()
    {
        _currentCharge = 0f;
        _currentChargeStep = 0;
        _chargeUi.fillAmount = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, _maxCharge, _currentCharge));
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetChargeLevel();
    }
}
