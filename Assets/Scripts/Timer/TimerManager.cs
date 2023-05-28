using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerManager : LocalManager<TimerManager>
{
    [SerializeField] int index;
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

    public void ResetTimer(int index)
    {
        bestTime[index] = 0;
        SaveLoadManager.SaveTimer(this);
    }

    [Button]
    public void DeletTimer()
    {
        bestTime[index] = 0;
    }

    void Update()
    {
        if (!isInCorridor && !endGame)
            time += Time.deltaTime;
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