using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

public class LD_Analytics : MonoBehaviour
{
    public bool seeMore = false;
    private Player player;
    [Header("Ref")]
    public LineRenderer playerPath;
    [Header("Debug")]
    public bool debugPlayerPath = false;
    public int trailRecorded_max = 100;

    // other things
    [HideInInspector] public List<Vector3> trailRecorded;
    [HideInInspector] public float alive_Duration = 0;
    private float pathTimer=0f;
    private float pathTimerMax=1f;
    public string pathSeed;

    void Start()
    {
        #region Ref
        player = FindObjectOfType<Player>();
        alive_Duration = 0;
        #endregion

        #region Reset
        trailRecorded.Clear();
        #endregion
    }

    void Update()
    {
        #region Debugs
        if (debugPlayerPath)
        {
            DebugPlayerPath();
        }
        #endregion

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

        #region create player path every second
        pathTimer += Time.deltaTime;
        if (pathTimer > pathTimerMax && trailRecorded.Count < trailRecorded_max)
        {
            trailRecorded.Add(player.transform.position);
            pathTimer = 0;
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
        //PlayerPrefs.SetFloat("alive_Duration", alive_Duration);
        //alive_Duration = PlayerPrefs.GetFloat("alive_Duration");
        
        //PlayerPrefs.SetInt("dead", 1);

        //float minutes = (int)alive_Duration / 60f;
        //float secondes = (minutes - Mathf.FloorToInt(minutes)) * 60f;
        //Debug.Log("Your are dead, your time alive was : " + Mathf.FloorToInt(minutes) + "min" + secondes + "s");

        //string path = "Assets/Scripts/LD/Data/Paths/" + gameObject.GetComponent<DungeonGenerator>().GetSeed().ToString() + ".txt";
        ////Write some text to the test.txt file
        //StreamWriter writer = new StreamWriter(path, true);
        //foreach (Vector3 pos in trailRecorded)
        //{
        //    writer.WriteLine(pos.x + ";" + pos.y + ";" + pos.z);
        //}
        //writer.Close();
        //UnityEditor.EditorApplication.isPlaying = false;
    }

    /// <summary>
    /// only for debuging player path
    /// </summary>
    public void DebugPlayerPath()
    {
        string path = "Assets/Scripts/LD/Data/Paths/"+ gameObject.GetComponent<DungeonGenerator>().GetSeed().ToString() + ".txt";
        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
        int count = dir.GetFiles().Length;
        if (count < 1)
        {
            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path, true);
            foreach (Vector3 pos in trailRecorded)
            {
                writer.WriteLine(pos.x + ";" + pos.y + ";" + pos.z);
            }
            writer.Close();
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    /// <summary>
    /// get the player path in the searched data seed
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetPositions()
    {
        trailRecorded.Clear();

        string path = "Assets/Scripts/LD/Data/Paths/" + pathSeed + ".txt";
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        //Debug.Log(reader.ReadToEnd());
        reader.Close();

        List<string> fileLines = File.ReadAllLines(path).ToList();
        foreach (string line in fileLines)
        {
            //Debug.Log(line);
            string[] sStrings = line.Split(";",StringSplitOptions.None);
            //Debug.Log(sStrings[1]);

            float x;
            float y;
            float z;

            if (float.TryParse(sStrings[0],out x))
            {
                x = float.Parse(sStrings[0]);
            }
            if (float.TryParse(sStrings[1], out y))
            {
                y = float.Parse(sStrings[1]);
            }
            if (float.TryParse(sStrings[2], out z))
            {
                z = float.Parse(sStrings[2]);
            }
            Vector3 newpos = new Vector3(x, y, z);
            trailRecorded.Add(newpos);
        }

        //Debug.Log(trailRecorded.ToArray().Length);
        return trailRecorded.ToArray();
    }
}
