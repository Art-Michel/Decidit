using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameObject _doorMesh;
    [SerializeField] Room _room;

    [Header("Door Settings")]
    [SerializeField] bool _isExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isExit)
            {
                _room.ExitRoom();
            }
            else
            {
                _room.EnterRoom();
            }
        }
        this.enabled = false;
    }

    public void CloseDoor()
    {
        _doorMesh.transform.localPosition = transform.position + Vector3.up * 3;
        //mettre animation ici à la place
    }

    public void OpenDoor()
    {
        _doorMesh.transform.localPosition = transform.position;
        //là aussi
    }
}
