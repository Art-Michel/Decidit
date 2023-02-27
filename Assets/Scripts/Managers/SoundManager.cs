using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;
using System;

public class SoundManager : LocalManager<SoundManager>
{
    [SerializeField] StudioListener AudioListener;
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

    public void PlaySound(string PathLink, float Volume, Vector3 position)
    {
        FMODUnity.RuntimeManager.PlayOneShot(PathLink, Volume, position);
        //SFX.setVolume(Volume);
        //SFX.
    }
}
