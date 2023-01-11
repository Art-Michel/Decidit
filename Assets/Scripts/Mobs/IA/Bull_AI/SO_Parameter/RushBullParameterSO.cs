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

    [Header("Movement Parameter")]
    public float rushSpeed;
    public float stopLockDist;
    public int damageRushAttack;
    public Vector3 rushDestination;
    public float rushInertieSetDistance;
    public bool stopLockPlayer;

    [Header("Detect Trash Mob Parameter")]
    public BoxCollider detectOtherAICollider;
    public List<GameObject> ennemiInCollider = new List<GameObject>();
    public RaycastHit hit;
    public LayerMask mask;
    public LayerMask maskCheckObstacle;


}
