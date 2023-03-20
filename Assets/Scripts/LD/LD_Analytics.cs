using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LD_Analytics : MonoBehaviour
{
    [HideInInspector] public float alive_Duration = 0;
    public bool seeMore = false;
    private Player player;
    [Header("Ref")]
    public List<GameObject> levels = new List<GameObject>();
    [SerializeField] private TrailRenderer playerPath;
    [Header("Data")]
    public bool displayPlayerPath = false;
    private TrailRenderer trailInstance;

    void Start()
    {
        #region Ref
        player = FindObjectOfType<Player>();
        foreach (Transform level in this.transform)
        {
            levels.Add(level.gameObject);
        }
        trailInstance = Instantiate(playerPath, player.transform);
        alive_Duration = 0;
        #endregion

        HideTrail();
    }

    void Update()
    {
        #region Alive Timer
        if (player.GetComponent<PlayerHealth>().GetHP() > 0)
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

        float minutes = (int)alive_Duration / 60f;
        float secondes = (minutes - Mathf.FloorToInt(minutes)) * 60f;
        Debug.Log("Your are dead, your time alive was : " + Mathf.FloorToInt(minutes) + "min" + secondes + "s");
        //UnityEditor.EditorApplication.isPaused = true;
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
