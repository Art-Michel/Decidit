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

    public List<GameObject> listOtherCACAI = new List<GameObject>();

    public List<Transform> listOtherAIContact = new List<Transform>();

    public Transform playerTransform;
    public Transform spawnSurroundDodge;
    public NavMeshAgent agent;
    public AILife aILife;
    public Animator myAnimator;
    public AICACVarianteState aICACVarianteState;

    [Header("Distance Player")]
    [SerializeField] public float distplayer;

    [Header("SpeedParameter")]
    [SerializeField] public float setSpeedSmoothRot;
    [SerializeField] public float speedSmoothRot;

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
        playerTransform = GameObject.FindWithTag("Player").transform;
        myAnimator = GetComponent<Animator>();
        spawnSurroundDodge = transform.Find("SpawnSurroundRay");
        aICACVarianteState = transform.parent.GetComponent<AICACVarianteState>();

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

        baseMoveAICAC.virtual_AICAC = this;
        baseMoveAICAC.baseMoveParameterSO = baseMoveParameterAICACSOInstance;
        baseMoveAICAC.baseAttackParameterSO = baseAttackParameterAICACSOInstance;

        baseAttackAICAC.stateManagerAICAC = this;
        baseAttackAICAC.baseAttackParameterSO = baseAttackParameterAICACSOInstance;

        baseIdleAICAC.virtual_AICAC = this;
        baseIdleAICAC.baseIdleParameterSO = baseIdleParameterAICACSOInstance;

        deathAICAC.stateManagerAICAC = this;
        deathAICAC.deathParameterAICACSO = deathParameterAICACSOInstance;

        dodgeAICAC.stateManagerAICAC = this;
        dodgeAICAC.dodgeAICACSO = dodgeParameterAICACSOInstance;

        surroundAICAC.stateManagerAICAC = this;
        surroundAICAC.aICACVarianteState = aICACVarianteState;
        surroundAICAC.SurroundAICACSO = surroundParameterAICACSOInstance;
    }

    public void SwitchToNewState(int indexState)
    {
        Debug.Log("SwitchToNewState");

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

        if (aILife.hp <= 0)
        {
            state = State.Death;
        }

        switch (state)
        {
            case State.BaseIdle:
                baseIdleAICAC.StateIdle();
                break;

            case State.BaseMovement:
                baseMoveAICAC.BaseMovement(agent, playerTransform, transform, distplayer);
                break;

            case State.Dodge:
                if (dodgeParameterAICACSOInstance.dodgeIsSet)
                    dodgeAICAC.Dodge();
                break;

            case State.BaseAttack:
                baseAttackAICAC.BaseAttack(myAnimator);
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
        Vector3 direction;
        Vector3 relativePos;

        direction = playerTransform.position;

        relativePos.x = direction.x - transform.position.x;
        relativePos.y = 0;
        relativePos.z = direction.z - transform.position.z;

        if (state == State.SurroundPlayer)
        {
            agent.angularSpeed = 360f;
            speedSmoothRot = 0;
        }
        else
        {
            agent.angularSpeed = 0;
            speedSmoothRot = setSpeedSmoothRot;
        }

        Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), speedSmoothRot);
        transform.rotation = rotation;
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    void EndAttack()
    {
        myAnimator.SetBool("Attack", false);
        baseAttackParameterAICACSOInstance.isAttacking = false;
    }
}