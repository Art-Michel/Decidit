using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bull_BaseAttackParameter", menuName = "Bull/BaseAttackParameter")]
public class BaseAttackBullSO : ScriptableObject
{
    public float attackRange;
    public float currentAttackRate;
    public float maxAttackRate;
    public int damageBaseAttack;
    public BoxCollider attackCollider;
}