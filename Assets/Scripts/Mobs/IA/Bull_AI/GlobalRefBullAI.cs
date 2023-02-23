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

        [Header("Slow Move References")]
        public bool isInEylau;
        public float slowSpeed;
        public float slowSpeedRot;
        public float slowRatio;

        [Header("Debug Destination")]
        public Transform sphereDebug;
        public LayerMask allMask;

        [Header("Ref Attack State")]
        public Hitbox hitBox;
        public BoxCollider detectOtherAICollider;
        public bool launchRush;

        [Header("Ref Death State")]
        public bool isDead;

        [Header("Scriptable")]
        public BaseIdleBullSO baseIdleBullSO;
        public BaseMoveBullParameterSO baseMoveBullSO;
        public CoolDownRushBullParameterSO coolDownRushBullSO;
        public RushBullParameterSO rushBullSO;
        public BaseAttackBullSO baseAttackBullSO;
        public DeathBullParameterSO deathBullSO;

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
            baseMoveBullSO = Instantiate(baseMoveBullSO);
            coolDownRushBullSO = Instantiate(coolDownRushBullSO);
            rushBullSO = Instantiate(rushBullSO);
            baseAttackBullSO = Instantiate(baseAttackBullSO);
            deathBullSO = Instantiate(deathBullSO);
        }

        void Update()
        {
            distPlayer = Vector3.Distance(transform.position, playerTransform.position);
        }

        public void ActiveState(StateControllerBull.AIState newState)
        {
            stateControllerBull.SetActiveState(newState);
        }

        public void ActiveKnockBackState()
        {
            ActiveState(StateControllerBull.AIState.KnockBack);
        }

        public void CheckHP()
        {
            if (enemyHealth._hp <= 0 && !isDead)
            {
                isDead = true;
                ActiveState(StateControllerBull.AIState.Death);
            }
        }
    }
}