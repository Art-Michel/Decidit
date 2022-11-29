using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlyBaseAttackParameter", menuName = "Fly/BaseAttackParameter")]
public class BaseAttackFlySO : ScriptableObject
{
    [Header("Speed Move Attack")]
    public float baseAttackSpeed;

    [Header("Distance Destination")]
    public float distDestinationFinal;


    [Header("Speed Rotation Attack")]
    public float maxSpeedRotationAIAttack;
    public float speedRotationAIAttack;
    public float smoothRotationAttack;

    [Header("Speed Y Position Attack")]
    public float timeGoToDestinationAttack;
    public float maxSpeedYTranslationAttack;
    public float currentSpeedYAttack;
    public float ySpeedSmootherAttack;
    public float lerpSpeedYValueAttack;
}