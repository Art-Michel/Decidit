using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> rooms = new List<GameObject>();
    List<Vector3> roomsPlaced = new List<Vector3>();
    [SerializeField] int roomsCount;
    [SerializeField] float roomsDist;
    private Vector3 previousRoom;
    private float previousRandAxe;
    private int roomsCountMax;

    private void Awake()
    {
        roomsCountMax = roomsCount;

        Generate();
    }

    void Generate()
    {
        previousRoom = transform.position;

        while (roomsCount > 0)
        {
            int randAxe = Random.Range(1, 6 + 1);

            if(randAxe != previousRandAxe || roomsCount == roomsCountMax)
            {
                switch (randAxe)
                {
                    case 1:
                        Debug.Log("x");
                        Vector3 SpawnPos1 = new Vector3(previousRoom.x + roomsDist, previousRoom.y, previousRoom.z);

                        if (CanSpawn(SpawnPos1))
                        {
                            GameObject corridorInstance1 = Instantiate(rooms[6], SpawnPos1, Quaternion.identity);
                            roomsPlaced.Add(corridorInstance1.transform.position);
                            previousRoom = corridorInstance1.transform.position;

                            Vector3 SpawnPos1b = new Vector3(previousRoom.x + roomsDist, previousRoom.y, previousRoom.z);
                            GameObject roomInstance1 = Instantiate(rooms[ChooseRoomDir(previousRoom, SpawnPos1b)], SpawnPos1b, Quaternion.identity);
                            roomsPlaced.Add(roomInstance1.transform.position);
                            previousRoom = roomInstance1.transform.position;
                            roomsCount--;

                            if (roomsCount > 0)
                            {
                                Vector3 SpawnPos1d = new Vector3(previousRoom.x + roomsDist, previousRoom.y, previousRoom.z);
                                GameObject corridorInstance2 = Instantiate(rooms[6], SpawnPos1d, Quaternion.identity);
                                roomsPlaced.Add(corridorInstance2.transform.position);
                                previousRoom = corridorInstance2.transform.position;

                                Vector3 SpawnPos1c = new Vector3(previousRoom.x + roomsDist, previousRoom.y, previousRoom.z);
                                GameObject nextRoomInstance = Instantiate(rooms[ChooseRoomDir(previousRoom, SpawnPos1c)], SpawnPos1c, Quaternion.identity);
                                roomsPlaced.Add(nextRoomInstance.transform.position);
                                previousRoom = nextRoomInstance.transform.position;
                                roomsCount--;
                            }
                        }
                        else
                        {
                            randAxe = Random.Range(1, 6 + 1);
                        }
                        break;
                    case 2:
                        Debug.Log("y");
                        Vector3 SpawnPos2 = new Vector3(previousRoom.x, previousRoom.y + roomsDist / 3f, previousRoom.z);

                        if (CanSpawn(SpawnPos2))
                        {
                            GameObject roomInstance2 = Instantiate(rooms[ChooseRoomDir(previousRoom, SpawnPos2)], SpawnPos2, Quaternion.identity);
                            roomsPlaced.Add(roomInstance2.transform.position);
                            previousRoom = roomInstance2.transform.position;
                            roomsCount--;
                        }
                        else
                        {
                            randAxe = Random.Range(1, 6 + 1);
                        }
                        break;
                    case 3:
                        Debug.Log("z");
                        Vector3 SpawnPos3 = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z + roomsDist);

                        if (CanSpawn(SpawnPos3))
                        {
                            GameObject corridorInstance2 = Instantiate(rooms[7], SpawnPos3, Quaternion.identity);
                            roomsPlaced.Add(corridorInstance2.transform.position);
                            previousRoom = corridorInstance2.transform.position;

                            Vector3 SpawnPos3b = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z + roomsDist);
                            GameObject roomInstance3 = Instantiate(rooms[ChooseRoomDir(previousRoom, SpawnPos3b)], SpawnPos3b, Quaternion.identity);
                            roomsPlaced.Add(roomInstance3.transform.position);
                            previousRoom = roomInstance3.transform.position;
                            roomsCount--;

                            if (roomsCount > 0)
                            {
                                Vector3 SpawnPos3d = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z + roomsDist);
                                GameObject corridorInstance3 = Instantiate(rooms[7], SpawnPos3d, Quaternion.identity);
                                roomsPlaced.Add(corridorInstance3.transform.position);
                                previousRoom = corridorInstance3.transform.position;

                                Vector3 SpawnPos3c = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z + roomsDist);
                                GameObject nextRoomInstance = Instantiate(rooms[ChooseRoomDir(previousRoom, SpawnPos3c)], SpawnPos3c, Quaternion.identity);
                                roomsPlaced.Add(nextRoomInstance.transform.position);
                                previousRoom = nextRoomInstance.transform.position;
                                roomsCount--;
                            }
                        }
                        else
                        {
                            randAxe = Random.Range(1, 6 + 1);
                        }
                        break;
                    case 4:
                        Debug.Log("-z");
                        Vector3 SpawnPos4 = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z - roomsDist);

                        if (CanSpawn(SpawnPos4))
                        {
                            GameObject corridorInstance2 = Instantiate(rooms[7], SpawnPos4, Quaternion.identity);
                            roomsPlaced.Add(corridorInstance2.transform.position);
                            previousRoom = corridorInstance2.transform.position;

                            Vector3 SpawnPos4b = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z - roomsDist);
                            GameObject roomInstance4 = Instantiate(rooms[ChooseRoomDir(previousRoom, SpawnPos4b)], SpawnPos4b, Quaternion.identity);
                            roomsPlaced.Add(roomInstance4.transform.position);
                            previousRoom = roomInstance4.transform.position;
                            roomsCount--;

                            if (roomsCount > 0)
                            {
                                Vector3 SpawnPos4d = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z - roomsDist);
                                GameObject corridorInstance3 = Instantiate(rooms[7], SpawnPos4d, Quaternion.identity);
                                roomsPlaced.Add(corridorInstance3.transform.position);
                                previousRoom = corridorInstance3.transform.position;

                                Vector3 SpawnPos4c = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z - roomsDist);
                                GameObject nextRoomInstance = Instantiate(rooms[ChooseRoomDir(previousRoom, SpawnPos4c)], SpawnPos4c, Quaternion.identity);
                                roomsPlaced.Add(nextRoomInstance.transform.position);
                                previousRoom = nextRoomInstance.transform.position;
                                roomsCount--;
                            }
                        }
                        else
                        {
                            randAxe = Random.Range(1, 6 + 1);
                        }
                        break;
                    case 5:
                        Debug.Log("-x");
                        Vector3 SpawnPos5 = new Vector3(previousRoom.x - roomsDist, previousRoom.y, previousRoom.z);

                        if (CanSpawn(SpawnPos5))
                        {
                            GameObject corridorInstance1 = Instantiate(rooms[6], SpawnPos5, Quaternion.identity);
                            roomsPlaced.Add(corridorInstance1.transform.position);
                            previousRoom = corridorInstance1.transform.position;

                            Vector3 SpawnPos5b = new Vector3(previousRoom.x - roomsDist, previousRoom.y, previousRoom.z);
                            GameObject roomInstance5 = Instantiate(rooms[ChooseRoomDir(previousRoom, SpawnPos5b)], SpawnPos5b, Quaternion.identity);
                            roomsPlaced.Add(roomInstance5.transform.position);
                            previousRoom = roomInstance5.transform.position;
                            roomsCount--;

                            if (roomsCount > 0)
                            {
                                Vector3 SpawnPos5d = new Vector3(previousRoom.x - roomsDist, previousRoom.y, previousRoom.z);
                                GameObject corridorInstance2 = Instantiate(rooms[6], SpawnPos5d, Quaternion.identity);
                                roomsPlaced.Add(corridorInstance2.transform.position);
                                previousRoom = corridorInstance2.transform.position;

                                Vector3 SpawnPos5c = new Vector3(previousRoom.x - roomsDist, previousRoom.y, previousRoom.z);
                                GameObject nextRoomInstance = Instantiate(rooms[ChooseRoomDir(previousRoom, SpawnPos5c)], SpawnPos5c, Quaternion.identity);
                                roomsPlaced.Add(nextRoomInstance.transform.position);
                                previousRoom = nextRoomInstance.transform.position;
                                roomsCount--;
                            }
                        }
                        else
                        {
                            randAxe = Random.Range(1, 6 + 1);
                        }
                        break;
                    case 6:
                        Debug.Log("-y");
                        Vector3 SpawnPos6 = new Vector3(previousRoom.x, previousRoom.y - roomsDist / 3f, previousRoom.z);

                        if (CanSpawn(SpawnPos6))
                        {
                            GameObject roomInstance6 = Instantiate(rooms[ChooseRoomDir(previousRoom, SpawnPos6)], SpawnPos6, Quaternion.identity);
                            roomsPlaced.Add(roomInstance6.transform.position);
                            previousRoom = roomInstance6.transform.position;
                            roomsCount--;
                        }
                        else
                        {
                            randAxe = Random.Range(1, 6 + 1);
                        }
                        break;
                }

                previousRandAxe = randAxe;
            }
        }
    }

    /// <summary>
    /// test if there is no room already placed at this position
    /// </summary>
    /// <param name="spawnPos"></param>
    /// <returns></returns>
    bool CanSpawn(Vector3 spawnPos)
    {
        foreach (Vector3 vect in roomsPlaced)
        {
            if (vect == spawnPos)
            {
                return false;
            }
        }

        return true;
    }

    int ChooseRoomDir(Vector3 prevPos, Vector3 pos)
    {
        if (prevPos.x == pos.x)
        {
            if (prevPos.y == pos.y)
            {
                if (prevPos.z == pos.z)
                {
                    return 1;
                }
                else
                {
                    if (prevPos.z > pos.z)
                    {
                        return 2;
                    }
                    else if (prevPos.z < pos.z)
                    {
                        return 3;
                    }
                }
            }
            else
            {
                if (prevPos.y > pos.y)
                {
                    return 1;
                }
                else if (prevPos.y < pos.y)
                {
                    return 5;
                }
            }
        }
        else
        {
            if (prevPos.x > pos.x)
            {
                return 0;
            }
            else if (prevPos.x < pos.x)
            {
                return 4;
            }
        }

        return 1;
    }
}
