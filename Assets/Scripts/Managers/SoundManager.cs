using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;
using System;

public class SoundManager : LocalManager<SoundManager>
{
    [SerializeField] StudioListener AudioListener;
    [SerializeField] StudioEventEmitter _theme;
    FMOD.Studio.Bus SFX;

    protected override void Awake()
    {
        base.Awake();
    }

    public void PlaySound(string PathLink, float Volume, GameObject gameObject)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(PathLink, Volume, gameObject);
        //SFX.setVolume(Volume);
        //SFX.
    }

    //Change la musique en mode boite de nuit 
    public void ClearedSound()
    {
        _theme.SetParameter("Sound", 1f);
    }

    public void FightingSound()
    {
        _theme.SetParameter("Sound", 0f);
    }

    //Change tous les sons en mode boite de nuit (pour le hit)
    public void SoundAfterHit()
    {
        _theme.SetParameter("SoundHit", 1f);
    }

    public void SoundEndHit()
    {
        _theme.SetParameter("SoundHit", 0f);
    }
}
