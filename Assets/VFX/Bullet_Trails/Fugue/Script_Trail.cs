using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Trail : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.forward * 0.2f;
    }
}
