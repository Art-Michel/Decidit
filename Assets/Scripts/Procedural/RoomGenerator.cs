using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> corridors = new List<GameObject>();
    [SerializeField] List<GameObject> rooms = new List<GameObject>();
    [SerializeField] List<GameObject> roomsOrder = new List<GameObject>();
    List<Vector3> roomsPlaced = new List<Vector3>();
    [SerializeField] int roomsCount;
    [SerializeField] float roomDist;
    [Header("param")]
    [SerializeField] float corridorMaxSize = 1;
    private Vector3 previousRoom;
    private float previousRandAxe;
    private int roomsCountMax;
    private Quaternion baseRot;
    bool isFinish = false;

    private void Awake()
    {
        roomsCountMax = roomsCount;

        Generate();
    }

    /// <summary>
    /// generate rooms & collidors
    /// </summary>
    void Generate()
    {
        previousRoom = transform.position;
        baseRot = transform.rotation;

        while (roomsCount > 0)
        {
            int randAxe = Random.Range(1, 6 + 1);

            if (randAxe != previousRandAxe || roomsCount == roomsCountMax)
            {
                switch (randAxe)
                {
                    case 1:
                        //Debug.Log("x");
                        int randomInt = (int)Random.Range(0, rooms.Count);
                        Vector3 SpawnPos1 = new Vector3(previousRoom.x + roomDist, previousRoom.y, previousRoom.z);
                        Vector3 NextSpawn1 = new Vector3(previousRoom.x + roomDist * 2, previousRoom.y, previousRoom.z);

                        if (CanSpawn(SpawnPos1) && CanSpawn(NextSpawn1))
                        {
                            //if (roomsCount < roomsCountMax)
                            //{
                            //    GameObject corridorInstance1 = Instantiate(corridors[0], SpawnPos1, Quaternion.identity);
                            //    roomsPlaced.Add(corridorInstance1.transform.position);
                            //    previousRoom = corridorInstance1.transform.position;
                            //}

                            Vector3 SpawnPos1b = new Vector3(previousRoom.x + roomDist, previousRoom.y, previousRoom.z);
                            GameObject roomInstance1 = Instantiate(rooms[randomInt], SpawnPos1b, Quaternion.identity);
                            roomsOrder.Add(roomInstance1);
                            roomsPlaced.Add(roomInstance1.transform.position);
                            previousRoom = roomInstance1.transform.position;
                            roomsCount--;
                        }
                        else
                        {
                            randAxe = Random.Range(1, 6 + 1);
                        }
                        break;
                    case 2:
                        //Debug.Log("y");
                        int randomInt2 = (int)Random.Range(0, rooms.Count);
                        Vector3 SpawnPos2 = new Vector3(previousRoom.x, previousRoom.y + roomDist / 3f, previousRoom.z);

                        if (CanSpawn(SpawnPos2))
                        {
                            GameObject roomInstance2 = Instantiate(rooms[randomInt2], SpawnPos2, Quaternion.identity);
                            roomsOrder.Add(roomInstance2);
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
                        //Debug.Log("z");
                        int randomInt3 = (int)Random.Range(0, rooms.Count);
                        Vector3 SpawnPos3 = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z + roomDist);
                        Vector3 NextSpawn3 = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z + roomDist * 2);

                        if (CanSpawn(SpawnPos3) && CanSpawn(NextSpawn3))
                        {
                            //if (roomsCount < roomsCountMax)
                            //{
                            //    GameObject corridorInstance2 = Instantiate(corridors[1], SpawnPos3, Quaternion.identity);
                            //    roomsPlaced.Add(corridorInstance2.transform.position);
                            //    previousRoom = corridorInstance2.transform.position;
                            //}

                            Vector3 SpawnPos3b = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z + roomDist);
                            GameObject roomInstance3 = Instantiate(rooms[randomInt3], SpawnPos3b, Quaternion.identity);
                            roomsOrder.Add(roomInstance3);
                            roomsPlaced.Add(roomInstance3.transform.position);
                            previousRoom = roomInstance3.transform.position;
                            roomsCount--;
                        }
                        else
                        {
                            randAxe = Random.Range(1, 6 + 1);
                        }
                        break;
                    case 4:
                        //Debug.Log("-z");
                        int randomInt4 = (int)Random.Range(0, rooms.Count);
                        Vector3 SpawnPos4 = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z - roomDist);
                        Vector3 NextSpawn4 = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z - roomDist * 2);

                        if (CanSpawn(SpawnPos4) && CanSpawn(NextSpawn4))
                        {
                            //if (roomsCount < roomsCountMax)
                            //{
                            //    GameObject corridorInstance2 = Instantiate(corridors[1], SpawnPos4, Quaternion.identity);
                            //    roomsPlaced.Add(corridorInstance2.transform.position);
                            //    previousRoom = corridorInstance2.transform.position;
                            //}

                            Vector3 SpawnPos4b = new Vector3(previousRoom.x, previousRoom.y, previousRoom.z - roomDist);
                            GameObject roomInstance4 = Instantiate(rooms[randomInt4], SpawnPos4b, Quaternion.identity);
                            roomsOrder.Add(roomInstance4);
                            roomsPlaced.Add(roomInstance4.transform.position);
                            previousRoom = roomInstance4.transform.position;
                            roomsCount--;
                        }
                        else
                        {
                            randAxe = Random.Range(1, 6 + 1);
                        }
                        break;
                    case 5:
                        //Debug.Log("-x");
                        int randomInt5 = (int)Random.Range(0, rooms.Count);
                        Vector3 SpawnPos5 = new Vector3(previousRoom.x - roomDist, previousRoom.y, previousRoom.z);
                        Vector3 NextSpawn5 = new Vector3(previousRoom.x - roomDist * 2, previousRoom.y, previousRoom.z);

                        if (CanSpawn(SpawnPos5) && CanSpawn(NextSpawn5))
                        {
                            //if (roomsCount < roomsCountMax)
                            //{
                            //    GameObject corridorInstance1 = Instantiate(corridors[0], SpawnPos5, Quaternion.identity);
                            //    roomsPlaced.Add(corridorInstance1.transform.position);
                            //    previousRoom = corridorInstance1.transform.position;
                            //}

                            Vector3 SpawnPos5b = new Vector3(previousRoom.x - roomDist, previousRoom.y, previousRoom.z);
                            GameObject roomInstance5 = Instantiate(rooms[randomInt5], SpawnPos5b, Quaternion.identity);
                            roomsOrder.Add(roomInstance5);
                            roomsPlaced.Add(roomInstance5.transform.position);
                            previousRoom = roomInstance5.transform.position;
                            roomsCount--;
                        }
                        else
                        {
                            randAxe = Random.Range(1, 6 + 1);
                        }
                        break;
                    case 6:
                        //Debug.Log("-y");
                        int randomInt6 = (int)Random.Range(0, rooms.Count);
                        Vector3 SpawnPos6 = new Vector3(previousRoom.x, previousRoom.y - roomDist / 3f, previousRoom.z);

                        if (CanSpawn(SpawnPos6))
                        {
                            GameObject roomInstance6 = Instantiate(rooms[randomInt6], SpawnPos6, Quaternion.identity);
                            roomsOrder.Add(roomInstance6);
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

    private void Update()
    {
        if (!isFinish)
        {
            for (int i = 0; i < roomsOrder.Count; i++)
            {
                if (roomsOrder.Count > i + 1)
                {
                    if (roomsOrder[i].transform.position.y > roomsOrder[i + 1].transform.position.y)
                    {
                        roomsOrder[i + 1].gameObject.GetComponentInChildren<GroundStates>().SpawnStairs();
                        roomsOrder[i].gameObject.GetComponentInChildren<GroundStates>().UnlockStairs();
                    }
                    else if (roomsOrder[i].transform.position.y < roomsOrder[i + 1].transform.position.y)
                    {
                        roomsOrder[i].gameObject.GetComponentInChildren<GroundStates>().SpawnStairs();
                        roomsOrder[i + 1].gameObject.GetComponentInChildren<GroundStates>().UnlockStairs();
                    }

                    if (i > corridors.Count)
                    {
                        isFinish = true;
                    }
                }
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
        if (Physics.CheckSphere(spawnPos, 2))
        {
            return false;
        }

        return true;
    }
}
