using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerManager : LocalManager<TimerManager>
{
    [SerializeField] public float time;
    public bool isInCorridor;
    public bool endGame;

    [SerializeField] public float[] bestTime = new float[3];

    void OnLevelWasLoaded()
    {
        //SaveTimer();
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == "GamePath")
        {
            endGame = false;
            time = 0;
            isInCorridor = true;
        }
        else
        {
            endGame = true;
            time = 0;
            isInCorridor = true;
        }
    }

    private void Start()
    {
        PrintTimer();
        Scene scene = SceneManager.GetActiveScene();
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

        Debug.Log(bestTime[0]);
        Debug.Log(bestTime[1]);
        Debug.Log(bestTime[2]);
    }

    public void SaveTimer()
    {
        Debug.Log("Save Timer");

        endGame = true;
        if (time < bestTime[ApplyDifficulty.indexDifficulty] || bestTime[ApplyDifficulty.indexDifficulty] == 0)
        {
            bestTime[ApplyDifficulty.indexDifficulty] = time;
            SaveLoadManager.SaveTimer(this);
            Debug.Log("Save Timer 2 ");
        }
    }
}