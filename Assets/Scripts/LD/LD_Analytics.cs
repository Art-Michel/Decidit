using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LD_Analytics : MonoBehaviour
{
    [HideInInspector] public float alive_Duration = 0;
    private Player player;
    //[SerializeField] private string[] ld_death_Names;
    //[SerializeField] private string[] murder_Names;
    //[SerializeField] private int playerKills;
    //[SerializeField] private bool hasWin = false;
    //private UnityEvent hasWinEvent;
    //[SerializeField] private float timeToFinish;
    //private canPlayTimer = true;
    

    ////Refs
    //public DungeonGenerator dungeonGenerator;
    //public PlayerManager playerManager;

    void Start()
    {
        //hasWinEvent = new UnityEvent();
        //hasWinEvent.AddListener(SetWinState);
        //playerManager._hasWinEvent = hasWinEvent;

        player = FindObjectOfType<Player>();
        alive_Duration = 0;
    }

    void Update()
    {
        //if (canPlayTimer)
        //{
        //    timeToFinish += Time.deltaTime;
        //}
        //Debug.Log(hasWin);

        if (player.enabled)
        {
            AliveTime();
        }
        else
        {
            PlayerPrefs.SetFloat("alive_Duration" + gameObject.name, alive_Duration);
            alive_Duration = PlayerPrefs.GetFloat("alive_Duration" + gameObject.name);
        }
    }

    private void AliveTime()
    {
        alive_Duration += Time.deltaTime;
        Debug.Log(alive_Duration + "s");
    }

    //public void SetWinState()
    //{
    //    canPlayTimer = false;
    //    hasWin = true;
    //    Debug.Log(timeToFinish);
    //}
}
