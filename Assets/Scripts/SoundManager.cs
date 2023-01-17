using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("List de son WallMob")] public List<SoundVolume> soundAndVolumeWallMob = new List<SoundVolume>();
    public static List<SoundVolume> soundAndVolumeWallMobStatic = new List<SoundVolume>();

    [Header("List de son Trash")] public List<SoundVolume> soundAndVolumeListTrashMob = new List<SoundVolume>();
    public static List<SoundVolume> soundAndVolumeListTrashMobStatic = new List<SoundVolume>();

    [Header("List de son RushMob")] public List<SoundVolume> soundAndVolumeRushMob = new List<SoundVolume>();
    public static List<SoundVolume> soundAndVolumeRushMobStatic = new List<SoundVolume>();

    [Header("List de son FlyMob")] public List<SoundVolume> soundAndVolumeFlyMob = new List<SoundVolume>();
    public static List<SoundVolume> soundAndVolumeFlyMobStatic = new List<SoundVolume>();

    private void Awake()
    {
        soundAndVolumeWallMobStatic = soundAndVolumeWallMob;

        soundAndVolumeListTrashMobStatic = soundAndVolumeListTrashMob;

        soundAndVolumeRushMobStatic = soundAndVolumeRushMob;

        soundAndVolumeFlyMobStatic = soundAndVolumeFlyMob;
    }

    //Player Sound
    public static void PlaySoundMobOneShot(AudioSource sourceAudio, SoundVolume soundVolume)
    {
        sourceAudio.volume = soundVolume._volume;
        sourceAudio.PlayOneShot(soundVolume._clip);
    }

    public static void PlaySoundMobByClip(AudioSource sourceAudio, SoundVolume soundVolume, bool active)
    {
        if (active)
        {
            sourceAudio.volume = soundVolume._volume;
            sourceAudio.clip = soundVolume._clip;
            sourceAudio.loop = active;
            sourceAudio.Play();
        }
        else
        {
            sourceAudio.Stop();
        }
    }

    //2D sound continue
    public static void PlaySound2DContinue(AudioSource sourceAudio, SoundVolume soundVolume, bool active)
    {
        if (active)
        {
            sourceAudio.volume = soundVolume._volume;
            sourceAudio.clip = soundVolume._clip;
            sourceAudio.loop = active;
            sourceAudio.Play();
        }
        else
        {
            sourceAudio.Stop();
        }
    }
}

[System.Serializable]
public class SoundVolume
{
    public string _nameSound;
    public AudioClip _clip;
    [Range(0.0f, 1f)] public float _volume;
    [Range(0.0f, 1f)] public float _pitch;
}