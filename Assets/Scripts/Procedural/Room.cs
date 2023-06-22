using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;

public class Room : MonoBehaviour
{
    [SerializeField] List<EnemyHealth> _enemiesList;
    List<Door> _doors;
    public List<TriggerActiveMobs> Triggers;

    public Door Entry;
    public Door Exit;
    [SerializeField] private GameObject _levelArtParent;
    public GameObject Altar;

    [SerializeField] private bool _isCorridor = false;
    [SerializeField] private int _triggersToActivateOnClear = 2;

    [NonSerialized] public int CurrentEnemiesInRoom;

    void Awake()
    {
        // // this.transform.parent = DungeonGenerator.Instance.transform;
        // _ennemiesParentList = GameObject.FindGameObjectsWithTag("ListEnnemies");
        // for (int i = 0; i < _ennemiesParentList.Length; i++)
        // {
        //     _ennemiesParentList[i].SetActive(false);
        // }
    }

    public void InitAltar()
    {
        if (this.Altar != null)
        {
            this.Altar.GetComponent<Altar>().AddAltarToStaticList();
        }
    }

    // [Button]
    public void FindDoors()
    {
        _doors = new List<Door>();
        foreach (Door door in GetComponentsInChildren<Door>())
        {
            _doors.Add(door);
            door.ThisDoorsRoom = this;
        }
        Entry = _doors[0];
        Exit = _doors[1];
    }

    public void FindTriggers()
    {
        Triggers = new List<TriggerActiveMobs>();
        foreach (TriggerActiveMobs trigger in GetComponentsInChildren<TriggerActiveMobs>())
        {
            Triggers.Add(trigger);
            trigger.thisTriggersRoom = this;
            trigger.ChooseAPool();
        }
    }

    // [Button]
    public void CountEnemies()
    {
        _enemiesList = new List<EnemyHealth>();

        foreach (EnemyHealth enemy in GetComponentsInChildren<EnemyHealth>(includeInactive: true))
        {
            _enemiesList.Add(enemy);
            enemy.Room = this;
            // enemy.gameObject.SetActive(false);
        }
        CurrentEnemiesInRoom = _enemiesList.Count;
    }


    [SerializeField] Material[] _blockingMats;

    [Button]
    public void ShowBlockingMeshes()
    {

        foreach (Material mat in _blockingMats)
        {
            foreach (MeshRenderer mr in GameObject.FindObjectsOfType<MeshRenderer>().Where(t => t.sharedMaterial == mat))
                mr.enabled = true;
        }
    }
    [Button]
    public void HideBlockingMeshes()
    {

        foreach (Material mat in _blockingMats)
        {
            foreach (MeshRenderer mr in GameObject.FindObjectsOfType<MeshRenderer>().Where(t => t.sharedMaterial == mat))
                mr.enabled = false;
        }
    }

    public void StartBattleRoom()
    {
        PlayerManager.Instance.RechargeEverything();
        // foreach (EnemyHealth enemyHealth in _enemiesList)
        // {
        //     if (enemyHealth == null)
        //     {
        //         Debug.LogError("La room [" + this.gameObject.name + "] n'a pas d'ennemi assigné");
        //     }
        //     enemyHealth.gameObject.SetActive(b);
        //     if (enemyHealth.isActiveAndEnabled)
        //     {
        //         enemyHealth.SetDissolve();
        //         enemyHealth.StartCoroutine("DissolveInverse");
        //     }
        // }
    }

    public void EnterRoom(float delayDisableLastRoom)
    {
        DungeonGenerator.Instance.SetCurrentRoom(this);
        Invoke("DisableLastRoom", delayDisableLastRoom);
        CountEnemies();
        this.Entry.CloseDoor();
        Killplane.Instance.MoveSpawnPointTo(this.Entry.transform.position + this.Entry.transform.forward * 4.0f + Vector3.up * 2);

        if (_isCorridor)
        {
            DungeonGenerator.Instance.GetRoom(1).gameObject.SetActive(true);
            DungeonGenerator.Instance.GetRoom(2).gameObject.SetActive(true);
            TimerManager.Instance.isInCorridor = true;
        }

        else
        {
            StartBattleRoom();
            // _ennemiesParentList[UnityEngine.Random.Range(0, 2)].SetActive(true);
            // this.EnableEnemies(true);
        }
    }
    void DisableLastRoom()
    {
        DungeonGenerator.Instance.GetRoom(-1).gameObject.SetActive(false);
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
        // Debug.Log(CurrentEnemiesInRoom + "/" + _enemiesList.Count + " enemies still alive in room '" + gameObject.name + "'");
        if (CurrentEnemiesInRoom <= 0)
        {
            Debug.Log("FinishRoom");
            FinishRoom();
            return;
        }

        bool anyMobAlive = false;
        for (int i = 0; i < _enemiesList.Count; i++)
        {
            if (_enemiesList[i] != null)
                anyMobAlive = anyMobAlive || (_enemiesList[i].gameObject.activeInHierarchy && !_enemiesList[i].IsDying);
            // Debug.Log(anyMobAlive);
        }

        if (CurrentEnemiesInRoom > 0 && !anyMobAlive)
            EnableClosestTriggers();
    }

    private void EnableClosestTriggers()
    {
        TriggerActiveMobs[] closest = Triggers.Where(t => !t._triggered).OrderBy(t => (Vector3.Distance(t.transform.position, Player.Instance.transform.position))).ToArray();
        for (int i = 0; i < closest.Length; i++)
        {
            if (i < _triggersToActivateOnClear)
                closest[i].EnableMobs();
        }
    }

    internal void Statify()
    {
        if (_levelArtParent)
        {
            this._levelArtParent.isStatic = true;
            StaticBatchingUtility.Combine(_levelArtParent);
        }
    }

    private void FinishRoom()
    {
        //Feedbacks
        SoundManager.Instance.PlaySound("event:/SFX_Environement/SlowMo", 1f, gameObject);
        SoundManager.Instance.ClearedSound();
        // PlayerHealth.Instance.TrueHeal(1);
        PlayerManager.Instance.StartSlowMo(0.01f, 2f);
        PlayerManager.Instance.StartFlash(1.0f, 1);

        //Progress in dungeon
        this.Exit.OpenDoor();
        if (DungeonGenerator.Instance != null)
        {
            DungeonGenerator.Instance.GetRoom(1).gameObject.SetActive(true);
            DungeonGenerator.Instance.GetRoom(1).Entry.OpenDoor();
        }

        // Break Timer
        TimerManager.Instance.isInCorridor = true;
    }

    public void CheckIfFirstTrigger()
    {
        if (_enemiesList.Count == CurrentEnemiesInRoom)
        {
            TimerManager.Instance.isInCorridor = false;
            SoundManager.Instance.FightingSound();
            SoundManager.Instance.PlaySound("event:/SFX_Environement/StartFight", 1f, gameObject);
        }

    }
}
