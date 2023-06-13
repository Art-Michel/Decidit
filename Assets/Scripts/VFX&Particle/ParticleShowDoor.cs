using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class ParticleShowDoor : MonoBehaviour
{
    [SerializeField] Transform endDoor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Button]
    void GetDoorRef()
    {
        endDoor = GameObject.Find("ExitDoor").transform;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
