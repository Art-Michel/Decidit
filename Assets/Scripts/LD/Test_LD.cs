using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Test_LD : MonoBehaviour
{
    public List<GameObject> rooms = new List<GameObject>();
    [SerializeField] private GameObject player;

    public void HideRooms(GameObject choosenRoom)
    {
        foreach (GameObject room in rooms)
        {
            if(room != choosenRoom)
            {
                room.SetActive(false);
            }
            else
            {
                room.SetActive(true);
            }
        }
    }

    public void SetSpawn(Transform pos)
    {
        player.transform.position = pos.position;
    }
}
