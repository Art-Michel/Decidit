using System.Collections.Specialized;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DungeonGenerator : LocalManager<DungeonGenerator>
{
    [SerializeField] int _seed;
    [SerializeField] bool _randomizeSeed;
    [SerializeField] float _dungeonRotation;
    [SerializeField] int[] _difficultyPerRoom;
    private int _numberOfRooms;
    [SerializeField] int _firstPowerupAfterRoom;
    [SerializeField] int _secondPowerupAfterRoom;

    private List<int> _subdifficultiesLeftToUse;


    [SerializeField] RoomSetup _starterRoom;
    [SerializeField] RoomSetup _finalRoom;
    public List<RoomSetup> RoomSets;
    public List<RoomSetup> Corridors;
    public int TotalRooms { get { return _actualRooms.Count - 1; } }
    List<Room> _actualRooms;
    public int CurrentRoom;


    [SerializeField] List<Room> _rooms;

    protected override void Awake()
    {
        base.Awake();
        _actualRooms = new List<Room>();
    }

    void Start()
    {
        _numberOfRooms = _difficultyPerRoom.Length;
        InitializeDifficultiesToUse();
        ResetDungeon();
        Generate();
    }

    private void InitializeDifficultiesToUse()
    {
        _subdifficultiesLeftToUse = new List<int>();
        for (int i = 0; i < _numberOfRooms; i++)
        {
            _subdifficultiesLeftToUse.Add(i);
        }
    }

    public void Generate()
    {
        if (transform.childCount > 0)
        {
            ResetDungeon();
            ClearDungeon();
        }

        //* randomizing seed
        if (_randomizeSeed)
        {
            _seed = Random.Range(0, 1000);
        }

        Random.InitState(_seed);

        _rooms = new List<Room>(_numberOfRooms + 2 + (_numberOfRooms + 1));
        _rooms.Add(_starterRoom.Get());

        //* variables pour r�partir la difficult� (difficulty) en fonction du nombre de salles et de la longueur du donjon (stackCount)
        int stackCount = Mathf.RoundToInt(_numberOfRooms / 3f);
        int difficulty = 0;

        for (int i = 0; i < _numberOfRooms; i++)
        {
            _rooms.Add(Corridors[Random.Range(0, Corridors.Count)].Get());

            //* passe � la difficult� suivante
            if (stackCount <= 0)
            {
                difficulty++;
                difficulty = Mathf.Clamp(difficulty, 0, RoomSets.Count - 1);
                stackCount = Mathf.RoundToInt(_numberOfRooms / 3f);
            }

            //* ajoute une salle avec une difficult� pr�d�fini
            _rooms.Add(RoomSets[difficulty].Get());
            stackCount--;
        }

        _rooms.Add(Corridors[Random.Range(0, Corridors.Count)].Get());
        _rooms.Add(_finalRoom.Get());
        Build();
    }

    private void Build()
    {
        GameObject lastDoor = null;
        foreach (Room room in _rooms)
        {
            //* Spawn room
            Room roomInstance = Instantiate(room, Vector3.zero, Quaternion.identity, transform);
            roomInstance.gameObject.SetActive(true);

            //* set rotation
            roomInstance.transform.rotation = Quaternion.Euler(0, _dungeonRotation, 0);

            //*set position
            if (lastDoor != null)
            {
                Vector3 roomPosition = (lastDoor.transform.position + Vector3.forward * 5) - (roomInstance.Entry.transform.position);
                roomInstance.transform.position = roomPosition;
            }

            //*Disable Room and its enemies
            lastDoor = roomInstance.Exit.gameObject;
            roomInstance.gameObject.SetActive(false);
            roomInstance.EnableEnemies(false);

            _actualRooms.Add(roomInstance);
        }

        EnableFirstRooms();
    }

    private void EnableFirstRooms()
    {
        _actualRooms[0].gameObject.SetActive(true);
        _actualRooms[0].Exit.OpenDoor();
        _actualRooms[1].gameObject.SetActive(true);
        _actualRooms[1].Entry.OpenDoor();
    }

    private void ResetDungeon()
    {
        //transform.DetachChildren();
        _rooms.Clear();
        _actualRooms.Clear();
    }

    public void ClearDungeon()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public Room GetRoom(int i = 0)
    {
        return _actualRooms[Mathf.Clamp(CurrentRoom + i, 0, TotalRooms)];
    }

    public void SetCurrentRoom(Room room)
    {
        CurrentRoom = _actualRooms.IndexOf(room);
    }

    public int GetRoomIndex(Room room)
    {
        return _actualRooms.IndexOf(room);
    }

    public void Endgame()
    {
        MenuManager.Instance.OpenWin();
    }
}
