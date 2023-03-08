using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace State.AICAC
{
    public class GlobalRefAICAC : _StateAICAC
    {
        [Header("Global Ref")]
        public Transform playerTransform;
        public Transform spawnSurroundDodge;
        public NavMeshAgent agent;
        public AICACVarianteState aICACVarianteState;
        public EnemyHealth enemyHealth;
        public Material_Instances material_Instances;
        public float distPlayer;
        [SerializeField] StateControllerAICAC stateControllerTrashMob;
        //public AudioSource audioSourceTrashMob;

        [Header("Animation")]
        public Animator myAnimator;
        public GlobalRefAnimator globalRefAnimator;
        public AnimEventAICAC animEventAICAC;

        [Header("Slow Move References")]
        public bool isInEylau;
        public float slowSpeedMove;
        public float slowSpeedRot;
        public float slowRatio;

        [Header("Ref Move State")]
        public float offsetDestination;
        public Vector3 debugDestination;
        public AgentLinkMover agentLinkMover;
        public Vector3 destinationSurround;
        public SurroundManager surroundManager;

        [Header("Ref KnockBack")]
        public CharacterController characterController;

        [Header("Ref Dodge State")]
        public Transform spawnRayDodge;

        [Header("Ref Attack State")]
        public GameObject hitBox;

        [Header("Ref Death State")]
        public bool isDead;

        [Header("Ref Surround State")]
        public List<GameObject> listOtherCACAI = new List<GameObject>();
        public List<Transform> listOtherAIContact = new List<Transform>();

        [Header("Scriptable")]
        public BaseMoveParameterAICAC baseMoveAICACSO;
        public BaseAttackParameterAICAC baseAttackAICACSO;
        public BaseIdleParameterAICAC baseIdleAICACSO;
        public DeathParameterAICAC deathAICACSO;
        public DodgeParameterAICAC dodgeAICACSO;
        public SurroundParameterAICAC surroundAICACSO;
        public KnockBackParameterAICAC knockBackAICACSO;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            playerTransform = GameObject.FindWithTag("Player").transform.GetChild(0).transform;
            myAnimator = GetComponent<Animator>();
            spawnSurroundDodge = transform.Find("SpawnSurroundRay");
            aICACVarianteState = transform.parent.GetComponent<AICACVarianteState>();
            enemyHealth = GetComponent<EnemyHealth>();
            material_Instances = GetComponent<Material_Instances>();
            agentLinkMover = GetComponent<AgentLinkMover>();
            surroundManager = GetComponentInParent<SurroundManager>();

            baseMoveAICACSO = Instantiate(baseMoveAICACSO);
            baseAttackAICACSO = Instantiate(baseAttackAICACSO);
            baseIdleAICACSO = Instantiate(baseIdleAICACSO);
            deathAICACSO = Instantiate(deathAICACSO);
            dodgeAICACSO = Instantiate(dodgeAICACSO);
            surroundAICACSO = Instantiate(surroundAICACSO);
            knockBackAICACSO = Instantiate(knockBackAICACSO);
        }

        private void Update()
        {
            distPlayer = Vector3.Distance(playerTransform.position, transform.position);
            CheckHP();
        }

        public void ActiveState(StateControllerAICAC.AIState newState)
        {
            stateControllerTrashMob.SetActiveState(newState);
        }

        public void ActiveStateDodge()
        {
            if (enemyHealth._hp > 0)
                stateControllerTrashMob.SetActiveState(StateControllerAICAC.AIState.Dodge);
        }

        public void ActiveKnockBackState()
        {
            if(enemyHealth._hp > 0)
                ActiveState(StateControllerAICAC.AIState.KnockBack);
        }

        public void CheckHP()
        {
            if (enemyHealth._hp <= 0 && !isDead)
            {
                isDead = true;
                aICACVarianteState.SetListActiveAI();
                ActiveState(StateControllerAICAC.AIState.BaseDeath);
            }
        }
    }
}