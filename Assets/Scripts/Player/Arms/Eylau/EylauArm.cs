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
    [Foldout("References")]
    [SerializeField]
    private GameObject _cancelPrompt;

    [Foldout("Stats")]
    [SerializeField]
    private float _range;
    [Foldout("Stats")]
    [SerializeField]
    private LayerMask _detectionMask;
    [Foldout("Stats")]
    [SerializeField]
    private LayerMask _enemyDetectionMask;
    private FMOD.Studio.EventInstance loopInstance;
    private Transform _currentEnemy;


    public override void StartIdle()
    {
        _previs.SetActive(false);
        base.StartIdle();
    }

    public override void StartPrevis()
    {
        base.StartPrevis();
        loopInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Controller/Chants/CimetièreEyleau/DuiringPreview");
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/CimetièreEyleau/Preview", 1f, gameObject);
        _previs.SetActive(true);
        loopInstance.start();
        _cancelPrompt.SetActive(true);
    }

    public override void UpdatePrevis()
    {
        // Vector3 pos;
        // Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _range, _detectionMask);
        // if (hit.transform != null)
        //     pos = hit.point + hit.normal;
        // else
        //     pos = _cameraTransform.transform.position + _cameraTransform.forward * _range;

        // Physics.Raycast(pos, Vector3.down, out RaycastHit groundHit, 100f, _detectionMask);

        // _previs.transform.position = groundHit.point + groundHit.normal * 0.1f;
        // _previs.transform.rotation = Quaternion.identity;

        Vector3 pos;
        Vector3 up;

        //Raycast forward looking for an enemy
        if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hurtbox, _range, _enemyDetectionMask))
        {
            pos = hurtbox.transform.position;
            up = Vector3.up;
            _currentEnemy = hurtbox.transform;
        }
        //Raycast forward looking for a wall
        else if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _range, _detectionMask))
        {
            pos = hit.point + hit.normal * 0.1f;
            up = hit.normal;
            _currentEnemy = null;
        }
        // if nothing was hit, Raycast downwards from camera's forward at max range
        else
        {
            Physics.Raycast(_cameraTransform.position + _cameraTransform.forward * _range, Vector3.down, out RaycastHit groundHit, 300f, _detectionMask);
            if (groundHit.transform != null)
            {
                pos = groundHit.point + Vector3.up * 0.1f;
                up = groundHit.normal;
            }
            else
            {
                pos = _cameraTransform.position + _cameraTransform.forward * _range;
                up = Vector3.up;
            }
            Debug.Log("non");
            _currentEnemy = null;
        }

        _previs.transform.position = pos;
        _previs.transform.up = up;
    }

    public override void StopPrevis()
    {
        loopInstance.release();
        loopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _previs.SetActive(false);
        _cancelPrompt.SetActive(false);
    }

    public override void StartActive()
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/CimetièreEyleau/Launch", 1f, gameObject);
        _crossHairFull.SetActive(false);
        StopGlowing();

        if (_currentEnemy != null)
        {
            EnemyHealth health = _currentEnemy.transform.GetComponent<Hurtbox>().HealthComponent as EnemyHealth;
            _area.transform.SetParent(health.transform);
            _area.transform.localPosition = Vector3.zero;
            health.AttachEylau(this.transform);
        }
        else
        {
            _area.transform.position = _previs.transform.position;
            _area.transform.parent = null;
        }

        _area.transform.rotation = Quaternion.identity;
        _area.SetActive(true);
        _area.GetComponent<EylauArea>().Reset();
        base.StartActive();
        Player.Instance.StartKickShake(_castShake, transform.position);
        this.Animator.CrossFade("cast", 0.1f, 0);
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
