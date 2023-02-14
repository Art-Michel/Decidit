using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonGenerator : LocalManager<DungeonGenerator>
{
    [SerializeField] int _seed;
    [SerializeField] bool _randomizeSeed;
    [SerializeField] float _dungeonRotation;
    [SerializeField] RoomSetup _starterRoom;
    [SerializeField] RoomSetup _finalRoom;
    [SerializeField] int _length;
    public int CurrentRoom { get; private set; }
    public List<RoomSetup> RoomSets;
    public List<RoomSetup> Corridors;
    List<Room> _actualRooms;
    public int TotalRooms { get { return _actualRooms.Count - 1; } }


    [SerializeField] List<Room> _rooms;

    protected override void Awake()
    {
        base.Awake();
        _actualRooms = new List<Room>();
    }

    void Start()
    {
        ResetDungeon();
        Generate();
    }

    public void Generate()
    {
        if (transform.childCount > 0)
        {
            ResetDungeon();
            ClearDungeon();
        }

        // randomizing seed
        if (_randomizeSeed)
        {
            _seed = Random.Range(0, 1000);
        }

        Random.InitState(_seed);

        _rooms = new List<Room>(_length + 2 + (_length + 1));
        _rooms.Add(_starterRoom.Get());

        // variables pour r�partir la difficult� (difficulty) en fonction du nombre de salles et de la longueur du donjon (stackCount)
        int stackCount = Mathf.RoundToInt(_length / 3f);
        int difficulty = 0;

        for (int i = 0; i < _length; i++)
        {
            _rooms.Add(Corridors[Random.Range(0, Corridors.Count)].Get());

            // passe � la difficult� suivante
            if (stackCount <= 0)
            {
                difficulty++;
                difficulty = Mathf.Clamp(difficulty, 0, RoomSets.Count - 1);
                stackCount = Mathf.RoundToInt(_length / 3f);
            }

            // ajoute une salle avec une difficult� pr�d�fini
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
            Room instance = Instantiate(room, Vector3.zero, Quaternion.identity, transform);
            instance.gameObject.SetActive(true);

            //* set rotation
            instance.transform.rotation = Quaternion.Euler(0, _dungeonRotation, 0);

            //*set position
            if (lastDoor != null)
            {
                Vector3 roomPosition = (lastDoor.transform.position + Vector3.forward * 5) - (instance.Entry.transform.position);
                instance.transform.position = roomPosition;
            }

            //*Disable Room and its enemies
            lastDoor = instance.Exit.gameObject;
            instance.gameObject.SetActive(false);
            instance.EnableEnemies(false);

            _actualRooms.Add(instance);
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
        int roomToReturn = CurrentRoom + i;
        if (roomToReturn < _actualRooms.Count)
        {
            return _actualRooms[roomToReturn];
        }
        else
        {
            Debug.LogWarning("The room you are trying to access is out of bounds, returning last room in list");
            return _actualRooms[_actualRooms.Count - 1];
        }
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
        SceneManager.LoadScene(3);
    }
}
