using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector3 Size => size;
    public GameObject Enter => enter;
    public GameObject Exit => exit;
    [SerializeField] private Vector3 size;
    [SerializeField] private GameObject enter;
    [SerializeField] private GameObject exit;

    [SerializeField] List<EnemyHealth> _IAList;

    public int NbIA;
    int _currentRoom;

    private void Awake()
    {
        NbIA = _IAList.Count;
        exit.GetComponent<MeshRenderer>().enabled = true;
        exit.GetComponent<BoxCollider>().isTrigger = false;
        ExitDoor();
    }

    private void Update()
    {

    }

    public void ExitDoor()
    {
        if (NbIA <= 0)
        {
            exit.GetComponent<MeshRenderer>().enabled = false;
            exit.GetComponent<BoxCollider>().isTrigger = true;
        }
    }
    public GameObject GetDoorsExit()
    {
        return (exit);
    }

    public void RoomEnter()
    {
        enter.SetActive(true);
        enter.GetComponent<MeshRenderer>().enabled = true;
        enter.GetComponent<BoxCollider>().isTrigger = false;
    }
    public void RoomExit()
    {
        if (exit.CompareTag("ExitDoor"))
        {
            DungeonGenerator.Instance.GetRoom().SetActive(false);
            DungeonGenerator.Instance.AddCurentRoom();
        }
        exit.SetActive(true);
        exit.GetComponent<MeshRenderer>().enabled = true;
        exit.GetComponent<BoxCollider>().isTrigger = false;
        DungeonGenerator.Instance.GetRoom().SetActive(true);
    }
}
