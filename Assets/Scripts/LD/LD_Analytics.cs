using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LD_Analytics : MonoBehaviour
{
    [SerializeField] private string[] ld_death_Names;
    [SerializeField] private string[] murder_Names;
    [SerializeField] private float[] alive_Durations;
    [SerializeField] private int playerKills;
    [SerializeField] private bool hasWin = false;
    [SerializeField] private float timeToFinish;
    private DungeonGenerator dungeonGenerator;

    void Start()
    {
        dungeonGenerator = gameObject.GetComponent<DungeonGenerator>();
    }

    void Update()
    {
        timeToFinish += Time.deltaTime;
        Debug.Log(hasWin);
    }

    public void SetWinState()
    {
        hasWin = true;
        Debug.Log(timeToFinish);
    }
}
