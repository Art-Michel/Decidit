using UnityEngine;

[CreateAssetMenu(fileName = "KnockBackFlySO", menuName = "Fly/KnockBackFlySO")]
public class KnockBackFlySO : ScriptableObject
{
    [Header("Gravity collision")]
    public LayerMask maskCheckObstacle;

    [Header("Movement value")]
    public Vector3 AIVelocity;
}