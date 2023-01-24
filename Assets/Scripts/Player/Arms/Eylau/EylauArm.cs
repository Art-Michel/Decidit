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

    public override void StartIdle()
    {
        _crossHairOutline.enabled = true;
        _previs.SetActive(false);
    }

    public override void StartPrevis()
    {
        _previs.SetActive(true);
    }

    public override void UpdatePrevis()
    {
        Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _range, _detectionMask);
        if (hit.transform != null)
            _previs.transform.position = hit.point + hit.normal * 0.5f;
        else
            _previs.transform.position = _cameraTransform.transform.position + _cameraTransform.forward * _range;

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
}
