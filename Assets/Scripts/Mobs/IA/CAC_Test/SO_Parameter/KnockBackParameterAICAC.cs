using UnityEngine;

[CreateAssetMenu(fileName = "TrashMob_KnockBackParameter", menuName = "TrashMob/KnockBackParameter")]
public class KnockBackParameterAICAC : ScriptableObject
{
    [Header("Gravity collision")]
    public LayerMask maskCheckObstacle;

    [Header("Gravity value")]
    public float gravity;

    [Header("Movement value")]
    public Vector3 AIVelocity;
}