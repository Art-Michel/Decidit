using NaughtyAttributes;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager instance;
    [SerializeField] public float time;
    public bool isInCorridor;
    public bool endGame;

    [SerializeField]public float[] bestTime = new float[5];


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

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
        if (time > bestTime[ApplyDifficulty.instance.indexDifficulty])
        {
            bestTime[ApplyDifficulty.instance.indexDifficulty] = time;
            SaveLoadManager.SaveTimer(this);
        } 
    }
}