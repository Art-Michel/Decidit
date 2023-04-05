using UnityEngine;

namespace State.WallAI
{
    [CreateAssetMenu(fileName = "BaseAttackWallAIParameter", menuName = "WallAI/BaseAttackParameter")]
    public class BaseAttackWallAISO : ScriptableObject
    {
        [Header("*Speed AI")]
        public float stopSpeed;

        [Header("*Anticipatoin pos Player")]
        public float distAnticipGround;
        public Vector3 playerPredicDir;
        public float timePlayerGoToPredicPos;
        public float vProjectileGotToPredicPos;
        [HideInInspector] public float vPlayer;
        public float vMultiplier;

        [Header("*Attack")]
        public Rigidbody bulletPrefab;
        public float defaultForceBullet;
        public int bulletCount;
        public int maxBulletCount;
        public int currentRafaleCount;
        public int maxRafaleCount;
        public float speedSlowAnimAttack;

        [Header("*Rate Attack")]
        public float maxRateAttack;
        public float currentRateAttack;
    }
}