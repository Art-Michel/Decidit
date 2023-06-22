using UnityEngine;

public class ApplyDifficulty : LocalManager<ApplyDifficulty>
{
    [Range((int)0, (int)2)]
    public static int indexDifficulty; // 0 ez / 1 Med / 2 Hard

    protected override void Awake()
    {
        base.Awake();
        indexDifficulty = 2;
        Debug.Log("Difficulty is at " + indexDifficulty + "/2");
    }

    public void SelectDifficulty(int i)
    {
        indexDifficulty = i;
    }
}