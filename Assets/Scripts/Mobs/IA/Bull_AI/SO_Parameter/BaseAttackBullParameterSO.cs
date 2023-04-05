using UnityEngine;

[CreateAssetMenu(fileName = "Bull_BaseAttackParameter", menuName = "Bull/BaseAttackParameter")]
public class BaseAttackBullParameterSO : ScriptableObject
{
    [Header("Delay")]
    public float maxDelayBeforeAttack;
    public float curentDelayBeforeAttack;

    [Header("SpeedMove")]
    public float speed;

    [Header("SpeedRotation")]
    public float speedRot;
    public float maxSpeedRot;
    public float smoothRot;

    [Header("DistLaunchAttack")]
    public float distLaunchAttack;
    public float distLaunchAttackState;
}