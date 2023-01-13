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
    [Foldout("References")]
    [SerializeField]
    private float _range;


    [Foldout("Stats")]
    [SerializeField]
    private LayerMask _detectionMask;

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
}
