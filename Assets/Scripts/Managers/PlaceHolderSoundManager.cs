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

    [SerializeField] AudioClip _revolverShot;
    [SerializeField] AudioClip _aragonShot;
    [SerializeField] AudioClip _museShot;
    [SerializeField] AudioClip _eylauShot;

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

    public void PlayEylauShot()
    {
        PlaySound(_eylauShot, 1f);
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