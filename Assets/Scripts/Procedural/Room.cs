using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector3 Size => size;
    public GameObject Enter => enter;
    public GameObject Exit => exit;
    [SerializeField] private Vector3 size;
    [SerializeField] private GameObject enter;
    [SerializeField] private GameObject exit;

    public void GetDoors()
    {

    }
}
