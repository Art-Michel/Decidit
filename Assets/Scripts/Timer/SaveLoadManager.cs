using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{
    // Save Timer
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
}

[Serializable]
public class TimerData
{
    public float[] bestTime = new float[5];

    public TimerData(TimerManager timerManager)
    {
        bestTime = TimerManager.instance.bestTime;
    }
}