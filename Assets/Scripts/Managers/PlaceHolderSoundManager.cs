using System;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlaceHolderSoundManager : LocalManager<PlaceHolderSoundManager>
{
    [NonSerialized] public AudioSource AudioSource;

    //Note: Muse-Gun and Muse-Skill's explosions are played inside an audiosource in the projectile itself.
    [Foldout("Movement Sounds")][SerializeField] AudioClip _land;
    [Foldout("Health Sounds")][SerializeField] AudioClip _hurt;
    [Foldout("Health Sounds")][SerializeField] AudioClip _regen;

    [Foldout("Common Gun Sounds")][SerializeField] AudioClip _playReload;
    [Foldout("Common Gun Sounds")][SerializeField] AudioClip _playReloaded;
    [Foldout("Common Gun Sounds")][SerializeField] AudioClip _weaponEquip;
    [Foldout("Common Gun Sounds")][SerializeField] AudioClip _lastBulletClick;

    [Foldout("Base Gun Sounds")][SerializeField] AudioClip _revolverShot;
    [Foldout("Aragon Gun Sounds")][SerializeField] AudioClip _aragonShot;
    [Foldout("Muse Gun Sounds")][SerializeField] AudioClip _museShot;
    [Foldout("Eylau Gun Sounds")][SerializeField] AudioClip _eylauShot0;
    [Foldout("Eylau Gun Sounds")][SerializeField] AudioClip _eylauShot1;
    [Foldout("Eylau Gun Sounds")][SerializeField] AudioClip _eylauShot2;

    [Foldout("Eylau Gun Sounds")][SerializeField] AudioClip _eylauShot3;
    [Foldout("Eylau Gun Sounds")][SerializeField] AudioClip _eylauShot4;
    [Foldout("Eylau Gun Sounds")][SerializeField] AudioClip _eylauShot5;
    [Foldout("Eylau Gun Sounds")][SerializeField] AudioClip _eylauCharge0;
    [Foldout("Eylau Gun Sounds")][SerializeField] AudioClip _eylauCharge1;
    [Foldout("Eylau Gun Sounds")][SerializeField] AudioClip _eylauCharge2;
    [Foldout("Eylau Gun Sounds")][SerializeField] AudioClip _eylauCharge3;
    [Foldout("Eylau Gun Sounds")][SerializeField] AudioClip _eylauCharge4;
    [Foldout("Eylau Gun Sounds")][SerializeField] AudioClip _eylauCharge5;

    [Foldout("Common Skills Sounds")][SerializeField] AudioClip _armEquip;
    [Foldout("Common Skills Sounds")][SerializeField] AudioClip _armFilled;
    [Foldout("Aragon Skills Sounds")][SerializeField] AudioClip _dash;
    [Foldout("Aragon Skills Sounds")][SerializeField] AudioClip _dashPrevis;
    [Foldout("Muse Skills Sounds")][SerializeField] AudioClip _museRocketLaunch;

    [Foldout("Enemy Hit sounds")][SerializeField] AudioClip _hit;
    [Foldout("Enemy Hit sounds")][SerializeField] AudioClip _criticalHit;

    override protected void Awake()
    {
        base.Awake();
        AudioSource = GetComponent<AudioSource>();
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        AudioSource.PlayOneShot(clip, volume);
    }

    #region Movement sounds
    public void PlayLand()
    {
        PlaySound(_land, .2f);
    }
    #endregion

    #region Health sounds
    public void PlayHurt()
    {
        PlaySound(_hurt, 1f);
    }

    public void PlayRegen()
    {
        PlaySound(_regen, 1f);
    }
    #endregion

    #region Common gun sounds
    public void PlayLastBulletClick()
    {
        PlaySound(_lastBulletClick, .5f);
    }

    public void PlayReload()
    {
        PlaySound(_playReload, 1f);
    }

    public void PlayReloaded()
    {
        PlaySound(_playReloaded, 1f);
    }

    public void PlayWeaponEquip()
    {
        PlaySound(_weaponEquip, 1f);
    }
    #endregion

    #region Base Gun sounds
    public void PlayRevolverShot()
    {
        PlaySound(_revolverShot, 1f);
    }
    #endregion

    #region Aragon Gun sounds
    public void PlayAragonShot()
    {
        PlaySound(_aragonShot, 1f);
    }
    #endregion

    #region Muse Gun sounds
    public void PlayMuseShot()
    {
        PlaySound(_museShot, 1f);
    }
    #endregion

    #region Eylau Gun sounds
    public void PlayEylauCharge(int currentChargeStep)
    {
        switch (currentChargeStep)
        {
            case 0:
                PlaySound(_eylauCharge0, .2f);
                break;
            case 1:
                PlaySound(_eylauCharge1, .2f);
                break;
            case 2:
                PlaySound(_eylauCharge2, .2f);
                break;
            case 3:
                PlaySound(_eylauCharge3, .2f);
                break;
            case 4:
                PlaySound(_eylauCharge4, .2f);
                break;
            case 5:
                PlaySound(_eylauCharge5, .3f);
                break;
        }
    }

    public void PlayEylauShot(int i)
    {
        switch (i)
        {
            case 0:
                PlaySound(_eylauShot0, 1f);
                break;
            case 1:
                PlaySound(_eylauShot1, 1f);
                break;
            case 2:
                PlaySound(_eylauShot2, 1f);
                break;
            case 3:
                PlaySound(_eylauShot3, 1f);
                break;
            case 4:
                PlaySound(_eylauShot4, 1f);
                break;
            case 5:
                PlaySound(_eylauShot5, 1f);
                break;
        }
    }
    #endregion

    #region Common Skills sounds
    public void PlayArmEquip()
    {
        PlaySound(_armEquip, 1f);
    }
    public void PlayArmFilled()
    {
        PlaySound(_armFilled, 1f);
    }
    #endregion

    #region Aragon Skills sounds
    internal void PlayDashSound()
    {
        PlaySound(_dash, .4f);
    }
    internal void PlayDashPrevisSound()
    {
        PlaySound(_dashPrevis, 1f);
    }
    #endregion

    #region Muse Skills sounds
    internal void PlayMuseRocketLaunch()
    {
        PlaySound(_museRocketLaunch, 1f);
    }
    #endregion

    #region Enemies hit sounds
    public void PlayHitSound()
    {
        PlaySound(_hit, 2f);
    }

    public void PlayCriticalHitSound()
    {
        PlaySound(_criticalHit, 5f);
    }
    #endregion



}