using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePooling : MonoBehaviour
{
    [SerializeField] List<int> ScenesMedium;
    [SerializeField] List<int> ScenesHard;
    [SerializeField] List<int> ScenesEasy;
    // [SerializeField] int corridor;
    // [SerializeField] int altar;
    [SerializeField] int nbRooms;


    void Start()
    {
        Generator();
    }

    public void Generator()
    {
        for (int i = 0; i < nbRooms; i++)
        {
            SceneManager.LoadScene(ScenesHard[Random.Range(0, ScenesHard.Count)], LoadSceneMode.Additive);
        }
    }
}
