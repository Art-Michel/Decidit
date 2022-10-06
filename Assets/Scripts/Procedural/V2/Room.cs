using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector3 Size => size;
    public Doors Enter => enter;
    public Doors Exit => exit;
    [SerializeField] private Vector3 size;
    [SerializeField] private Doors enter;
    [SerializeField] private Doors exit;

    public void GetDoors()
    {

    }
}
