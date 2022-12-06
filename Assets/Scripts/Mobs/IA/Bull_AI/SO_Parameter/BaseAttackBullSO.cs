using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bull_BaseAttackParameter", menuName = "Bull/BaseAttackParameter")]
public class BaseAttackBullSO : ScriptableObject
{
    [Header("Speed Rotation Parameter")]
    [SerializeField] public float speedRot;
    [SerializeField] public float maxSpeedRot;
    [SerializeField] public float smoothRot;

    [Header("Attack Parameter")]
    public float attackRange;
    public float currentAttackRate;
    public float maxAttackRate;
    public int damageBaseAttack;
    public BoxCollider attackCollider;
}