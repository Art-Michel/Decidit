using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] int seed;
    [SerializeField] RoomSetup starterRoom;
    [SerializeField] RoomSetup finalRoom;
    [SerializeField] int length;
    [SerializeField] List<RoomSetup> rooms;
    [SerializeField] List<RoomSetup> corridors;

    List<Room> roomGen;

    private void Awake()
    {
        Generate();
    }

    public void Generate()
    {
        Random.InitState(seed);

        roomGen = new List<Room>(length + 2 + (length + 1));
        roomGen.Add(starterRoom.Get());
        for (int i = 0; i < length; i++)
        {
            roomGen.Add(corridors[Random.Range(0, corridors.Count)].Get());
            roomGen.Add(rooms[Random.Range(0, rooms.Count)].Get());
        }

        roomGen.Add(corridors[Random.Range(0, corridors.Count)].Get());
        roomGen.Add(finalRoom.Get());

        Build();
    }

    private void Build()
    {
        Doors lastDoor = null;

        foreach (Room room in roomGen)
        {
            Room instance = Instantiate(room, Vector3.zero, Quaternion.identity, transform);
            
            if(lastDoor != null)
            {
                Vector3 direction = lastDoor.transform.position - instance.Enter.transform.position;
                instance.transform.Translate(direction);
            }

            lastDoor = instance.Exit;
        }
    }
}
