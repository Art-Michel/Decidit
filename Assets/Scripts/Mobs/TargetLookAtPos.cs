using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLookAtPos : MonoBehaviour
{
    Transform playerPos;
    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerPos.position;
    }
}
