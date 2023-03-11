using UnityEngine;

public class ApplyDifficulty : MonoBehaviour
{
    static public ApplyDifficulty instance;
    public int indexDifficulty; // 0 ez / 1 Med / 2 Hard

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SelectDifficulty(int i)
    {
        indexDifficulty = i;
    }
}