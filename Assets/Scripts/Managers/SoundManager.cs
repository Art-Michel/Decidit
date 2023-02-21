using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundManager : ProjectManager<SoundManager>
{
    private FMOD.Studio.EventInstance MenuSoundInstance;
    private FMOD.Studio.EventInstance ThemeSoundInstance;

    private void Awake()
    {
        MenuSoundInstance = RuntimeManager.CreateInstance("event:/Theme/MenuTheme");
        ThemeSoundInstance = RuntimeManager.CreateInstance("event:/Theme/GameTheme");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySound(string PathLink)
    {
        FMODUnity.RuntimeManager.PlayOneShot(PathLink);
    }

    public void PlayMenu()
    {
        ThemeSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        MenuSoundInstance.start();
    }
    public void PlayTheme(string PathLink)
    {
        MenuSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ThemeSoundInstance.start();
    }
}
