using UnityEngine;

[CreateAssetMenu(fileName = "TrashMob_BaseMoveParameter", menuName = "TrashMob/BaseMoveParameter")]
public class BaseMoveParameterAICAC : ScriptableObject
{
    [Header("Speed Rotation Parameter")]
    public float speedRot;
    public float maxSpeedRot;
    public float smoothRot;

    [Header("Take Back")]
    public float lenghtBack;

    [Header("Trigger Attack")]
    public float attackRange;
    public Vector2 maxCoolDownAttack;
    public float currentCoolDownAttack;

    [Header("Jump")]
    public float delayBeforeJump;
    public float maxDelayBeforeJump;

    [Header("Speed Movement Parameter")]
    public float baseSpeed;
    public float smoothSpeedbase;
    public float runSpeed;
    public float smoothSpeedRun;
    public float anticipSpeed;
    public float smoothSpeedAnticip;
    public float distCanRun;
    public float distStopRun;
    public bool activeAnticipDestination;
    public float jumpRate;
    public float offsetTransitionSmooth;

    [Header("Trigger Surround")]
    public float distStopSurround; // IA proche du joueur
}