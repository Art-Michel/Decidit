using UnityEngine;

public class CheckPlayerDownPos : MonoBehaviour
{
    public static CheckPlayerDownPos instanceCheckPlayerPos;

    [SerializeField] LayerMask mask;
    public Vector3 positionPlayer;

    private void Awake()
    {
        instanceCheckPlayerPos = this;
    }

    void FixedUpdate()
    {
        positionPlayer = RaycastAIManager.instanceRaycast.RaycastAI(transform.position, Vector3.down, mask, Color.yellow, 100).point;
    }
}
