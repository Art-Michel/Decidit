using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

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
    private FMOD.Studio.EventInstance loopInstance;


    public override void StartIdle()
    {
        base.StartIdle();
        _previs.SetActive(false);
    }

    public override void StartPrevis()
    {
        base.StartPrevis();
        //TODOLucas un ptit son one shot au début de la preview pour pas que le son sorte de nulle part comme ça
        loopInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Controller/Chants/CimetièreEyleau/DuiringPreview");
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/CimetièreEyleau/Preview", 1f, gameObject);
        _previs.SetActive(true);
        loopInstance.start();
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
        base.StartActive();
        loopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        loopInstance.release();
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/CimetièreEyleau/Launch", 1f, gameObject);
        _crossHairFull.SetActive(false);
        StopGlowing();
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
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/CimetièreEyleau/End", 1f, gameObject);
        _area.transform.parent = this.gameObject.transform;
    }

    public override void StartRecovery()
    {
        // _area.SetActive(false);
        base.StartRecovery();
    }


}
