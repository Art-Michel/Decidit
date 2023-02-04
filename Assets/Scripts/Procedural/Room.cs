using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;

public class Room : MonoBehaviour
{
    [Header("References")]
    [SerializeField] List<EnemyHealth> _enemiesList;
    [SerializeField] List<Door> _doors;
    public Door Entry => _doors[0];
    public Door Exit => _doors[1];

    [Header("Settings")]
    [SerializeField] private bool _isCorridor = false;

    public int CurrentEnemiesInRoom;

    [Button]
    private void CountEnemies()
    {
        _enemiesList.Clear();
        foreach (EnemyHealth enemy in GetComponentsInChildren<EnemyHealth>())
        {
            _enemiesList.Add(enemy);
        }
    }

    [Button]
    private void FindDoors()
    {
        _doors.Clear();
        foreach (Door door in GetComponentsInChildren<Door>())
        {
            _doors.Add(door);
        }
    }

    public void EnableEnemies(bool b)
    {
        foreach (EnemyHealth enemyHealth in _enemiesList)
        {
            enemyHealth.gameObject.SetActive(b);
        }
        CurrentEnemiesInRoom = _enemiesList.Count;
    }

    public void EnterRoom()
    {
        DungeonGenerator.Instance.IncrementCurrentRoom();
        DungeonGenerator.Instance.GetRoom(-1).gameObject.SetActive(false);
        this.Entry.CloseDoor();

        if (_isCorridor)
        {
            DungeonGenerator.Instance.GetRoom(1).gameObject.SetActive(true);
            DungeonGenerator.Instance.GetRoom(2).gameObject.SetActive(true);
        }

        else
        {
            this.EnableEnemies(true);
        }
    }

    public void ExitRoom()
    {
        if (_isCorridor)
        {
            this.Exit.OpenDoor();
            DungeonGenerator.Instance.GetRoom(1).Entry.OpenDoor();
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
            GameManager.Instance.StartSlowMo(0.01f, 2f);
            this.Exit.OpenDoor();
            DungeonGenerator.Instance.GetRoom(1).Entry.OpenDoor();
        }
    }
}
