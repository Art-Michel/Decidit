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
        public BullCount bullCount;
        public StateControllerBull stateControllerBull;
        public AgentLinkMover agentLinkMover;
        public CharacterController characterController;
        public RushManager rushManager;
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

        [Header("Debug Destination")]
        public Transform sphereDebug;
        public LayerMask allMask;

        [Header("Ref BaseMove State")]
        public Transform rayCheckRush;

        [Header("Ref Attack State")]
        public Hitbox hitBox;
        public BoxCollider detectOtherAICollider;
        public bool launchRush;
        public Transform RayRushRight;
        public Transform RayRushMiddle;
        public Transform RayRushLeft;
        [SerializeField] GameObject refRushStateObj;

        [Header("Ref Death State")]
        public bool isDead;

        [Foldout("Scriptable")] public BaseIdleBullSO baseIdleBullSO;
        [Foldout("Scriptable")] public BaseMoveBullParameterSO baseMoveBullSO;
        [Foldout("Scriptable")] public RushBullParameterSO rushBullSO;
        [Foldout("Scriptable")] public KnockBackBullSO knockBackBullSO;
        [Foldout("Scriptable")] public DeathBullParameterSO deathBullSO;

        [Foldout("VeryEasy")] public BaseMoveBullParameterSO baseMoveBullSO_VeryEZ;
        [Foldout("VeryEasy")] public RushBullParameterSO rushBullSO_VeryEZ;
        [Foldout("VeryEasy")] public KnockBackBullSO knockBackBullSO_VeryEZ;

        [Foldout("Easy")] public BaseMoveBullParameterSO baseMoveBullSO_EZ;
        [Foldout("Easy")] public RushBullParameterSO rushBullSO_EZ;
        [Foldout("Easy")] public KnockBackBullSO knockBackBullSO_EZ;

        [Foldout("Medium")] public BaseMoveBullParameterSO baseMoveBullSO_Med;
        [Foldout("Medium")] public RushBullParameterSO rushBullSO_Med;
        [Foldout("Medium")] public KnockBackBullSO knockBackBullSO_Med;

        [Foldout("Hard")] public BaseMoveBullParameterSO baseMoveBullSO_Hard;
        [Foldout("Hard")] public RushBullParameterSO rushBullSO_Hard;
        [Foldout("Hard")] public KnockBackBullSO knockBackBullSO_Hard;

        [Foldout("VeryHard")] public BaseMoveBullParameterSO baseMoveBullSO_VeryHard;
        [Foldout("VeryHard")] public RushBullParameterSO rushBullSO_VeryHard;
        [Foldout("VeryHard")] public KnockBackBullSO knockBackBullSO_VeryHard;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            playerTransform = GameObject.FindWithTag("Player").transform;
            enemyHealth = GetComponent<EnemyHealth>();
            material_Instances = GetComponent<Material_Instances>();
            bullCount = GetComponentInParent<BullCount>();
            agentLinkMover = GetComponent<AgentLinkMover>();
            rushManager = GetComponentInParent<RushManager>();


            baseIdleBullSO = Instantiate(baseIdleBullSO);
            deathBullSO = Instantiate(deathBullSO);

            switch (ApplyDifficulty.Instance.indexDifficulty)
            {
                case 0:
                    baseMoveBullSO = Instantiate(baseMoveBullSO_VeryEZ);
                    rushBullSO = Instantiate(rushBullSO_VeryEZ);
                    knockBackBullSO = Instantiate(knockBackBullSO_VeryEZ);
                    break;

                case 1:
                    baseMoveBullSO = Instantiate(baseMoveBullSO_EZ);
                    rushBullSO = Instantiate(rushBullSO_EZ);
                    knockBackBullSO = Instantiate(knockBackBullSO_EZ);
                    break;

                case 2:
                    baseMoveBullSO = Instantiate(baseMoveBullSO_Med);
                    rushBullSO = Instantiate(rushBullSO_Med);
                    knockBackBullSO = Instantiate(knockBackBullSO_Med);
                    break;

                case 3:
                    baseMoveBullSO = Instantiate(baseMoveBullSO_Hard);
                    rushBullSO = Instantiate(rushBullSO_Hard);
                    knockBackBullSO = Instantiate(knockBackBullSO_Hard);
                    break;

                case 4:
                    baseMoveBullSO = Instantiate(baseMoveBullSO_VeryHard);
                    rushBullSO = Instantiate(rushBullSO_VeryHard);
                    knockBackBullSO = Instantiate(knockBackBullSO_VeryHard);
                    break;
            }

        }

        void Update()
        {
            CheckHP();
            distPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (isInEylau)
                myAnimator.speed = 1/ (slowRatio/2);
            else
                myAnimator.speed = 1;
        }

        public void ActiveState(StateControllerBull.AIState newState)
        {
            stateControllerBull.SetActiveState(newState);
        }

        public void ActiveKnockBackState()
        {
            if(enemyHealth._hp >0 && !refRushStateObj.activeInHierarchy)
                ActiveState(StateControllerBull.AIState.KnockBack);
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