using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    #region Variables
    [SerializeField] int NbIA;
    [SerializeField] int indexRoom;
    [SerializeField] int indexDoor;

    [SerializeField] List<GameObject> Door;
    [SerializeField] List<GameObject> Room;

    #endregion

    private void Awake()
    {
        //on enregistre le nombre d'enemies
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
            Door[indexDoor].SetActive(false);
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
    public GameObject GetDoor()
    {
        return(Door[indexDoor]);
    }
    public void AddDoorIndex()
    {
        indexDoor ++;
    }
    public int GetDoorIndex()
    {
        return (indexDoor);
    }
    public GameObject GetRoom()
    {
        return(Room[indexRoom]);
    }
    public void AddRoomIndex()
    {
        indexRoom ++;
    }
    #endregion
}
