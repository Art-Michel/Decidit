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
    [SerializeField] private Pooler _projectile1Pooler;
    [Foldout("References")]
    [SerializeField] private Image _chargeUi;

    [Foldout("Recoils")]
    [SerializeField] private float _recoilCharge0 = .9f;
    [Foldout("Recoils")]
    [SerializeField] private float _recoilCharge1 = 1f;
    [Foldout("Recoils")]
    [SerializeField] private float _recoilCharge2 = 1.1f;
    [Foldout("Recoils")]
    [SerializeField] private float _recoilCharge3 = 1.2f;
    [Foldout("Recoils")]
    [SerializeField] private float _recoilCharge4 = 1.3f;
    [Foldout("Recoils")]
    [SerializeField] private float _recoilCharge5 = 1.6f;

    [Foldout("Stats")]
    [SerializeField] private float _chargeSpeed = 2f;
    private float _currentCharge;
    private int _currentChargeStep;
    private const int _maxCharge = 5;

    public override void UpdateChargeLevel()
    {
        _currentCharge = Mathf.Clamp(_currentCharge + Time.deltaTime * _chargeSpeed, 0, _maxCharge);
        _chargeUi.fillAmount = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, _maxCharge, _currentCharge));

        Debug.Log(_currentCharge);
        if (_currentCharge >= _currentChargeStep + 1)
        {
            _currentChargeStep += 1;
            PlaceHolderSoundManager.Instance.PlayEylauCharge(_currentChargeStep);
        }
    }

    public override void Shoot()
    {
        PooledObject shot = _projectile1Pooler.Get();
        shot.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized);

        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
        PlaceHolderSoundManager.Instance.PlayEylauShot(_currentChargeStep);
        ResetChargeLevel();
        _muzzleFlash.Play();
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
