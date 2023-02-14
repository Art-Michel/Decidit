using UnityEngine;

[CreateAssetMenu(fileName = "FlyBaseMoveParameter", menuName = "Fly/BaseMoveParameter")]
public class BaseMoveFlySO : ScriptableObject
{
    public float baseMoveSpeed;
    public LayerMask maskObstacle;
    public LayerMask maskCheckCanRush;
    public float distDetectObstace;

    [Header("Current Destination")]
    public Vector3 destinationFinal;
    public float distDestinationFinal;
    public Vector3 newPos;
    public bool newPosIsSet;

    [Header("Next Destination")]
    public Vector3 nextDestinationFinal;
    public bool nextPosIsSet;
    public float distNextDestinationFinal;

    [Header("Speed Y Position Patrol")]
    public float currentSpeedYPatrol;
  
    [Header("Speed Rotation Patrol")]
    public float maxSpeedRotationAIPatrol;
    public float speedRotationAIPatrol;
    public float smoothRotationPatrol;

    [Header("Speed Rotation Patrol")]
    public float maxSpeedRotationAIDodgeObstacle;
    public float speedRotationAIDodgeObstacle;
    public float smoothRotationDodgeObstacle;
    public float lenghtRayDetectObstacle;

    [Header("Rate Attack")]
    public float currentRateAttack;
    public Vector2 maxRateAttack;
}