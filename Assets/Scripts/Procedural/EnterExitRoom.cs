using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterExitRoom : MonoBehaviour
{
    [SerializeField] bool _isStart;

    public Room room;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            room.CheckForEnemies();
        }
    }
}
