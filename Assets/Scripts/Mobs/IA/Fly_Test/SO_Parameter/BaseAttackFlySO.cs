using UnityEngine;

[CreateAssetMenu(fileName = "FlyBaseAttackParameter", menuName = "Fly/BaseAttackParameter")]
public class BaseAttackFlySO : ScriptableObject
{
    [Header("Speed Move Attack")]
    public float baseAttackSpeed;

    [Header("Distance Destination")]
    public float distDestinationFinal;
    public float distDetectWall;
    public LayerMask wallMask;

    [Header("Speed Rotation Attack")]
    public float maxSpeedRotationAIAttack;
    public float speedRotationAIAttack;
    public float smoothRotationAttack;

    [Header("Speed Y Position Attack")]
    public float currentSpeedYAttack;
}