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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySound(string PathLink, float Volume, GameObject gameObject)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(PathLink, Volume, gameObject);
        //SFX.setVolume(Volume);
        //SFX.
    }

    public void ClearedSound()
    {
        _theme.SetParameter("Sound", 1f);
    }

    public void FightingSound()
    {
        _theme.SetParameter("Sound", 0f);
    }
}
