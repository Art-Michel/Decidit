using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseAttackWallAIParameter", menuName = "WallAI/BaseAttackParameter")]
public class BaseAttackWallAISO : ScriptableObject
{
    [Header("*Speed AI")]
    public float stopSpeed;

    [Header("*Anticipatoin pos Player")]
    public float distAnticip;
    public Vector3 playerPredicDir;
    public float timePlayerGoToPredicPos;
    public float vProjectileGotToPredicPos;
    public float vPlayer;

    [Header("*Attack")]
    public Rigidbody bulletPrefab;
    public float defaultForceBullet;
}
