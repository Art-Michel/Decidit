using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameObject _doorMesh;
    [SerializeField] GameObject _doorTrigger;
    public Room ThisDoorsRoom;

    [Header("Door Settings")]
    [SerializeField] bool _isExit;

    bool _hasBeenTriggered = false;

    void Awake()
    {
        if (this.ThisDoorsRoom == null)
            Debug.LogError("No assigned Room! Click on this rooms ''Find Rooms'' button.");
    }

    void Start()
    {
        _hasBeenTriggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Trigger();
        }
    }

    public void Trigger()
    {
        if (_hasBeenTriggered)
            return;

        if (_isExit)
        {
            this.ThisDoorsRoom.ExitRoom();
        }
        else
        {
            this.ThisDoorsRoom.EnterRoom();
        }
        _hasBeenTriggered = true;
    }

    public void CloseDoor()
    {
        if (_doorMesh == null)
        {
            Debug.Log("No door to close");
            return;
        }

        _doorMesh.transform.localPosition = Vector3.up * 2;
        //mettre animation ici à la place
    }

    public void OpenDoor()
    {
        if (_doorMesh == null)
        {
            Debug.Log("No door to open");
            return;
        }

        _doorMesh.transform.localPosition = Vector3.up * 8;
        //là aussi
    }
}
