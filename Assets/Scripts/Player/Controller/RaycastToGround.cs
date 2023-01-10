using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastToGround : MonoBehaviour
{
    [SerializeField] LayerMask mask;

    public static Vector3 positionPlayer;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        positionPlayer = RaycastAIManager.RaycastAI(transform.position, Vector3.down, mask, Color.yellow, 100).point;
    }
}
