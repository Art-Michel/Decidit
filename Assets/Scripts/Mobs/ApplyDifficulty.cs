using UnityEngine;

public class ApplyDifficulty : MonoBehaviour
{
    static public ApplyDifficulty instance;
    public int indexDifficulty; // 0 ez / 1 Med / 2 Hard

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("Difficulty Value : " + indexDifficulty);
    }

    private void Update()
    {
        Debug.Log("Difficulty Value : " + indexDifficulty);
    }

    public void SelectDifficulty(int i)
    {
        indexDifficulty = i;
        Debug.Log("Difficulty Value : " + indexDifficulty);
    }
}
