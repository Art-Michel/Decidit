using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class AragonArm : Arm
{
    [Foldout("References")]
    [SerializeField]
    private GameObject _vfx;
    [Foldout("References")]
    [SerializeField]
    private Transform _vfxGroundProjection;
    [Foldout("Stats")]
    [SerializeField]
    private float _range;
    [Foldout("References")]
    [SerializeField]
    private Player _player;
    [Foldout("References")]
    [SerializeField]
    private Image _vignette;

    private Camera _camera;


    [Foldout("Stats")]
    [SerializeField]
    private LayerMask _detectionMask;
    [Foldout("Stats")]
    [SerializeField]
    private float _dashSpeed = 3f;
    [Foldout("Stats")]
    [SerializeField]
    AnimationCurve _dashFeedbacksCurve;
    [Foldout("Stats")]
    [SerializeField]
    AnimationCurve _dashMovementCurve;
    [Foldout("Stats")]
    [SerializeField]
    AnimationCurve _dashSpeedCurve;
    [Foldout("Stats")]
    [SerializeField]
    float _dashFovIncrease = 20f;
    [Foldout("Stats")]
    [SerializeField]
    float _maxVignetteAlpha = 0.3f;

    float _defaultFov;
    private Vector3 _dashStartPosition;
    private Vector3 _dashDestination;
    private float _dashT;
    private float currentDashSpeed;
    int _adjustedTimesNb;

    protected override void Awake()
    {
        base.Awake();
        _camera = Camera.main;
    }

    public override void StartIdle()
    {
        Refilled();
        _vfx.SetActive(false);
    }

    public override void StartPrevis()
    {
        _vfx.SetActive(true);
        PlaceHolderSoundManager.Instance.PlayDashPrevisSound();
    }

    public override void UpdatePrevis()
    {
        Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _range, _detectionMask);
        if (hit.transform != null)
            _vfx.transform.position = hit.point + hit.normal * 0.2f;
        else
            _vfx.transform.position = _cameraTransform.transform.position + _cameraTransform.forward * _range;

        _vfx.transform.up = hit.normal;
        Physics.Raycast(_vfx.transform.position, Vector3.down, out RaycastHit groundHit, 100f, _detectionMask);
        _vfxGroundProjection.position = groundHit.point + Vector3.up * 0.1f;
        _vfxGroundProjection.up = groundHit.normal;
    }

    public override void StopPrevis()
    {
        _vfx.SetActive(false);
    }

    public override void StartActive()
    {
        _crossHairOutline.enabled = false;
        PlaceHolderSoundManager.Instance.PlayDashSound();
        _player.AllowMovement(false);
        _player.KillMomentum();
        _player._charaCon.enabled = false;
        _dashStartPosition = _player.transform.position;
        _dashDestination = _vfx.transform.position;
        _adjustedTimesNb = 0;
        AdjustDestination();
        currentDashSpeed = _dashSpeed / Vector3.Distance(_dashStartPosition, _dashDestination);
        _dashT = 0;
        StartDashFeedbacks();
    }

    private void AdjustDestination()
    {
        bool yeah = false;
        _adjustedTimesNb++;
        if (Physics.Raycast(_dashDestination, Vector3.forward, .5f, _detectionMask))
        {
            _dashDestination += Vector3.forward * -.1f;
            yeah = true;
        }
        if (Physics.Raycast(_dashDestination, Vector3.back, .5f, _detectionMask))
        {
            _dashDestination += Vector3.back * -.1f;
            yeah = true;
        }
        if (Physics.Raycast(_dashDestination, Vector3.right, .5f, _detectionMask))
        {
            _dashDestination += Vector3.right * -.1f;
            yeah = true;
        }
        if (Physics.Raycast(_dashDestination, Vector3.left, .5f, _detectionMask))
        {
            _dashDestination += Vector3.left * -.1f;
            yeah = true;
        }
        if (Physics.Raycast(_dashDestination, Vector3.up, .5f, _detectionMask))
        {
            _dashDestination += Vector3.up * -.1f;
            yeah = true;
        }
        if (Physics.Raycast(_dashDestination, Vector3.down, .5f, _detectionMask))
        {
            _dashDestination += Vector3.down * -.1f;
            yeah = true;
        }
        if (yeah && _adjustedTimesNb < 8)
            AdjustDestination();
    }

    private void StartDashFeedbacks()
    {
        _defaultFov = _camera.fieldOfView;
    }

    public override void UpdateActive()
    {
        _dashT += Time.deltaTime * currentDashSpeed * _dashSpeedCurve.Evaluate(_dashT);
        _player.transform.position = Vector3.LerpUnclamped(_dashStartPosition, _dashDestination, _dashMovementCurve.Evaluate(_dashT));
        if (_dashT >= 1)
        {
            _fsm.ChangeState(ArmStateList.RECOVERY);
        }
        DashFeedbacks();
    }

    private void DashFeedbacks()
    {
        _camera.fieldOfView = Mathf.LerpUnclamped(_defaultFov, _defaultFov + _dashFovIncrease, _dashFeedbacksCurve.Evaluate(_dashT));
        _vignette.color = new Color(_vignette.color.r, _vignette.color.g, _vignette.color.b, Mathf.LerpUnclamped(0, _maxVignetteAlpha, _dashFeedbacksCurve.Evaluate(_dashT)));
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
