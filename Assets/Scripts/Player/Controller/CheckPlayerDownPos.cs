using UnityEngine;

public class CheckPlayerDownPos : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    public static Vector3 positionPlayer;

    void Start()
    {

    }

    void FixedUpdate()
    {
        positionPlayer = RaycastAIManager.RaycastAI(transform.position, Vector3.down, mask, Color.yellow, 100).point;
    }
}
