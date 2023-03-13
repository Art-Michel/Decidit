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
            enemy.Room = this;
        }
    }

    [Button]
    private void FindDoors()
    {
        _doors.Clear();
        foreach (Door door in GetComponentsInChildren<Door>())
        {
            _doors.Add(door);
            door.ThisDoorsRoom = this;
        }
    }

    public void EnableEnemies(bool b)
    {
        CurrentEnemiesInRoom = _enemiesList.Count;
        foreach (EnemyHealth enemyHealth in _enemiesList)
        {
            if (enemyHealth == null)
            {
                Debug.LogError("La room [" + this.gameObject.name + "] n'a pas d'ennemi assigné");
            }
            enemyHealth.gameObject.SetActive(b);
        }
    }

    public void EnterRoom()
    {
        DungeonGenerator.Instance.SetCurrentRoom(this);
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
        if (DungeonGenerator.Instance.GetRoomIndex(this) != DungeonGenerator.Instance.CurrentRoom)
            return;

        if (DungeonGenerator.Instance.GetRoomIndex(this) >= DungeonGenerator.Instance.TotalRooms)
        {
            PlayerManager.Instance.OnPlayerWin();
        }

        if (_isCorridor)
        {
            this.Exit.OpenDoor();
            DungeonGenerator.Instance.GetRoom(1).Entry.OpenDoor();
        }

        else
        {
            //Nothing
        }

        this.Exit.HasBeenTriggered = true;
    }

    public void CheckForEnemies()
    {
        Debug.Log(CurrentEnemiesInRoom + " left in " + gameObject.name);
        if (CurrentEnemiesInRoom <= 0)
            FinishRoom();
    }

    private void FinishRoom()
    {
        //Feedbacks
        SoundManager.Instance.PlaySound("event:/SFX_Environement/SlowMo", 1f, gameObject);
        SoundManager.Instance.PlaySound("event:/SFX_Environement/StartFight", 1f, gameObject);
        //TODO Lucas calmer la musique ici
        PlayerManager.Instance.StartSlowMo(0.01f, 2f);

        //Progress in dungeon
        this.Exit.OpenDoor();
        if (DungeonGenerator.Instance != null)
            DungeonGenerator.Instance.GetRoom(1).Entry.OpenDoor();
    }
}
