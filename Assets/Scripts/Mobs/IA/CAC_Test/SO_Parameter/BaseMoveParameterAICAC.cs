using UnityEngine;

[CreateAssetMenu(fileName = "TrashMob_BaseMoveParameter", menuName = "TrashMob/BaseMoveParameter")]
public class BaseMoveParameterAICAC : ScriptableObject
{
    [Header("Speed Rotation Parameter")]
    [SerializeField] public float speedRot;
    [SerializeField] public float maxSpeedRot;
    [SerializeField] public float smoothRot;

    [Header("Speed Movement Parameter")]
    [SerializeField] public float baseSpeed;
    [SerializeField] public float smoothSpeedbase;
    [SerializeField] public float runSpeed;
    [SerializeField] public float smoothSpeedRun;
    [SerializeField] public float anticipSpeed;
    [SerializeField] public float smoothSpeedAnticip;
    [SerializeField] public float distCanRun;
    [SerializeField] public float distStopRun;
    [SerializeField] public float attackRange;
    [SerializeField] public bool activeAnticipDestination;
}