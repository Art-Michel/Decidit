using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using State.WallAI;


namespace State.FlyAI
{
    public class GlobalRefFlyAI : MonoBehaviour
    {
        [Header("Global Ref")]
        public NavMeshAgent agent;
        public Transform playerTransform;
        public StateControllerFlyAI stateControllerFlyAI;
        public EnemyHealth enemyHealth;
        public FlyMobAttackManager flyMobAttackManager;
        public GameObject vfxShowDoor;

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

        [Header("SynergyAttraction References")]
        public bool isInSynergyAttraction;

        [Header("Slow Zap References")]
        public bool IsZap;

        [Header("Ref Base Move")]
        public BoxCollider myCollider;

        [Header("Ref Base Attack")]
        public GameObject colliderBaseAttack;
        public Hitbox hitbox;
        public Transform spawnBullet;
        public bool attackRange;
        public bool SpreadShot;

        [Header("Ref Base Death")]
        [SerializeField] bool isDead;

        [Foldout("Scriptable")] public BaseMoveFlySO baseMoveFlySO;
        [Foldout("Scriptable")] public LockPlayerFlySO lockPlayerFlySO;
        [Foldout("Scriptable")] public BaseAttackFlySO baseAttackFlySO;
        [Foldout("Scriptable")] public BaseAttackWallAISO baseAttackWallAISO;
        [Foldout("Scriptable")] public DeathFlySO deathFlySO;
        [Foldout("Scriptable")] public KnockBackFlySO KnockBackFlySO;
        [Foldout("Scriptable")] public AttractionSO AttractionSO;

        [Foldout("VeryEasy")] public BaseMoveFlySO baseMoveFlySO_VeryEZ;
        [Foldout("VeryEasy")] public LockPlayerFlySO lockPlayerFlySO_VeryEZ;
        [Foldout("VeryEasy")] public BaseAttackFlySO baseAttackFlySO_VeryEZ;
        [Foldout("VeryEasy")] public BaseAttackWallAISO baseAttackWallAISO_VeryEZ;

        [Foldout("Medium")] public BaseMoveFlySO baseMoveFlySO_Medium;
        [Foldout("Medium")] public LockPlayerFlySO lockPlayerFlySO_Medium;
        [Foldout("Medium")] public BaseAttackFlySO baseAttackFlySO_Medium;
        [Foldout("Medium")] public BaseAttackWallAISO baseAttackWallAISO_Med;

        [Foldout("VeryHard")] public BaseMoveFlySO baseMoveFlySO_VeryHard;
        [Foldout("VeryHard")] public LockPlayerFlySO lockPlayerFlySO_VeryHard;
        [Foldout("VeryHard")] public BaseAttackFlySO baseAttackFlySO_VeryHard;
        [Foldout("VeryHard")] public BaseAttackWallAISO baseAttackWallAISO_VeryHard;

        // Start is called before the first frame update
        void Awake()
        {
            deathFlySO = Instantiate(deathFlySO);
            KnockBackFlySO = Instantiate(KnockBackFlySO);
            AttractionSO = Instantiate(AttractionSO);
            flyMobAttackManager = transform.GetComponentInParent<FlyMobAttackManager>();

            switch (ApplyDifficulty.indexDifficulty)
            {
                case 0:
                    baseMoveFlySO = Instantiate(baseMoveFlySO_VeryEZ);
                    lockPlayerFlySO = Instantiate(lockPlayerFlySO_VeryEZ);
                    baseAttackFlySO = Instantiate(baseAttackFlySO_VeryEZ);
                    baseAttackWallAISO = Instantiate(baseAttackWallAISO_VeryEZ);
                    break;

                case 1:
                    baseMoveFlySO = Instantiate(baseMoveFlySO_Medium);
                    lockPlayerFlySO = Instantiate(lockPlayerFlySO_Medium);
                    baseAttackFlySO = Instantiate(baseAttackFlySO_Medium);
                    baseAttackWallAISO = Instantiate(baseAttackWallAISO_Med);
                    break;

                case 2:
                    baseMoveFlySO = Instantiate(baseMoveFlySO_VeryHard);
                    lockPlayerFlySO = Instantiate(lockPlayerFlySO_VeryHard);
                    baseAttackFlySO = Instantiate(baseAttackFlySO_VeryHard);
                    baseAttackWallAISO = Instantiate(baseAttackWallAISO_VeryHard);
                    break;
            }

            playerTransform = GameObject.FindWithTag("Player").transform;
        }

        private void OnEnable()
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        private void Update()
        {
            CheckHP();

            if (isInSynergyAttraction && stateControllerFlyAI != null)
                ActiveAttractionState();
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
            if (stateControllerFlyAI.currentState == StateControllerFlyAI.AIState.BaseMove && enemyHealth._hp > 0)
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

        public void ActiveAttractionState()
        {
            ActiveState(StateControllerFlyAI.AIState.Attraction);
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