using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> rooms = new List<GameObject>();
    [SerializeField] float roomsCount;
    [SerializeField] float roomsDist;
    private Vector3 previousRoom;

    private void Awake()
    {
        previousRoom = transform.position;

        while (roomsCount > 0)
        {
            int randRoom = Random.Range(0, rooms.Count);

            int randAxe = Random.Range(1, 3 + 1);

            switch (randAxe)
            {
                case 1:
                    Debug.Log("x");
                    GameObject roomInstance1 = Instantiate(rooms[randRoom], new Vector3(previousRoom.x + roomsDist, previousRoom.y, previousRoom.z), Quaternion.identity);
                    previousRoom = roomInstance1.transform.position;
                    break;
                case 2:
                    Debug.Log("y");
                    GameObject roomInstance2 = Instantiate(rooms[randRoom], new Vector3(previousRoom.x, previousRoom.y + roomsDist / 3f, previousRoom.z), Quaternion.identity);
                    previousRoom = roomInstance2.transform.position;
                    break;
                case 3:
                    Debug.Log("z");
                    GameObject roomInstance3 = Instantiate(rooms[randRoom], new Vector3(previousRoom.x, previousRoom.y, previousRoom.z + roomsDist), Quaternion.identity);
                    previousRoom = roomInstance3.transform.position;
                    break;
            }

            roomsCount--;
        }
    }
}
