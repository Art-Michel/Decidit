using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance = null;
    [SerializeField] int _seed;
    [SerializeField] bool _randomizeSeed;
    [SerializeField] float _dungeonRotation;
    [SerializeField] RoomSetup _starterRoom;
    [SerializeField] RoomSetup _finalRoom;
    [SerializeField] int _length;
    [SerializeField] int _currentRoom;
    public List<RoomSetup> Rooms;
    public List<RoomSetup> Corridors;
    List<GameObject> _roomsPrefabs;

    [SerializeField] List<Room> _roomGen;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        Instance = this;
        _roomsPrefabs = new List<GameObject>();
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
            Debug.Log(_seed.ToString());
        }

        Random.InitState(_seed);

        _roomGen = new List<Room>(_length + 2 + (_length + 1));
        _roomGen.Add(_starterRoom.Get());

        // variables pour r�partir la difficult� (difficulty) en fonction du nombre de salles et de la longueur du donjon (stackCount)
        int stackCount = Mathf.RoundToInt(_length / 3f);
        int difficulty = 0;

        for (int i = 0; i < _length; i++)
        {
            _roomGen.Add(Corridors[Random.Range(0, Corridors.Count)].Get());

            // passe � la difficult� suivante
            if (stackCount <= 0)
            {
                difficulty++;
                difficulty = Mathf.Clamp(difficulty, 0, Rooms.Count - 1);
                stackCount = Mathf.RoundToInt(_length / 3f);
            }

            // ajoute une salle avec une difficult� pr�d�fini
            _roomGen.Add(Rooms[difficulty].Get());
            stackCount--;
        }

        _roomGen.Add(Corridors[Random.Range(0, Corridors.Count)].Get());
        _roomGen.Add(_finalRoom.Get());
        Build();
    }

    private void Build()
    {
        GameObject lastDoor = null;
        foreach (Room room in _roomGen)
        {
            Room instance = Instantiate(room, Vector3.zero, Quaternion.identity, transform);
            instance.gameObject.SetActive(true);
            instance.Dungeon = this;
            if (instance.gameObject.CompareTag("LD"))
            {
                _roomsPrefabs.Add(instance.gameObject);
            }
            // setup rotation
            instance.transform.rotation = Quaternion.Euler(0, _dungeonRotation, 0);

            if (lastDoor != null)
            {
                Vector3 direction = lastDoor.transform.position - instance.Enter.transform.position;
                instance.transform.position = direction;
            }

            lastDoor = instance.Exit;
            for (int i = 0; i < _roomsPrefabs.Count; i++)
            {
                _roomsPrefabs[i].gameObject.SetActive(false);
                _roomsPrefabs[0].gameObject.SetActive(true);
            }
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void ResetDungeon()
    {
        //transform.DetachChildren();
        _roomGen.Clear();
        _roomsPrefabs.Clear();
    }

    public void ClearDungeon()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public GameObject GetRoom()
    {
        return (_roomsPrefabs[_currentRoom]);
    }

    public int GetCurrentRoom()
    {
        return (_currentRoom);
    }
    public void IncrementCurrentRoom()
    {
        _currentRoom++;
    }

    public List<GameObject> GetRoomsPrefabs()
    {
        return (_roomsPrefabs);
    }

}
