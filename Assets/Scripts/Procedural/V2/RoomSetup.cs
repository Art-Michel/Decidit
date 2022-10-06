using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomSetup", menuName = "Decidit/Room Setup")]
public class RoomSetup : ScriptableObject
{
    public string title;
    public Color color;
    public List<Room> rooms = new List<Room>();

    public Room Get()
    {
        return rooms[Random.Range(0, rooms.Count)];
    }
}
