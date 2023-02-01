using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    [SerializeField] bool _isExit;
    [SerializeField] bool _isStartEnd;
    [SerializeField] Room room;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(_isExit)
            {
                room.RoomExit();
            }
            else
            {
                room.RoomEnter();
            }
        }
    }
}
