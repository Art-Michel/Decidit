using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashMob_BaseAttackParameter", menuName = "TrashMob/BaseAttackParameter")]
public class BaseAttackParameterAICAC : ScriptableObject
{
    [Header("Attack Parameter")]
    [SerializeField] public float attackRange;
    [SerializeField] public float currentAttackRate;
    [SerializeField] public float maxAttackRate;
    [SerializeField] public int damageBaseAttack;
    [SerializeField] public bool isAttacking = false;
}