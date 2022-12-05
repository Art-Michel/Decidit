using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerYRotation : MonoBehaviour
{
    [SerializeField] Transform playerT;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, playerT.eulerAngles.y, 0);
    }
}
