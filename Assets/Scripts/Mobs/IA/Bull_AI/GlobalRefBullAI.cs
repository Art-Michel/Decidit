using UnityEngine;
using UnityEngine.AI;

namespace State.AIBull
{
    public class GlobalRefBullAI : MonoBehaviour
    {
        [Header("Global References")]
        public NavMeshAgent agent;
        public Transform playerTransform;
        public LayerMask noMask;
        public BullAIStartPosRush bullAIStartPosRush;
        public Material_Instances material_Instances;
        public EnemyHealth enemyHealth;
        public float distPlayer;
        public float offsetDestination;

        [Header("Ref ATtck State")]
        public GameObject hitBox;
        public BoxCollider detectOtherAICollider;

        [Header("Scriptable")]
        public BaseIdleBullSO baseIdleBullSO;
        public BaseMoveBullParameterSO baseMoveBullSO;
        public CoolDownRushBullParameterSO coolDownRushBullSO;
        public RushBullParameterSO rushBullSO;
        public BaseAttackBullSO baseAttackBullSO;
        public DeathBullParameterSO deathBullSO;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            playerTransform = GameObject.FindWithTag("Player").transform;
            enemyHealth = GetComponent<EnemyHealth>();
            material_Instances = GetComponent<Material_Instances>();

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
    }
}