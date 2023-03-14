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

    public bool HasBeenTriggered = false;

    void Awake()
    {
        if (this.ThisDoorsRoom == null)
            Debug.LogError("No assigned Room! Click the 'Find Rooms' button in " + transform.parent.name);
    }

    void Start()
    {
        HasBeenTriggered = false;
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
        if (HasBeenTriggered)
            return;

        if (_isExit)
        {
            this.ThisDoorsRoom.ExitRoom();
        }
        else
        {
            this.ThisDoorsRoom.EnterRoom();
        }
        HasBeenTriggered = true;
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
        
        //TODO Lucas SoundManager.Instance.PlaySound("event:/SFX_Environement/DoorClosing", 1f);
        //TODO Lucas SoundManager.Instance.PlaySound("event:/SFX_Environement/StartFight", 1f);
    }

    public void OpenDoor()
    {
        if (_doorMesh == null)
        {
            Debug.Log("No door to open");
            return;
        }

        _doorMesh.transform.localPosition = Vector3.up * 8;
        //TODO Lucas SoundManager.Instance.PlaySound("event:/SFX_Environement/DoorOpening", 1f);
        //là aussi
    }
}
