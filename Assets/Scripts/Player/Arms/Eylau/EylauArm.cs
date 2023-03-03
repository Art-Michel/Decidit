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
        _previs.SetActive(false);
        Refilled();
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
        _area.GetComponent<EylauArea>().Reset();
        _previs.SetActive(false);
        _fsm.ChangeState(ArmStateList.RECOVERY);
    }

    public override void StopActive()
    {
        _area.transform.parent = this.gameObject.transform;
    }

    public override void StartRecovery()
    {
        // _area.SetActive(false);
        base.StartRecovery();
    }


}
