using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

namespace State.AIBull
{
    public class GlobalRefBullAI : MonoBehaviour
    {
        [Header("Global References")]
        public NavMeshAgent agent;
        public Transform playerTransform;
        public LayerMask ennemiMask;
        public Material_Instances material_Instances;
        public EnemyHealth enemyHealth;
        public float distPlayer;
        public float offsetDestination;
        public StateControllerBull stateControllerBull;
        public AgentLinkMover agentLinkMover;
        public CharacterController characterController;
        public ManagerShrednoss managerShrednoss;

        //public AudioSource audioSourceBull;

        [Header("Animation")]
        public Animator myAnimator;
        public GlobalRefAnimator globalRefAnimator;
        public AnimEventRusher animEventRusher;

        [Header("Eylau Move References")]
        public bool isInEylau;
        public float slowSpeed;
        public float slowSpeedRot;
        public float slowRatio;

        [Header("SynergyAttraction References")]
        public bool isInSynergyAttraction;

        [Header("Slow Zap References")]
        public bool IsZap;

        [Header("Debug Destination")]
        public LayerMask allMask;

        [Header("Ref BaseMove State")]
        public Transform rayCheckRush;
        public GameObject refBaseMoveStateObj;

        [Header("Ref Attack State")]
        public Hitbox hitBoxRush;
        public Hitbox hitBoxAttack;
        public BoxCollider detectOtherAICollider;
        public bool launchRush;
        public Transform RayRushRight;
        public Transform RayRushMiddle;
        public Transform RayRushLeft;
        public Transform RayAttackMiddle;
        [SerializeField] GameObject refRushStateObj;
        [SerializeField] GameObject refKnockBackStateObj;
        public bool forceRush;

        [Header("Ref BaseMove State")]
        public bool ActiveAttraction;

        [Header("Ref Death State")]
        public bool isDead;

        [Foldout("Scriptable")] public BaseIdleBullSO baseIdleBullSO;
        [Foldout("Scriptable")] public BaseMoveBullParameterSO baseMoveBullSO;
        [Foldout("Scriptable")] public BaseAttackBullParameterSO baseAttackBullSO;
        [Foldout("Scriptable")] public RushBullParameterSO rushBullSO;
        [Foldout("Scriptable")] public KnockBackBullSO knockBackBullSO;
        [Foldout("Scriptable")] public DeathBullParameterSO deathBullSO;
        [Foldout("Scriptable")] public AttractionSO AttractionSO;


        [Foldout("VeryEasy")] public BaseMoveBullParameterSO baseMoveBullSO_VeryEZ;
        [Foldout("VeryEasy")] public RushBullParameterSO rushBullSO_VeryEZ;
        [Foldout("VeryEasy")] public KnockBackBullSO knockBackBullSO_VeryEZ;

        [Foldout("Medium")] public BaseMoveBullParameterSO baseMoveBullSO_Med;
        [Foldout("Medium")] public RushBullParameterSO rushBullSO_Med;
        [Foldout("Medium")] public KnockBackBullSO knockBackBullSO_Med;

        [Foldout("VeryHard")] public BaseMoveBullParameterSO baseMoveBullSO_VeryHard;
        [Foldout("VeryHard")] public RushBullParameterSO rushBullSO_VeryHard;
        [Foldout("VeryHard")] public KnockBackBullSO knockBackBullSO_VeryHard;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            playerTransform = GameObject.FindWithTag("Player").transform;
            enemyHealth = GetComponent<EnemyHealth>();
            material_Instances = GetComponent<Material_Instances>();
            agentLinkMover = GetComponent<AgentLinkMover>();

            baseIdleBullSO = Instantiate(baseIdleBullSO);
            baseAttackBullSO = Instantiate(baseAttackBullSO);
            deathBullSO = Instantiate(deathBullSO);
            AttractionSO = Instantiate(AttractionSO);

            switch (ApplyDifficulty.indexDifficulty)
            {
                case 0:
                    baseMoveBullSO = Instantiate(baseMoveBullSO_VeryEZ);
                    rushBullSO = Instantiate(rushBullSO_VeryEZ);
                    knockBackBullSO = Instantiate(knockBackBullSO_VeryEZ);
                    break;

                case 1:
                    baseMoveBullSO = Instantiate(baseMoveBullSO_Med);
                    rushBullSO = Instantiate(rushBullSO_Med);
                    knockBackBullSO = Instantiate(knockBackBullSO_Med);
                    break;

                case 2:
                    baseMoveBullSO = Instantiate(baseMoveBullSO_VeryHard);
                    rushBullSO = Instantiate(rushBullSO_VeryHard);
                    knockBackBullSO = Instantiate(knockBackBullSO_VeryHard);
                    break;
            }

        }

        private void OnEnable()
        {
            managerShrednoss.GetRef(this);
        }

        void Update()
        {
            CheckHP();
            distPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (isInEylau)
                myAnimator.speed = 1 / (slowRatio / 2);
            else
                myAnimator.speed = 1;

            if (IsZap)
                myAnimator.speed = 1 / (slowRatio / 2);
            else
                myAnimator.speed = 1;

            if (isInSynergyAttraction)
                ActiveAttractionState();
        }

        public void ActiveState(StateControllerBull.AIState newState)
        {
            stateControllerBull.SetActiveState(newState);
        }

        public void ActiveKnockBackState()
        {
            if(enemyHealth._hp >0 && !refRushStateObj.activeInHierarchy && !agent.isOnOffMeshLink)
                ActiveState(StateControllerBull.AIState.KnockBack);
        }
        public void ActiveKnockBackSynergieState()
        {
            if (enemyHealth._hp > 0 && !refKnockBackStateObj.activeInHierarchy && !agent.isOnOffMeshLink)
                ActiveState(StateControllerBull.AIState.KnockBack);
        }

        public void ActiveAttractionState()
        {
            ActiveState(StateControllerBull.AIState.Attraction);
        }

        public void CheckHP()
        {
            if (enemyHealth._hp <= 0 && !isDead)
            {
                ActiveState(StateControllerBull.AIState.Death);
                isDead = true;
            }
        }
    }
}