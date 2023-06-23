using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameObject _doorMesh;
    [SerializeField] GameObject _doorTrigger;
    public Room ThisDoorsRoom;
    [SerializeField] Animator animatorDoor;
    [SerializeField] float delayCloseDoor;

    [Header("Door Settings")]
    [SerializeField] bool _isExit;

    public bool HasBeenTriggered = false;

    void Awake()
    {
        animatorDoor = GetComponent<Animator>();
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
            this.ThisDoorsRoom.EnterRoom(delayCloseDoor);
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

        animatorDoor.SetBool("Close", true);

        SoundManager.Instance.PlaySound("event:/SFX_Environement/DoorClosing", 1f, gameObject);
    }

    public void OpenDoor()
    {
        if (_doorMesh == null)
        {
            Debug.Log("No door to open");
            return;
        }

        if (ThisDoorsRoom.Altar != null)
            ThisDoorsRoom.Altar.SetActive(true);
        animatorDoor.SetBool("Open", true);

        SoundManager.Instance.PlaySound("event:/SFX_Environement/DoorOpening", 1f, gameObject);
    }
}
