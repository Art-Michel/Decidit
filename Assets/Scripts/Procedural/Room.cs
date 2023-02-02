using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _enter;
    [SerializeField] private GameObject _exit;
    [SerializeField] List<EnemyHealth> _enemiesList;
    public GameObject Enter => _enter;
    public GameObject Exit => _exit;

    [Header("Settings")]
    private bool _isCorridor;

    [System.NonSerialized] public int CurrentEnemiesInRoom;
    [System.NonSerialized] public DungeonGenerator Dungeon;
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
        if (_isCorridor)
        {
            //Load Room+1
            //Load Corridor+1
        }

        else
        {

        }
    }

    public void ExitRoom()
    {
        if (_isCorridor)
        {

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
            //_exit.GetComponent<Door>().OpenDoor();
            //Dungeon.Corridors[Dungeon.GetCurrentRoom() + 1].OpenDoor();;
            //Corridor+1.OpenDoor()
        }
    }
}
