using System;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlaceHolderSoundManager : LocalManager<PlaceHolderSoundManager>
{
    [NonSerialized] public AudioSource AudioSource;

    [SerializeField] AudioClip _land;

    [SerializeField] AudioClip _lastBulletClick;
    [SerializeField] AudioClip _revolverShot;
    [SerializeField] AudioClip _aragonShot;
    [SerializeField] AudioClip _museShot;
    [SerializeField] AudioClip _museExplosion;

    [SerializeField] AudioClip _eylauShot0;
    [SerializeField] AudioClip _eylauShot1;
    [SerializeField] AudioClip _eylauShot2;
    [SerializeField] AudioClip _eylauShot3;
    [SerializeField] AudioClip _eylauShot4;
    [SerializeField] AudioClip _eylauShot5;
    [SerializeField] AudioClip _eylauCharge0;
    [SerializeField] AudioClip _eylauCharge1;
    [SerializeField] AudioClip _eylauCharge2;
    [SerializeField] AudioClip _eylauCharge3;
    [SerializeField] AudioClip _eylauCharge4;
    [SerializeField] AudioClip _eylauCharge5;

    [SerializeField] AudioClip _playReload;
    [SerializeField] AudioClip _playReloaded;
    [SerializeField] AudioClip _weaponEquip;
    [SerializeField] AudioClip _armEquip;

    [SerializeField] AudioClip _hurt;
    [SerializeField] AudioClip _regen;

    [SerializeField] AudioClip _hit;
    [SerializeField] AudioClip _criticalHit;


    private void PlaySound(AudioClip clip, float volume)
    {
        AudioSource.PlayOneShot(clip, volume);
    }

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

    public void PlayLand()
    {
        PlaySound(_land, .2f);
    }

    public void PlayHurt()
    {
        PlaySound(_hurt, 1f);
    }

    public void PlayRegen()
    {
        PlaySound(_regen, 1f);
    }

    public void PlayRevolverShot()
    {
        PlaySound(_revolverShot, 1f);
    }

    public void PlayHitSound()
    {
        PlaySound(_hit, 2f);
    }

    public void PlayCriticalHitSound()
    {
        PlaySound(_criticalHit, 5f);
    }

    public void PlayAragonShot()
    {
        PlaySound(_aragonShot, 1f);
    }

    public void PlayMuseShot()
    {
        PlaySound(_museShot, 1f);
    }

    public void PlayLastBulletClick()
    {
        PlaySound(_lastBulletClick, .5f);
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
                PlaySound(_eylauShot5, 1f);
                break;
            case 5:
                PlaySound(_eylauShot5, 1f);
                break;
        }
    }

    public void PlayWeaponEquip()
    {
        PlaySound(_weaponEquip, 1f);
    }

    public void PlayArmEquip()
    {
        PlaySound(_armEquip, 1f);
    }

    public void PlayReload()
    {
        PlaySound(_playReload, 1f);
    }

    public void PlayReloaded()
    {
        PlaySound(_playReloaded, 1f);
    }

    override protected void Awake()
    {
        base.Awake();
        AudioSource = GetComponent<AudioSource>();
    }

}