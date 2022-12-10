using UnityEngine;

[CreateAssetMenu(fileName = "Bull_CoolDownRushParameter", menuName = "Bull/CoolDownParameter")]
public class CoolDownRushBullParameterSO : ScriptableObject
{
    [Header("Speed Rotation Parameter")]
    [SerializeField] public float speedRot;
    [SerializeField] public float maxSpeedRot;
    [SerializeField] public float smoothRot;

    [Header("Movement Parameter")]
    public float stopSpeed;
    public float speedPatrolToStartPos;
    public float speedRushToStartPos;
    public float rushInertieSetDistance;
    public Vector3 rushDestination;
    public Vector3 startPos;
    public BoxCollider boxSelected;

    [Header("Duration Stay On StartPos")]
    public float currentDurationStay;
    public float maxDurationStay;

    [Header("Cool Down Rush Parameter")]
    public float currentCoolDownRush;
    public float coolDownRush;

    [Header("Detect Wall")]
    public LayerMask mask;
    public float distDetect;
     public float currentCoolDownCheckObstacle;
    public float maxCoolDownCheckObstacle;
}