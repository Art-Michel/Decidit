using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashMob_BaseMoveParameter", menuName = "TrashMob/BaseMoveParameter")]
public class BaseMoveParameterAICAC : ScriptableObject
{
    [Header("SpeedParameter")]
    [SerializeField] public float baseSpeed;
    [SerializeField] public float runSpeed;
    [SerializeField] public float distCanRun;
    [SerializeField] public float distStopRun;
    [SerializeField] public float setSpeedSmoothRot;
    [SerializeField] public float speedSmoothRot;
    [SerializeField] public float rotSpeed;
    [SerializeField] public float attackRange;
}