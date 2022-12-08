using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateManagerAICAC : MonoBehaviour
{
    [SerializeField] public enum State { BaseIdle, BaseMovement, Dodge, BaseAttack, Death, SurroundPlayer};
    [SerializeField] public State state;

    RaycastHit hitRight, hitLeft;
    [SerializeField] LayerMask noMask;
    [SerializeField] GameObject hitBox;

    public List<GameObject> listOtherCACAI = new List<GameObject>();

    public List<Transform> listOtherAIContact = new List<Transform>();

    public Transform playerTransform;
    public Transform spawnSurroundDodge;
    public NavMeshAgent agent;
    public AILife aILife;
    public Animator myAnimator;
    public AICACVarianteState aICACVarianteState;
    EnemyHealth enemyHealth;
    [SerializeField] Material_Instances material_Instances;

    [Header("Distance Player")]
    [SerializeField] public float distplayer;
    [SerializeField] public float offsetDestination;

    [Header("SpeedParameter")]
    [SerializeField] public float maxSpeedRot;
    [SerializeField] public float speedRot;
    [SerializeField] public float smoothRot;

    [Header("Dodge")]
    [SerializeField] public Transform spawnRayDodge;

    [Header("Detect Ennemi proche")]
    [SerializeField] bool right;
    [SerializeField] bool left;



    [Header("State Script")]
    public BaseMoveParameterAICAC baseMoveParameterAICACSO;
    public BaseMoveParameterAICAC baseMoveParameterAICACSOInstance;
    BaseMoveAICAC baseMoveAICAC;

    BaseAttackAICAC baseAttackAICAC;
    public BaseAttackParameterAICAC baseAttackParameterAICACSO;
    public BaseAttackParameterAICAC baseAttackParameterAICACSOInstance;

    BaseIdleAICAC baseIdleAICAC;
    public BaseIdleParameterAICAC baseIdleParameterAICACSO;
    public BaseIdleParameterAICAC baseIdleParameterAICACSOInstance;

    DeathAICAC deathAICAC;
    public DeathParameterAICAC deathParameterAICACSO;
    public DeathParameterAICAC deathParameterAICACSOInstance;

    public DodgeAICAC dodgeAICAC;
    public DodgeParameterAICAC dodgeParameterAICACSO;
    public DodgeParameterAICAC dodgeParameterAICACSOInstance;

    SurroundAICAC surroundAICAC;
    public SurroundParameterAICAC surroundParameterAICACSO;
    public SurroundParameterAICAC surroundParameterAICACSOInstance;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        aILife = GetComponent<AILife>();
        playerTransform = GameObject.FindWithTag("Player").transform.GetChild(0).transform;
        myAnimator = GetComponent<Animator>();
        spawnSurroundDodge = transform.Find("SpawnSurroundRay");
        aICACVarianteState = transform.parent.GetComponent<AICACVarianteState>();
        enemyHealth = GetComponent<EnemyHealth>();
        material_Instances = GetComponent<Material_Instances>();

        baseMoveAICAC = new BaseMoveAICAC();
        baseAttackAICAC = new BaseAttackAICAC();
        baseIdleAICAC = new BaseIdleAICAC();
        deathAICAC = new DeathAICAC();
        dodgeAICAC = new DodgeAICAC();
        surroundAICAC = new SurroundAICAC();

        baseMoveParameterAICACSOInstance = Instantiate(baseMoveParameterAICACSO);
        baseAttackParameterAICACSOInstance = Instantiate(baseAttackParameterAICACSO);
        baseIdleParameterAICACSOInstance = Instantiate(baseIdleParameterAICACSO);
        deathParameterAICACSOInstance = Instantiate(deathParameterAICACSO);
        dodgeParameterAICACSOInstance = Instantiate(dodgeParameterAICACSO);
        surroundParameterAICACSOInstance = Instantiate(surroundParameterAICACSO);

        baseMoveAICAC.stateManagerAICAC = this;
        baseMoveAICAC.baseMoveParameterSO = baseMoveParameterAICACSOInstance;
        baseMoveAICAC.baseAttackParameterSO = baseAttackParameterAICACSOInstance;

        baseAttackAICAC.stateManagerAICAC = this;
        baseAttackAICAC.baseAttackParameterSO = baseAttackParameterAICACSOInstance;

        baseIdleAICAC.stateManagerAICAC = this;
        baseIdleAICAC.baseIdleParameterSO = baseIdleParameterAICACSOInstance;

        deathAICAC.stateManagerAICAC = this;
        deathAICAC.deathParameterAICACSO = deathParameterAICACSOInstance;

        dodgeAICAC.stateManagerAICAC = this;
        dodgeAICAC.dodgeAICACSO = dodgeParameterAICACSOInstance;
        dodgeAICAC.baseMoveAICACSO = baseMoveParameterAICACSOInstance;

        surroundAICAC.stateManagerAICAC = this;
        surroundAICAC.aICACVarianteState = aICACVarianteState;
        surroundAICAC.SurroundAICACSO = surroundParameterAICACSOInstance;
    }

    public void SwitchToNewState(int indexState)
    {
        if (state != State.Death)
        {
            if (indexState == 2)
            {
                if (state != State.Dodge && state != State.SurroundPlayer)
                {
                    state = (State)indexState;
                }
            }
            else
            {
                state = (State)indexState;
            }
        }
    }

    public int GetCurrentState()
    {
        return (int)state;
    }

    public void Update()
    {
        distplayer = Vector3.Distance(playerTransform.position, transform.position);

        if (state != State.Death)
            SmoothLookForward();

        if (enemyHealth._hp <= 0)
        {
            state = State.Death;
        }

        switch (state)
        {
            case State.BaseIdle:
                baseIdleAICAC.StateIdle();
                break;

            case State.BaseMovement:
                baseMoveAICAC.SmoothLookAt();
                baseMoveAICAC.BaseMovement(agent, playerTransform, transform, distplayer);
                break;

            case State.Dodge:
                dodgeAICAC.SmoothLookAt();
                if (dodgeParameterAICACSOInstance.dodgeIsSet)
                    dodgeAICAC.Dodge();
                break;

            case State.BaseAttack:
                baseAttackAICAC.BaseAttack(myAnimator);
                baseAttackAICAC.SmoothLookAt();
                break;

            case State.SurroundPlayer:
                surroundAICAC.ChooseDirection();
                break;

            case State.Death:
                deathAICAC.Death();
                break;
        }
    }
    private void FixedUpdate()
    {
        if (state == State.Dodge)
        {
            if (!dodgeParameterAICACSOInstance.dodgeIsSet)
                dodgeAICAC.SetDodgePosition();
        }
        else if (state == State.SurroundPlayer)
        {
            surroundAICAC.ChooseDirection();
            surroundAICAC.GetSurroundDestination();
        }
    }

    void SmoothLookForward()
    {
        if (state == State.SurroundPlayer)
        {
            agent.angularSpeed = 360f;
        }
        else
        {
            agent.angularSpeed = 0f;
        }
    }

    /// <summary>
    /// Animation Event
    /// </summary>

    void PreAttack()
    {
        material_Instances.material.color = material_Instances.colorPreAtatck;
        material_Instances.ChangeColorTexture(material_Instances.colorPreAtatck);
    }
    void LaunchAttack()
    {
        material_Instances.material.color = material_Instances.color;
        material_Instances.ChangeColorTexture(material_Instances.color);
        hitBox.SetActive(true);
    }
    void EndAttack()
    {
        hitBox.SetActive(false);
        myAnimator.SetBool("Attack", false);
        baseAttackParameterAICACSOInstance.isAttacking = false;
    }
}