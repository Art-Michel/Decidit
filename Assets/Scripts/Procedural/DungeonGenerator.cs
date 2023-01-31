using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance = null;
    [SerializeField] int seed;
    [SerializeField] bool randomizeSeed;
    [SerializeField] float dungeonRotation;
    [SerializeField] RoomSetup starterRoom;
    [SerializeField] RoomSetup finalRoom;
    [SerializeField] int length;
    [SerializeField] List<RoomSetup> rooms;
    [SerializeField] List<RoomSetup> corridors;
    [SerializeField] List<GameObject> destroyObjects;
    [SerializeField] List<GameObject> _referenceRoom;

    List<Room> roomGen;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        Instance = this;
        Generate();
    }

    public void Generate()
    {
        if (destroyObjects.Count > length)
        {
            ClearDungeon();
        }

        // randomizing seed
        if (randomizeSeed)
        {
            seed = Random.Range(0, 1000);
            Debug.Log(seed.ToString());
        }

        Random.InitState(seed);

        roomGen = new List<Room>(length + 2 + (length + 1));
        roomGen.Add(starterRoom.Get());

        // variables pour r�partir la difficult� (difficulty) en fonction du nombre de salles et de la longueur du donjon (stackCount)
        int stackCount = Mathf.RoundToInt(length / 3f);
        int difficulty = 0;

        for (int i = 0; i < length; i++)
        {
            roomGen.Add(corridors[Random.Range(0, corridors.Count)].Get());

            // passe � la difficult� suivante
            if (stackCount <= 0)
            {
                difficulty++;
                difficulty = Mathf.Clamp(difficulty, 0, rooms.Count - 1);
                stackCount = Mathf.RoundToInt(length / 3f);
            }

            // ajoute une salle avec une difficult� pr�d�fini
            roomGen.Add(rooms[difficulty].Get());
            stackCount--;
        }

        roomGen.Add(corridors[Random.Range(0, corridors.Count)].Get());
        roomGen.Add(finalRoom.Get());

        Build();

        ResetDungeon();
    }

    private void Build()
    {
        GameObject lastDoor = null;

        foreach (Room room in roomGen)
        {
            Room instance = Instantiate(room, Vector3.zero, Quaternion.identity, transform);
            destroyObjects.Add(instance.gameObject);
            if (instance.gameObject.CompareTag("LD"))
            {
                _referenceRoom.Add(instance.gameObject);
            }
            // setup rotation
            instance.transform.rotation = Quaternion.Euler(0, dungeonRotation, 0);

            if (lastDoor != null)
            {
                Vector3 direction = lastDoor.transform.position - instance.Enter.transform.position;
                instance.transform.position = direction;
            }

            lastDoor = instance.Exit;
        }
        for (int i = 0; i < _referenceRoom.Count; i++)
        {
            _referenceRoom[i].gameObject.SetActive(false);
            _referenceRoom[1].gameObject.SetActive(true);
        }
    }

    private void ResetDungeon()
    {
        transform.DetachChildren();

        roomGen.Clear();
        _referenceRoom.Clear();
    }

    public void ClearDungeon()
    {
        foreach (GameObject obj in destroyObjects)
        {
            //Debug.Log("child name " + obj.name);
            DestroyImmediate(obj);
        }
        destroyObjects.Clear();
    }

    public List<Room> GetRoom()
    {
        return (roomGen);
    }
}
