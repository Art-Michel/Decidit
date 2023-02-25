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

    [HideInInspector]
    public float speedRotationAIAttack;

    [Header("Speed Rotation Attack")]
    public float maxSpeedRotationAILock;
    public float maxSpeedRotationAIAttack;
    public float smoothRotationLock;
    public float smoothRotationAttack;

    [Header("Speed Y Position Attack")]
    public float currentSpeedYAttack;
}