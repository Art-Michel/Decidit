using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using FMODUnity;

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
    private float _dashRange;
    [Foldout("References")]
    [SerializeField]
    private Player _player;
    [Foldout("References")]
    [SerializeField]
    private Image _vignette;
    [Foldout("References")]
    [SerializeField]
    private GameObject _cancelPrompt;
    [Foldout("References")]
    [SerializeField] Pooler _cloudPooler;

    private Camera _camera;


    [Foldout("Stats")]
    [SerializeField]
    private LayerMask _detectionMask;
    [Foldout("Stats")]
    [SerializeField]
    private LayerMask _triggerMask;
    [Foldout("Stats")]
    [SerializeField]
    private float _dashDuration = 0.6f;
    [Foldout("Stats")]
    [SerializeField]
    AnimationCurve _dashFeedbacksCurve;
    [Foldout("Stats")]
    [SerializeField]
    AnimationCurve _dashMovementCurve;
    // [Foldout("Stats")]
    // [SerializeField]
    // AnimationCurve _dashSpeedCurve;
    [Foldout("Stats")]
    [SerializeField]
    float _dashFovIncrease = 12f;
    [Foldout("Stats")]
    [SerializeField]
    float _maxVignetteAlpha = 0.3f;
    [Foldout("Stats")]
    [SerializeField]
    private float _momentumPostDash = 1f;
    [Foldout("Stats")]
    [SerializeField]
    private bool _uniformMomentumAfterDash = true;

    float _defaultFov;
    private Vector3 _dashStartPosition;
    private Vector3 _dashDestination;
    private float _dashT;
    private float currentDashSpeed;
    int _adjustedTimesNb;

    private FMOD.Studio.EventInstance loopInstance;

    private Vector3 _lastFramePosition;

    protected override void Awake()
    {
        base.Awake();
        _camera = Camera.main;
    }

    public override void StartIdle()
    {
        _vfx.SetActive(false);
        base.StartIdle();
    }

    public override void StartPrevis()
    {
        for (int i = 0; i < _castFx.Length; i++)
        {
            _precastFx[i].gameObject.SetActive(false);
            _precastFx[i].gameObject.SetActive(true);
        }
        base.StartPrevis();
        loopInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Controller/Chants/FugueAragon/DuringPreveiw");
        loopInstance.start();
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/FugueAragon/StartPreveiw", 1f, gameObject);
        _vfx.SetActive(true);
        _cancelPrompt.SetActive(true);
    }

    public override void UpdatePrevis()
    {
        Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _dashRange, _detectionMask);
        if (hit.transform != null)
            _vfx.transform.position = hit.point + hit.normal * 0.2f;
        else
            _vfx.transform.position = _cameraTransform.transform.position + _cameraTransform.forward * _dashRange;

        _vfx.transform.up = hit.normal;
        Physics.Raycast(_vfx.transform.position, Vector3.down, out RaycastHit groundHit, 100f, _detectionMask);
        _vfxGroundProjection.position = groundHit.point + Vector3.up * 0.1f;
        _vfxGroundProjection.up = groundHit.normal;
    }

    public override void StopPrevis()
    {
        loopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        loopInstance.release();
        _vfx.SetActive(false);
        _cancelPrompt.SetActive(false);
    }

    public override void StartActive()
    {
        //Prepare
        foreach (Image crosshair in _crossHairs)
            crosshair.fillAmount = 0.0f;

        for (int i = 0; i < _castFx.Length; i++)
            _castFx[i].Reinit();

        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/FugueAragon/DashStart", 1f, gameObject);

        _player.PlayerHealth.IsInvulnerable = true;
        _player.AllowMovement(false);
        _player.KillGravity();
        _player.KillMomentum();
        _player.CharaCon.detectCollisions = false;
        _dashStartPosition = _player.transform.position;
        _dashDestination = _vfx.transform.position;

        //Adjust Destination in order to stay away from walls!
        _adjustedTimesNb = 0;
        AdjustDestination();

        PlaceClouds(_dashStartPosition, _dashStartPosition + (_dashDestination - _dashStartPosition).normalized * (_dashRange + 8));

        //Actually move
        currentDashSpeed = _dashDuration / Vector3.Distance(_dashStartPosition, _dashDestination);
        _dashT = 0;

        //Feedbacks
        StartDashFeedbacks();
        Player.Instance.StartKickShake(_castShake, transform.position);
        this.Animator.CrossFade("cast", 0f, 0);
        base.StartActive();
    }

    private void PlaceClouds(Vector3 start, Vector3 end)
    {
        float dashLength = (end - start).magnitude;
        int numberOfClouds = Mathf.RoundToInt(dashLength);
        for (int i = 0; i < numberOfClouds; i++)
        {
            AragonCloud cloud = _cloudPooler.Get().GetComponent<AragonCloud>();
            Vector3 cloudPosition = Vector3.Lerp(start, end, Mathf.InverseLerp(0, numberOfClouds, i));
            float cloudDelay = Mathf.Lerp(0, _dashDuration + 0.2f, Mathf.InverseLerp(0, numberOfClouds, i));
            cloud.Setup(cloudPosition, _camera.transform.rotation, cloudDelay);
        }
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
        if (Physics.Raycast(_dashDestination, Vector3.right + Vector3.back, .5f, _detectionMask))
        {
            _dashDestination += Vector3.left * -.1f;
            yeah = true;
        }
        if (Physics.Raycast(_dashDestination, Vector3.right + Vector3.forward, .5f, _detectionMask))
        {
            _dashDestination += Vector3.left * -.1f;
            yeah = true;
        }
        if (Physics.Raycast(_dashDestination, Vector3.left, .5f, _detectionMask))
        {
            _dashDestination += Vector3.left * -.1f;
            yeah = true;
        }
        if (Physics.Raycast(_dashDestination, Vector3.left + Vector3.back, .5f, _detectionMask))
        {
            _dashDestination += Vector3.left * -.1f;
            yeah = true;
        }
        if (Physics.Raycast(_dashDestination, Vector3.left + Vector3.forward, .5f, _detectionMask))
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
        _lastFramePosition = transform.position;

        //Move
        _dashT += Time.deltaTime / _dashDuration / (Mathf.InverseLerp(0, _dashRange, (_dashDestination - _dashStartPosition).magnitude));

        _player.CharaCon.Move(Vector3.LerpUnclamped(_dashStartPosition, _dashDestination, _dashMovementCurve.Evaluate(_dashT)) - _player.transform.position);

        CheckForTriggers();

        //End dash when lerped all the way
        if (_dashT >= 1)
        {
            _fsm.ChangeState(ArmStateList.RECOVERY);
        }

        DashFeedbacks();
    }

    private void CheckForTriggers()
    {
        Vector3 spaceTraveledLastFrame = transform.position - _lastFramePosition;
        foreach (RaycastHit hit in Physics.RaycastAll(_lastFramePosition, spaceTraveledLastFrame.normalized, spaceTraveledLastFrame.magnitude, _triggerMask))
        {
            if (hit.transform.TryGetComponent<Door>(out Door door))
                door.Trigger();
            else if (hit.transform.parent.TryGetComponent<Door>(out Door door2))
                door2.Trigger();
            else
                Debug.LogWarning("crossed a trigger without a door");
        }
        foreach (Collider collider in Physics.OverlapSphere(transform.position, .4f, _triggerMask))
        {
            if (collider.transform.TryGetComponent<Door>(out Door door))
                door.Trigger();
            else if (collider.transform.parent.TryGetComponent<Door>(out Door door2))
                door2.Trigger();
            else
                Debug.LogWarning("crossed a trigger without a door");
        }
    }

    private void DashFeedbacks()
    {
        _camera.fieldOfView = Mathf.LerpUnclamped(_defaultFov, _defaultFov + _dashFovIncrease, _dashFeedbacksCurve.Evaluate(_dashT));
        _vignette.color = new Color(_vignette.color.r, _vignette.color.g, _vignette.color.b, Mathf.LerpUnclamped(0, _maxVignetteAlpha, _dashFeedbacksCurve.Evaluate(_dashT)));
    }

    public override void StopActive()
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/FugueAragon/DashEnd", 1f, gameObject);
        _player.AllowMovement(true);
        _player.KillGravity();
        _player.KillMomentum();
        _player.PlayerHealth.IsInvulnerable = false;
        _player.CharaCon.detectCollisions = true;
        StopDashFeedbacks();

        if (_uniformMomentumAfterDash)
            _player.AddMomentum((_dashDestination - _dashStartPosition).normalized * _momentumPostDash);
        else
            _player.AddMomentum((_dashDestination - _dashStartPosition) * _momentumPostDash);
    }

    private void StopDashFeedbacks()
    {
        _camera.fieldOfView = _defaultFov;
    }
}
