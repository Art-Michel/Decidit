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


        // Start is called before the first frame update
        void Awake()
        {
            baseMoveFlySO = Instantiate(baseMoveFlySO);
            lockPlayerFlySO = Instantiate(lockPlayerFlySO);
            baseAttackFlySO = Instantiate(baseAttackFlySO);
            deathFlySO = Instantiate(deathFlySO);
            playerTransform = GameObject.FindWithTag("Player").transform;
        }

        private void Start()
        {

            transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        public void ActiveState(StateControllerFlyAI.AIState newState)
        {
            Debug.Log(stateControllerFlyAI);
            stateControllerFlyAI.SetActiveState(newState);
        }

        public void ActiveKnockBackState()
        {
            ActiveState(StateControllerFlyAI.AIState.KnockBack);
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