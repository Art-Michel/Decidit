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

    [SerializeField] bool _isStartEnd;

    public int NbIA;
    int _currentRoom;

    private void Awake()
    {
        NbIA = _IAList.Count;
    }

    private void Update()
    {
        ExitDoor();
    }

    public void ExitDoor()
    {
        if (!_isStartEnd)
        {
            if (NbIA <= 0)
            {
                exit.SetActive(false);
            }
        }
    }
    public GameObject GetDoorsExit()
    {
        return (exit);
    }

    public void RoomEnter()
    {
        enter.SetActive(true);
        DungeonGenerator.Instance.GetRoom().SetActive(false);
        DungeonGenerator.Instance.AddCurentRoom();
    }
    public void RoomExit()
    {
        exit.SetActive(true);
        DungeonGenerator.Instance.GetRoom().SetActive(true);
    }
}
