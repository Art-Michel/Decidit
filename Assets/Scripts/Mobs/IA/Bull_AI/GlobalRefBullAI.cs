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
        public BullAIStartPosRush bullAIStartPosRush;
        public Material_Instances material_Instances;
        public EnemyHealth enemyHealth;
        public float distPlayer;
        public float offsetDestination;
        public BullCount bullCount;
        public StateControllerBull stateControllerBull;
        public AgentLinkMover agentLinkMover;

        [Header("Debug Destination")]
        public Transform sphereDebug;

        [Header("Ref ATtck State")]
        public Hitbox hitBox;
        public BoxCollider detectOtherAICollider;

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
            bullAIStartPosRush = GetComponentInParent<BullAIStartPosRush>();
            bullCount = GetComponentInParent<BullCount>();
            agentLinkMover = GetComponent<AgentLinkMover>();

            baseIdleBullSO = Instantiate(baseIdleBullSO);
            baseMoveBullSO = Instantiate(baseMoveBullSO);
            coolDownRushBullSO = Instantiate(coolDownRushBullSO);
            rushBullSO = Instantiate(rushBullSO);
            baseAttackBullSO = Instantiate(baseAttackBullSO);
            deathBullSO = Instantiate(deathBullSO);
        }

        void Update()
        {
            sphereDebug.position = agent.destination;

            distPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (enemyHealth._hp <= 0 && !isDead)
            {
                isDead = true;
                ActiveState(StateControllerBull.AIState.Death);
                //myAnimator.SetBool("Death", true);
            }
        }

        public void ActiveState(StateControllerBull.AIState newState)
        {
            stateControllerBull.SetActiveState(newState);
        }
    }
}