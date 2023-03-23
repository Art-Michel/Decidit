using UnityEngine;

public class ApplyDifficulty : ProjectManager<ApplyDifficulty>
{
    [Range((int)0, (int)4)]
    public int indexDifficulty; // 0 ez / 1 Med / 2 Hard

    public void SelectDifficulty(int i)
    {
        indexDifficulty = i;
    }
}