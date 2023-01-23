using UnityEngine;
using UnityEngine.AI;

namespace State.FlyAI
{
    public class GlobalRefFlyAI : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Transform playerTransform;

        [Header("Ref Base Move")]
        public BoxCollider myCollider;

        [Header("Ref Base Attack")]
        public GameObject colliderBaseAttack;
        public Hitbox hitbox;

        [Header("Ref Scriptable")]
        public BaseMoveFlySO baseMoveFlySO;
        public LockPlayerFlySO lockPlayerFlySO;
        public BaseAttackFlySO baseAttackFlySO;
        public DeathFlySO deathFlySO;

        // Start is called before the first frame update
        void Awake()
        {
            baseMoveFlySO = Instantiate(baseMoveFlySO);
            lockPlayerFlySO = Instantiate(lockPlayerFlySO);
            baseAttackFlySO = Instantiate(baseAttackFlySO);
            deathFlySO = Instantiate(deathFlySO);
            playerTransform = GameObject.FindWithTag("Player").transform;
        }
    }
}