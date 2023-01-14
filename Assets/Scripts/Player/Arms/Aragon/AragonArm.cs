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

    [Foldout("Stats")]
    [SerializeField]
    private LayerMask _detectionMask;
    [Foldout("Stats")]
    [SerializeField]
    private float _dashSpeed;

    private Vector3 _dashStartPosition;
    private Vector3 _dashDestination;
    private float _dashT;

    public override void StartPrevis()
    {
        _vfx.SetActive(true);
    }

    public override void UpdatePrevis()
    {
        Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _range, _detectionMask);
        if (hit.transform != null)
            _vfx.transform.position = hit.point + hit.normal * 0.3f;
        else
            _vfx.transform.position = _camera.transform.position + _camera.forward * _range;
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
    }

    public override void UpdateActive()
    {
        _dashT += Time.deltaTime * _dashSpeed;
        _player.transform.position = Vector3.Lerp(_dashStartPosition, _dashDestination, _dashT);
        if (_dashT >= 1)
        {
            _fsm.ChangeState(ArmStateList.RECOVERY);
        }
    }

    public override void StopActive()
    {
        _player.AllowMovement(true);
        _player.KillMomentum();
        _player._charaCon.enabled = true;
    }
}
