using System.Collections;
using System.Collections.Generic;
using CameraShake;
using NaughtyAttributes;
using UnityEngine;

public class MuseArm : Arm
{
    [Foldout("References")]
    [SerializeField] Pooler _pooler;

    [Foldout("Stats")]
    [SerializeField] protected float _launchShakeIntensity;
    [Foldout("Stats")]
    [SerializeField] protected float _launchShakeDuration;

    [Foldout("References")]
    [SerializeField]
    private GameObject _cancelPrompt;
    [Foldout("References")]
    [SerializeField]
    Transform _canonPosition;
    [Foldout("References")]
    [SerializeField]
    LayerMask _mask;
    private FMOD.Studio.EventInstance loopInstance;

    private Projectile _missile;
    Vector3 _currentlyAimedAt;

    protected override void PressSong()
    {
        if (_missile == null || _missile.enabled == false)
        {
            base.PressSong();
            // this.ReleaseSong();
        }
        else
            _missile.Explode(_missile.transform.forward);
    }

    protected override void ReleaseSong()
    {
        base.ReleaseSong();
    }

    public override void StartIdle()
    {
        base.StartIdle();
    }

    public override void StartPrevis()
    {
        if (_precastFx.Length > 0)
            for (int i = 0; i < _precastFx.Length; i++)
            {
                _precastFx[i].Reinit();
                _precastFx[i].Play();
            }
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/FugueAragon/StartPreveiw", 1f, gameObject);
        loopInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Controller/Chants/FugueAragon/DuringPreveiw");
        base.StartPrevis();
        _cancelPrompt.SetActive(true);
        loopInstance.start();
    }

    public override void StopPrevis()
    {
        loopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        loopInstance.release();
        _cancelPrompt.SetActive(false);
    }

    public override void CheckLookedAt()
    {
        if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, 10000f, _mask))
            _currentlyAimedAt = hit.point;
        else
            _currentlyAimedAt = _cameraTransform.forward * 10000f;
    }

    public override void StartActive()
    {
        CheckLookedAt();

        _crossHairFull.SetActive(false);
        StopGlowing();

        for (int i = 0; i < _castFx.Length; i++)
        {
            _castFx[i].Reinit();
            _castFx[i].Play();
        }

        _missile = _pooler.Get().GetComponent<Projectile>();
        _missile.transform.rotation = transform.rotation;
        _missile.Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _cameraTransform.forward);

        Player.Instance.StartKickShake(_castShake, transform.position);
        ////PlaceHolderSoundManager.Instance.PlayMuseRocketLaunch();
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/MuseMalade/Launch", 1f, gameObject);
        //_muzzleFlash.PlayAll();
        base.StartActive();
        this.Animator.CrossFade("cast", 0, 0);
        _fsm.ChangeState(ArmStateList.RECOVERY);
    }
}
