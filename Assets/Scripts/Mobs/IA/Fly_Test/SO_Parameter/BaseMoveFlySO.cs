using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlyBaseMoveParameter", menuName = "Fly/BaseMoveParameter")]
public class BaseMoveFlySO : ScriptableObject
{
    public float baseMoveSpeed;
    public Vector3 destinationFinal;
    public float distDestinationFinal;
    public Vector3 newPos;
    public bool newPosIsSet;
    public LayerMask maskObstacle;
    public float distDetectObstace;

    [Header("Speed Y Position Patrol")]
    public float timeGoToDestinationPatrol;
    public float maxSpeedYTranslationPatrol;
    public float currentSpeedYPatrol;
    public float ySpeedSmootherPatrol;
    public float lerpSpeedYValuePatrol;

    [Header("Speed Rotation Patrol")]
    public float maxSpeedRotationAIPatrol;
    public float speedRotationAIPatrol;
    public float smoothRotationPatrol;

    [Header("Rate Attack")]
    public float currentRateAttack;
    public float maxRateAttack;
}