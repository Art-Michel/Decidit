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
        public float distAnticipUp;
        public Vector3 playerPredicDir;
        public float timePlayerGoToPredicPos;
        public float vProjectileGotToPredicPos;
        public float vPlayer;

        [Header("*Attack")]
        public Rigidbody bulletPrefab;
        public float defaultForceBullet;
        public int bulletCount;
        public int maxBulletCount;
    }
}