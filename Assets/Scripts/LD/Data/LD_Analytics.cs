using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LD_Analytics : MonoBehaviour
{
    public bool seeMore = false;
    private Player player;
    [Header("Ref")]
    public LineRenderer playerPath;
    [Header("Debug")]
    public bool debugPlayerPath = false;
    public bool canSavePlayerPath = false;
    public int pathMaxSize = 100;
    public int dataMaxSize = 20;

    // other things
    [HideInInspector] public List<Vector3> trailRecorded;
    [HideInInspector] public float alive_Duration = 0;
    private float pathTimer=0f;
    private float pathTimerMax=1f;
    public string dataName;
    private bool isDead=false;

    void Start()
    {
        #region Ref
        player = FindObjectOfType<Player>();
        alive_Duration = 0;
        #endregion

        #region Reset
        trailRecorded.Clear();
        isDead = false;
        canSavePlayerPath = true;
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
        if (pathTimer > pathTimerMax && trailRecorded.Count < pathMaxSize)
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
        PlayerPrefs.SetFloat("alive_Duration", alive_Duration);
        alive_Duration = PlayerPrefs.GetFloat("alive_Duration");

        PlayerPrefs.SetInt("dead", 1);

        float minutes = (int)alive_Duration / 60f;
        float secondes = (minutes - Mathf.FloorToInt(minutes)) * 60f;
        Debug.Log("Your are dead, your time alive was : " + Mathf.FloorToInt(minutes) + "min" + secondes + "s");

        if (canSavePlayerPath)
        {
            if (!isDead)
            {
                #region Create Data .txt
                int dataID = Random.Range(0, 1000);

                string path = "Assets/Scripts/LD/Data/Paths/" + "seed_" + gameObject.GetComponent<DungeonGenerator>().GetSeed().ToString() + "_" + dataID + ".txt";
                int count = Directory.GetFiles("Assets/Scripts/LD/Data/Paths/").Length;
                if (count < 50)
                {
                    //Write some text to the test.txt file
                    StreamWriter writer = new StreamWriter(path, true);
                    foreach (Vector3 pos in trailRecorded)
                    {
                        writer.WriteLine(pos.x + ";" + pos.y + ";" + pos.z);
                    }
                    writer.Close();
                }
                else
                {
                    Debug.Log("You have too much data!");
                }
                #endregion
                isDead = true;
            }
        }
    }

    /// <summary>
    /// only for debuging player path
    /// </summary>
    public void DebugPlayerPath()
    {
        if (canSavePlayerPath)
        {
            int dataID = Random.Range(0, 1000);

            string path = "Assets/Scripts/LD/Data/Paths/" + "seed_" + gameObject.GetComponent<DungeonGenerator>().GetSeed().ToString() + "_" + dataID + ".txt";
            int count = Directory.GetFiles("Assets/Scripts/LD/Data/Paths/").Length;
            if (count < 50)
            {
                //Write some text to the test.txt file
                StreamWriter writer = new StreamWriter(path, true);
                foreach (Vector3 pos in trailRecorded)
                {
                    writer.WriteLine(pos.x + ";" + pos.y + ";" + pos.z);
                }
                writer.Close();
                //UnityEditor.EditorApplication.isPlaying = false;
            }
            else
            {
                Debug.Log("You have too much data!");
            }
        }
    }

    /// <summary>
    /// get the player path in the searched data seed
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetPositions()
    {
        trailRecorded.Clear();

        string path = "Assets/Scripts/LD/Data/Paths/" + dataName + ".txt";
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
