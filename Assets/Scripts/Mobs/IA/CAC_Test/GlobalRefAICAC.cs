using NaughtyAttributes;
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

        [Header("SynergyAttraction References")]
        public bool isInSynergyAttraction;

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

        [Foldout("Scriptable")] public BaseMoveParameterAICAC baseMoveAICACSO;
        [Foldout("Scriptable")] public BaseAttackParameterAICAC baseAttackAICACSO;
        [Foldout("Scriptable")] public BaseIdleParameterAICAC baseIdleAICACSO;
        [Foldout("Scriptable")] public DeathParameterAICAC deathAICACSO;
        [Foldout("Scriptable")] public DodgeParameterAICAC dodgeAICACSO;
        [Foldout("Scriptable")] public SurroundParameterAICAC surroundAICACSO;
        [Foldout("Scriptable")] public KnockBackParameterAICAC knockBackAICACSO;
        [Foldout("Scriptable")] public AttractionSO AttractionSO;

        [Foldout("VeryEasy")] public BaseMoveParameterAICAC baseMoveAICACSO_VeryEZ;
        [Foldout("Medium")] public BaseMoveParameterAICAC baseMoveAICACSO_Med;
        [Foldout("VeryHard")] public BaseMoveParameterAICAC baseMoveAICACSO_VeryHard;

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

            switch (ApplyDifficulty.Instance.indexDifficulty)
            {
                case 0:
                    baseMoveAICACSO = Instantiate(baseMoveAICACSO_VeryEZ);
                    break;

                case 1:
                    baseMoveAICACSO = Instantiate(baseMoveAICACSO_Med);
                    break;

                case 2:
                    baseMoveAICACSO = Instantiate(baseMoveAICACSO_VeryHard);
                    break;
            }

            baseAttackAICACSO = Instantiate(baseAttackAICACSO);
            baseIdleAICACSO = Instantiate(baseIdleAICACSO);
            deathAICACSO = Instantiate(deathAICACSO);
            dodgeAICACSO = Instantiate(dodgeAICACSO);
            surroundAICACSO = Instantiate(surroundAICACSO);
            knockBackAICACSO = Instantiate(knockBackAICACSO);
            AttractionSO = Instantiate(AttractionSO);
        }

        private void Update()
        {
            distPlayer = Vector3.Distance(playerTransform.position, transform.position);
            CheckHP();

            if (isInSynergyAttraction)
                ActiveAttractionState();
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

        public void ActiveAttractionState()
        {
            ActiveState(StateControllerAICAC.AIState.Attraction);
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