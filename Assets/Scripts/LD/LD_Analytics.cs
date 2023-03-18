using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LD_Analytics : MonoBehaviour
{
    [HideInInspector] public float alive_Duration = 0;
    public bool seeMore = false;
    private Player player;
    public List<Room> levels;
    [SerializeField] private TrailRenderer playerPath;
    public bool displayPlayerPath = false;
    private TrailRenderer trailInstance;

    void Start()
    {
        #region Ref
        player = FindObjectOfType<Player>();
        levels = gameObject.GetComponent<DungeonGenerator>().GetRooms();
        alive_Duration = 0;
        #endregion

        trailInstance = Instantiate(playerPath, player.transform);
        HideTrail();
    }

    void Update()
    {
        #region Alive Timer
        if (player.enabled)
        {
            Alive();
        }
        else
        {
            Dead();
        }
        #endregion

        #region Player Path
        if (displayPlayerPath)
        {
            DisplayTrail();
        }
        else
        {
            HideTrail();
        }
        #endregion
    }

    /// <summary>
    /// is PLAYER alive
    /// </summary>
    private void Alive()
    {
        alive_Duration += Time.deltaTime;
        //Debug.Log(alive_Duration + "s");

        PlayerPrefs.SetInt("dead", 0);
    }

    /// <summary>
    /// is PLAYER dead
    /// </summary>
    private void Dead()
    {
        PlayerPrefs.SetFloat("alive_Duration", alive_Duration);
        alive_Duration = PlayerPrefs.GetFloat("alive_Duration");
        
        PlayerPrefs.SetInt("dead", 1);
    }

    public void DisplayTrail()
    {
        trailInstance.GetComponent<TrailRenderer>().widthMultiplier = 1;
    }

    public void HideTrail()
    {
        trailInstance.GetComponent<TrailRenderer>().widthMultiplier = 0;
    }
}
