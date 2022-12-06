using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RushParameter", menuName = "Bull/RushParameter")]
public class RushBullParameterSO : ScriptableObject
{
    [Header("Speed Rotation Parameter")]
    [SerializeField] public float speedRot;
    [SerializeField] public float maxSpeedRot;
    [SerializeField] public float smoothRot;

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


}
