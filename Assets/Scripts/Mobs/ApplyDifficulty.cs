using UnityEngine;

public class ApplyDifficulty : LocalManager<ApplyDifficulty>
{
    [Range((int)0, (int)2)]
    public static int indexDifficulty; // 0 ez / 1 Med / 2 Hard

    public void SelectDifficulty(int i)
    {
        Debug.Log(i);
        indexDifficulty = i;
    }
}