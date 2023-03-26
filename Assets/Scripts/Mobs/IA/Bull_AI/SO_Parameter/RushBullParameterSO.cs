using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RushParameter", menuName = "Bull/RushParameter")]
public class RushBullParameterSO : ScriptableObject
{
    [Header("Rate Rush")]
    public Vector2 rangeTimerRush;

    [Header("Look At")]
    public Vector3 directionLookAt;
    public Vector3 relativePos;

    [Header("Speed Rotation Rush Parameter")]
    public float speedRotRush;
    public float maxSpeedRotRush;
    public float smoothRotRush;

    [Header("Speed Rotation Lock Parameter")]
    public float speedRotLock;
    public float maxSpeedRotLock;
    public float smoothRotLock;

    [Header("Gravity collision")]
    public LayerMask maskCollision;
    public bool isFall;
    public bool isGround;

    [Header("Gravity value")]
    public float gravity;
    public float fallingTime;
    public float currentAcceleration;

    [Header("Movement value")]
    public Vector3 AIVelocity;
    public float speedMove;

    [Header("Movement Parameter")]
    public Vector3 rushDestination;
    public bool stopLockPlayer;
    public float stopLockDist;
    public float rushDistance;
    public float rushInertieSetDistance;

    [Header("Rush Movement")]
    [HideInInspector]
    public Vector3 move;
    [HideInInspector]
    public Vector3 directionYSlope;
    [HideInInspector]
    public Vector2 targetPos;
    [HideInInspector]
    public Vector2 direction;
    [HideInInspector]
    public float distRush;
    [HideInInspector]
    public float effectiveGravity;

    [Header("Raycast")]
    public RaycastHit hitGround;
    public RaycastHit hitAICAC;
    public RaycastHit hitObstacle;

    [Header("Detect Trash Mob Parameter")]
    public List<GameObject> ennemiInCollider = new List<GameObject>();
    public RaycastHit hit;
    public LayerMask maskCheckEnnemi;
    public LayerMask maskCheckObstacle;
    public LayerMask maskCheckPlayer;
    public LayerMask noMask;
    public LayerMask maskCheckCanRush;
}
