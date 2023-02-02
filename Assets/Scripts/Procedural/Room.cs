using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector3 Size => _size;
    public GameObject Enter => _enter;
    public GameObject Exit => _exit;
    [SerializeField] private Vector3 _size;
    [SerializeField] private GameObject _enter;
    [SerializeField] private GameObject _exit;

    [SerializeField] List<EnemyHealth> _IAList;

    public int CurrentEnemiesInRoom;
    int _currentRoom;

    private void Awake()
    {
        CurrentEnemiesInRoom = _IAList.Count;
        _exit.GetComponent<MeshRenderer>().enabled = true;
        _exit.GetComponent<BoxCollider>().isTrigger = false;
        ExitDoor();
    }

    private void Update()
    {

    }

    public void ExitDoor()
    {
        if (CurrentEnemiesInRoom <= 0)
        {
            _exit.GetComponent<MeshRenderer>().enabled = false;
            _exit.GetComponent<BoxCollider>().isTrigger = true;
        }
    }
    public GameObject GetDoorsExit()
    {
        return (_exit);
    }

    public void RoomEnter()
    {
        _enter.SetActive(true);
        _enter.GetComponent<MeshRenderer>().enabled = true;
        _enter.GetComponent<BoxCollider>().isTrigger = false;
    }
    public void RoomExit()
    {
        if (_exit.CompareTag("ExitDoor"))
        {
            DungeonGenerator.Instance.GetRoom().SetActive(false);
            DungeonGenerator.Instance.IncrementCurrentRoom();
        }
        _exit.SetActive(true);
        _exit.GetComponent<MeshRenderer>().enabled = true;
        _exit.GetComponent<BoxCollider>().isTrigger = false;
        DungeonGenerator.Instance.GetRoom().SetActive(true);
    }
}
