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
        public Animator myAnimator;
        public AICACVarianteState aICACVarianteState;
        EnemyHealth enemyHealth;
        public Material_Instances material_Instances;
        public float distPlayer;
        [SerializeField] StateControllerAICAC stateControllerAICAC;

        [Header("Ref Move State")]
        public float offsetDestination;
        public Vector3 debugDestination;

        [Header("Ref Dodge State")]
        public Transform spawnRayDodge;

        [Header("Ref Attack State")]
        public GameObject hitBox;

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

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            playerTransform = GameObject.FindWithTag("Player").transform.GetChild(0).transform;
            myAnimator = GetComponent<Animator>();
            spawnSurroundDodge = transform.Find("SpawnSurroundRay");
            aICACVarianteState = transform.parent.GetComponent<AICACVarianteState>();
            enemyHealth = GetComponent<EnemyHealth>();
            material_Instances = GetComponent<Material_Instances>();


            baseMoveAICACSO = Instantiate(baseMoveAICACSO);
            baseAttackAICACSO = Instantiate(baseAttackAICACSO);
            baseIdleAICACSO = Instantiate(baseIdleAICACSO);
            deathAICACSO = Instantiate(deathAICACSO);
            dodgeAICACSO = Instantiate(dodgeAICACSO);
            surroundAICACSO = Instantiate(surroundAICACSO);
        }

        private void Update()
        {
            distPlayer = Vector3.Distance(playerTransform.position, transform.position);
        }

        public void ActiveState(StateControllerAICAC.AIState newState)
        {
            Debug.Log(stateControllerAICAC);

            stateControllerAICAC.SetActiveState(newState);
        }

        public void ActiveStateDodge()
        {
            Debug.Log(stateControllerAICAC);
            stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.Dodge);
        }
    }
}