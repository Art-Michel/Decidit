using NaughtyAttributes;
using UnityEngine;

public class TimerManager : ProjectManager<TimerManager>
{
    [SerializeField] public float time;
    public bool isInCorridor;
    public bool endGame;

    [SerializeField] public float[] bestTime = new float[5];

    void OnLevelWasLoaded()
    {
        time = 0;
        endGame = true;
        isInCorridor = true;
    }

    private void Start()
    {
        PrintTimer();
    }

    [Button]
    void PrintTimer()
    {
        bestTime = SaveLoadManager.LoadTimer().bestTime;
    }

    [Button]
    void DeletTimer()
    {
        SaveLoadManager.DeleteStatsTimer();
    }

    void Update()
    {
        if (!isInCorridor && !endGame)
            time += Time.deltaTime;
    }

    public void SaveTimer()
    {
        endGame = true;
        if (time < bestTime[ApplyDifficulty.Instance.indexDifficulty])
        {
            bestTime[ApplyDifficulty.Instance.indexDifficulty] = time;
            SaveLoadManager.SaveTimer(this);
        }
    }
}