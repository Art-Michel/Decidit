using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameObject _doorMesh;
    [SerializeField] Collider _doorTrigger;
    [SerializeField] Room _room;

    [Header("Door Settings")]
    [SerializeField] bool _isExit;

    void Awake()
    {
        if (_room == null)
            _room = GetComponentInParent<Room>();
    }

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
        _doorTrigger.enabled = false;
    }

    public void CloseDoor()
    {
        if (_doorMesh == null)
        {
            Debug.Log("No door to close");
            return;
        }

        _doorMesh.transform.localPosition = Vector3.zero;
        //mettre animation ici à la place
    }

    public void OpenDoor()
    {
        if (_doorMesh == null)
        {
            Debug.Log("No door to open");
            return;
        }

        _doorMesh.transform.localPosition = Vector3.up * 6;
        //là aussi
    }
}
