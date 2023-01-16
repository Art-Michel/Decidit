using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class AragonArm : Arm
{
    [Foldout("References")]
    [SerializeField]
    private GameObject _vfx;
    [Foldout("Stats")]
    [SerializeField]
    private float _range;
    [Foldout("References")]
    [SerializeField]
    private Player _player;

    private Camera _camera;


    [Foldout("Stats")]
    [SerializeField]
    private LayerMask _detectionMask;
    [Foldout("Stats")]
    [SerializeField]
    private float _dashSpeed;
    [Foldout("Stats")]
    [SerializeField]
    AnimationCurve _dashFeedbacksCurve;
    [Foldout("Stats")]
    [SerializeField]
    float _dashFovIncrease;

    float _defaultFov;
    private Vector3 _dashStartPosition;
    private Vector3 _dashDestination;
    private float _dashT;

    protected override void Awake()
    {
        base.Awake();
        _camera = Camera.main;
    }

    public override void StartIdle()
    {
        _vfx.SetActive(false);
    }

    public override void StartPrevis()
    {
        _vfx.SetActive(true);
    }

    public override void UpdatePrevis()
    {
        Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _range, _detectionMask);
        if (hit.transform != null)
            _vfx.transform.position = hit.point + hit.normal * 0.3f;
        else
            _vfx.transform.position = _cameraTransform.transform.position + _cameraTransform.forward * _range;
        _vfx.transform.rotation = Quaternion.identity;
    }

    public override void StopPrevis()
    {
        _vfx.SetActive(false);
    }

    public override void StartActive()
    {
        _player.AllowMovement(false);
        _player.KillMomentum();
        _player._charaCon.enabled = false;
        _dashStartPosition = _player.transform.position;
        _dashDestination = _vfx.transform.position + Vector3.up;
        _dashT = 0;
        StartDashFeedbacks();
    }

    private void StartDashFeedbacks()
    {
        _defaultFov = _camera.fieldOfView;
    }

    public override void UpdateActive()
    {
        _dashT += Time.deltaTime * _dashSpeed;
        _player.transform.position = Vector3.Lerp(_dashStartPosition, _dashDestination, _dashT);
        if (_dashT >= 1)
        {
            _fsm.ChangeState(ArmStateList.RECOVERY);
        }
        DashFeedbacks();
    }

    private void DashFeedbacks()
    {
        _camera.fieldOfView = Mathf.Lerp(_defaultFov, _defaultFov + _dashFovIncrease, _dashFeedbacksCurve.Evaluate(_dashT));
    }

    public override void StopActive()
    {
        _player.AllowMovement(true);
        _player.KillMomentum();
        _player._charaCon.enabled = true;
        StopDashFeedbacks();
    }

    private void StopDashFeedbacks()
    {
        _camera.fieldOfView = _defaultFov;
    }
}
