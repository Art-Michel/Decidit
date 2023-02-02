using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("References")]
    [SerializeField] List<EnemyHealth> _enemiesList;
    public Door Entry;
    public Door Exit;

    [Header("Settings")]
    private bool _isCorridor;

    [System.NonSerialized] public int CurrentEnemiesInRoom;
    int _currentRoom;

    [Button]
    private void CountEnemies()
    {
        _enemiesList.Clear();
        foreach (EnemyHealth enemy in GetComponentsInChildren<EnemyHealth>())
        {
            _enemiesList.Add(enemy);
        }
    }

    void Start()
    {
        CurrentEnemiesInRoom = _enemiesList.Count;
        CheckForEnemies();
    }

    public void EnterRoom()
    {
        DungeonGenerator.Instance.IncrementCurrentRoom();

        if (_isCorridor)
        {
            DungeonGenerator.Instance.GetRoom().Entry.CloseDoor();
            DungeonGenerator.Instance.GetRoom(-1).Entry.gameObject.SetActive(false);
            DungeonGenerator.Instance.GetRoom(1).gameObject.SetActive(true);
            DungeonGenerator.Instance.GetRoom(2).gameObject.SetActive(true);
        }

        else
        {
            //Enable Enemies in current room;
            DungeonGenerator.Instance.GetRoom().Entry.CloseDoor();
            DungeonGenerator.Instance.GetRoom().Exit.CloseDoor();
            DungeonGenerator.Instance.GetRoom(-1).gameObject.SetActive(false);
        }
    }

    public void ExitRoom()
    {
        if (_isCorridor)
        {
            this.Exit.GetComponent<Door>().OpenDoor();
        }

        else
        {
            //Nothing
        }
    }

    public void CheckForEnemies()
    {
        if (CurrentEnemiesInRoom <= 0)
        {
            this.Exit.OpenDoor();
            DungeonGenerator.Instance.GetRoom(1).Entry.OpenDoor();
        }
    }
}
