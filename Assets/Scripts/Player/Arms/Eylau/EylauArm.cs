using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class EylauArm : Arm
{
    [Foldout("References")]
    [SerializeField]
    private GameObject _previs;
    [Foldout("References")]
    [SerializeField]
    private GameObject _area;

    [Foldout("Stats")]
    [SerializeField]
    private float _range;
    [Foldout("Stats")]
    [SerializeField]
    private LayerMask _detectionMask;

    float _shakeT;
    float _shakeInitialT;
    float _shakeIntensity;
    private Vector3 _initialHeadPos;

    public override void StartIdle()
    {
        Refilled();
        _previs.SetActive(false);
    }

    public override void StartPrevis()
    {
        _previs.SetActive(true);
    }

    public override void UpdatePrevis()
    {
        Vector3 pos;
        Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _range, _detectionMask);
        if (hit.transform != null)
            pos = hit.point + hit.normal;
        else
            pos = _cameraTransform.transform.position + _cameraTransform.forward * _range;

        Physics.Raycast(pos, Vector3.down, out RaycastHit groundHit, 100f, _detectionMask);

        _previs.transform.position = groundHit.point + groundHit.normal * 0.1f;
        _previs.transform.rotation = Quaternion.identity;
    }

    public override void StartActive()
    {
        _crossHairOutline.enabled = false;
        _area.transform.position = _previs.transform.position;
        _area.transform.parent = null;
        _area.transform.rotation = Quaternion.identity;
        _area.SetActive(true);
        _previs.SetActive(false);
        _fsm.ChangeState(ArmStateList.RECOVERY);
    }

    public override void StopActive()
    {
        _area.transform.parent = null;
    }

    public override void StartRecovery()
    {
        // _area.SetActive(false);
        base.StartRecovery();
    }

    public void StartShake(float intensity, float duration)
    {
        if (duration > _shakeT)
        {
            _shakeInitialT = duration;
            _shakeT = duration;
        }
        if (intensity > _shakeIntensity)
            _shakeIntensity = intensity;
    }

    private void Shake()
    {
        if (_shakeT > 0)
        {
            float shakeIntensity = _shakeIntensity * Mathf.InverseLerp(0, _shakeInitialT, _shakeT);
            transform.localPosition = _initialHeadPos + new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), 0).normalized * shakeIntensity;
            _shakeT -= Time.deltaTime;
            if (_shakeT < 0)
                StopShake();
        }
    }

    public void StopShake()
    {
        _shakeIntensity = 0;
        _shakeT = 0;
        transform.localPosition = _initialHeadPos;
    }
}
