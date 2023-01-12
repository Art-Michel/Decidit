using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    #region Variables
    public static Doors Instance = null;
    [SerializeField] int NbIA;

    [SerializeField] GameObject Door;
    [SerializeField] GameObject Door2;
    [SerializeField] GameObject Room;
    [SerializeField] GameObject Room2;

    
    #endregion

    private void Awake()
    {
        if(Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        Instance = this;
        //fait spawn la map de base et on enregistre le nombre d'enemies
        SetUp();
        NbIACheck();
    }

    void Start()
    {
        
    }

    void Update()
    {
        //on check si il a clean sa room
        if(NbIA <= 0)
        {
            //on ouvre la porte
            Door.SetActive(false);
            NbIA +=1;
        }
    }

    #region Becons
    public void NbIACheck()
    {
        NbIA = GameObject.FindGameObjectsWithTag("Ennemi").Length;
    }
    public void NbIASubqtract()
    {
        NbIA --;
    }
    private void SetUp()
    {
        Room.SetActive(true);
    }
    public GameObject GetDoor()
    {
        return(Door);
    }
    public GameObject GetDoor2()
    {
        return(Door2);
    }
    public GameObject GetRoom()
    {
        return(Room);
    }
    public GameObject GetRoom2()
    {
        return(Room2);
    }
    #endregion
}
