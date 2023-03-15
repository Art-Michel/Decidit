using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

namespace State.FlyAI
{
    public class GlobalRefFlyAI : MonoBehaviour
    {
        [Header("Global Ref")]
        public NavMeshAgent agent;
        public Transform playerTransform;
        public StateControllerFlyAI stateControllerFlyAI;
        public EnemyHealth enemyHealth;
        //public AudioSource audioSourceFly;

        [Header("Animation")]
        public Animator myAnimator;
        public GlobalRefAnimator globalRefAnimator;

        [Header("Ref KnockBack")]
        public CharacterController characterController;

        [Header("Slow Move References")]
        public bool isInEylau;
        public float slowSpeed;
        public float slowSpeedRot;
        public float slowRatio;

        [Header("Ref Base Move")]
        public BoxCollider myCollider;

        [Header("Ref Base Attack")]
        public GameObject colliderBaseAttack;
        public Hitbox hitbox;

        [Header("Ref Base Death")]
        [SerializeField] bool isDead;

        [Header("Ref Scriptable")]
        public BaseMoveFlySO baseMoveFlySO;
        public LockPlayerFlySO lockPlayerFlySO;
        public BaseAttackFlySO baseAttackFlySO;
        public DeathFlySO deathFlySO;
        public KnockBackFlySO KnockBackFlySO;

        [Foldout("VeryEasy")] public BaseMoveFlySO baseMoveFlySO_VeryEZ;
        [Foldout("VeryEasy")] public LockPlayerFlySO lockPlayerFlySO_VeryEZ;
        [Foldout("VeryEasy")] public BaseAttackFlySO baseAttackFlySO_VeryEZ;

        [Foldout("Easy")] public BaseMoveFlySO baseMoveFlySO_EZ;
        [Foldout("Easy")] public LockPlayerFlySO lockPlayerFlySO_EZ;
        [Foldout("Easy")] public BaseAttackFlySO baseAttackFlySO_EZ;

        [Foldout("Medium")] public BaseMoveFlySO baseMoveFlySO_Medium;
        [Foldout("Medium")] public LockPlayerFlySO lockPlayerFlySO_Medium;
        [Foldout("Medium")] public BaseAttackFlySO baseAttackFlySO_Medium;

        [Foldout("Hard")] public BaseMoveFlySO baseMoveFlySO_Hard;
        [Foldout("Hard")] public LockPlayerFlySO lockPlayerFlySO_Hard;
        [Foldout("Hard")] public BaseAttackFlySO baseAttackFlySO_Hard;

        [Foldout("VeryHard")] public BaseMoveFlySO baseMoveFlySO_VeryHard;
        [Foldout("VeryHard")] public LockPlayerFlySO lockPlayerFlySO_VeryHard;
        [Foldout("VeryHard")] public BaseAttackFlySO baseAttackFlySO_VeryHard;

        // Start is called before the first frame update
        void Awake()
        {
            deathFlySO = Instantiate(deathFlySO);
            KnockBackFlySO = Instantiate(KnockBackFlySO);

            switch (ApplyDifficulty.instance.indexDifficulty)
            {
                case 0:
                    baseMoveFlySO = Instantiate(baseMoveFlySO_VeryEZ);
                    lockPlayerFlySO = Instantiate(lockPlayerFlySO_VeryEZ);
                    baseAttackFlySO = Instantiate(baseAttackFlySO_VeryEZ);
                    break;

                case 1:
                    baseMoveFlySO = Instantiate(baseMoveFlySO_EZ);
                    lockPlayerFlySO = Instantiate(lockPlayerFlySO_EZ);
                    baseAttackFlySO = Instantiate(baseAttackFlySO_EZ);
                    break;

                case 2:
                    baseMoveFlySO = Instantiate(baseMoveFlySO_Medium);
                    lockPlayerFlySO = Instantiate(lockPlayerFlySO_Medium);
                    baseAttackFlySO = Instantiate(baseAttackFlySO_Medium);
                    break;

                case 3:
                    baseMoveFlySO = Instantiate(baseMoveFlySO_Hard);
                    lockPlayerFlySO = Instantiate(lockPlayerFlySO_Hard);
                    baseAttackFlySO = Instantiate(baseAttackFlySO_Hard);
                    break;

                case 4:
                    baseMoveFlySO = Instantiate(baseMoveFlySO_VeryHard);
                    lockPlayerFlySO = Instantiate(lockPlayerFlySO_VeryHard);
                    baseAttackFlySO = Instantiate(baseAttackFlySO_VeryHard);
                    break;
            }

            playerTransform = GameObject.FindWithTag("Player").transform;
        }

        private void Start()
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        private void Update()
        {
            CheckHP();
        }

        public void ActiveState(StateControllerFlyAI.AIState newState)
        {
            stateControllerFlyAI.SetActiveState(newState);
        }

        public void ActiveKnockBackState()
        {
            if(enemyHealth._hp >0)
                ActiveState(StateControllerFlyAI.AIState.KnockBack);
        }

        public void LaunchAttack()
        {
            if (StateControllerFlyAI.currentState == StateControllerFlyAI.AIState.BaseMove && enemyHealth._hp > 0)
            {
                if (enemyHealth._hp > 0)
                    ActiveState(StateControllerFlyAI.AIState.LockPlayer);
                else
                {
                    ActiveState(StateControllerFlyAI.AIState.Death);
                    isDead = true;
                }
            }
        }

        public void CheckHP()
        {
            if (enemyHealth._hp <= 0 && !isDead)
            {
                ActiveState(StateControllerFlyAI.AIState.Death);
                isDead = true;
            }
        }
    }
}