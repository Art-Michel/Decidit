using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RushParameter", menuName = "Bull/RushParameter")]
public class RushBullParameterSO : ScriptableObject
{
    [Header("Duration rush")]
    public float rushCurrentDuration;
    public float rushMaxDuration;

    [Header("Speed Rotation Parameter")]
    public float speedRot;
    public float maxSpeedRot;
    public float smoothRot;

    [Header("Gravity collision")]
    public LayerMask maskCollision;
    public bool isFall;
    public bool isGround;

    [Header("Gravity value")]
    public float gravity;
    public float fallingTime;
    public float currentAcceleration;

    [Header("Movement value")]
    public Vector3 playerVelocity;
    public float speedMove;

    [Header("Movement Parameter")]
    public float stopLockDist;
    public Vector3 rushDestination;
    public float rushInertieSetDistance;
    public bool stopLockPlayer;

    [Header("Detect Trash Mob Parameter")]
    public BoxCollider detectOtherAICollider;
    public List<GameObject> ennemiInCollider = new List<GameObject>();
    public RaycastHit hit;
    public LayerMask maskCheckEnnemi;
    public LayerMask maskCheckObstacle;
    public LayerMask noMask;
}
