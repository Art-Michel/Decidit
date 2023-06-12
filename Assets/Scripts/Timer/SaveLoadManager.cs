using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{
    #region SaveTImer
    public static void SaveTimer(TimerManager timerManager)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/timer.dat");

        TimerData data = new TimerData(timerManager);

        bf.Serialize(file, data);
        file.Close();
    }
    public static TimerData LoadTimer()
    {
        if (File.Exists(Application.persistentDataPath + "/timer.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/timer.dat", FileMode.Open);

            TimerData data = bf.Deserialize(file) as TimerData;
            file.Close();
            return data;
        }
        else
        {
            Debug.LogWarning("no file found");
            return null;
        }
    }
    public static void DeleteStatsTimer()
    {
        Debug.Log("Delete Timer");
        File.Delete(Application.persistentDataPath + "/timer.dat");
    }

    public static float[] GetBestTime()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/timer.sav", FileMode.Open);

        TimerData data = bf.Deserialize(stream) as TimerData;

        return data.bestTime;
    }
    #endregion

    #region SaveSoundSetting
    public static void SaveSoundSet(PlayerSettings playerSettings)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SoundSettings.dat");

        SoundSettingData data = new SoundSettingData(playerSettings);

        bf.Serialize(file, data);
        file.Close();
    }
    public static SoundSettingData LoadSoundSet()
    {
        if (File.Exists(Application.persistentDataPath + "/SoundSettings.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SoundSettings.dat", FileMode.Open);

            SoundSettingData data = bf.Deserialize(file) as SoundSettingData;
            file.Close();
            return data;
        }
        else
        {
            Debug.LogWarning("no file found");
            return null;
        }
    }
    #endregion
}

[Serializable]
public class TimerData
{
    public float[] bestTime = new float[3];

    public TimerData(TimerManager timerManager)
    {
        bestTime = TimerManager.Instance.bestTime;
    }
}

[Serializable]
public class SoundSettingData
{
    public float masterVolumeNumber;
    public float SFXVolumeNumber;
    public float musicVolumeNumber;

    public SoundSettingData(PlayerSettings playerSettings)
    {
        masterVolumeNumber = playerSettings.masterVolumeNumber;
        SFXVolumeNumber = playerSettings.SFXVolumeNumber;
        musicVolumeNumber = playerSettings.musicVolumeNumber;
    }
}