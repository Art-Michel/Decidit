using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateManagerAICAC : MonoBehaviour
{
    [SerializeField] public enum State { BaseIdle, BaseMovement, Dodge, BaseAttack, Death, SurroundPlayer, DodgeOtherAI};
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

    DodgeOtherAICAC dodgeOtherAICAC;
    public DodgeOtherParameterAICAC dodgeOtherParameterAICACSO;
    public DodgeOtherParameterAICAC dodgeOtherParameterAICACSOInstance;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        aILife = GetComponent<AILife>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        myAnimator = GetComponent<Animator>();
        spawnSurroundDodge = transform.Find("SpawnSurroundRay");

        baseMoveAICAC = new BaseMoveAICAC();
        baseAttackAICAC = new BaseAttackAICAC();
        baseIdleAICAC = new BaseIdleAICAC();
        deathAICAC = new DeathAICAC();
        dodgeAICAC = new DodgeAICAC();
        surroundAICAC = new SurroundAICAC();
        dodgeOtherAICAC = new DodgeOtherAICAC();

        baseMoveParameterAICACSOInstance = Instantiate(baseMoveParameterAICACSO);
        baseAttackParameterAICACSOInstance = Instantiate(baseAttackParameterAICACSO);
        baseIdleParameterAICACSOInstance = Instantiate(baseIdleParameterAICACSO);
        deathParameterAICACSOInstance = Instantiate(deathParameterAICACSO);
        dodgeParameterAICACSOInstance = Instantiate(dodgeParameterAICACSO);
        surroundParameterAICACSOInstance = Instantiate(surroundParameterAICACSO);
        dodgeOtherParameterAICACSOInstance = Instantiate(dodgeOtherParameterAICACSO);

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
        surroundAICAC.SurroundAICACSO = surroundParameterAICACSOInstance;

        dodgeOtherAICAC.stateManagerAICAC = this;
        dodgeOtherAICAC.dodgeOtherSO = dodgeOtherParameterAICACSOInstance;
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
                //surroundAICAC.ChooseDirection();
                break;
            case State.DodgeOtherAI:
                dodgeOtherAICAC.MoveDodge();
                break;

            case State.Death:
                deathAICAC.Death();
                break;
        }

        if (left && right && state == State.BaseMovement)
            state = State.DodgeOtherAI;
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
        else if (state == State.DodgeOtherAI)
            dodgeOtherAICAC.SetDodgeDestination();

        if (listOtherAIContact.Count>0)
            DetectNearOther();
    }

    void SmoothLookForward()
    {
        Vector3 direction;
        Vector3 relativePos;

        direction = playerTransform.position;

        relativePos.x = direction.x - transform.position.x;
        relativePos.y = 0;
        relativePos.z = direction.z - transform.position.z;

        if (state == State.SurroundPlayer || state == State.DodgeOtherAI)
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

    void DetectNearOther()
    {
        for(int i =0; i < listOtherCACAI.Count; i++)
        {
            hitRight = RaycastAIManager.RaycastAI(transform.position, listOtherAIContact[i].position - transform.position, noMask, Color.red, 5f);
            float angleRight;
            angleRight = Vector3.SignedAngle(listOtherAIContact[i].forward, transform.forward, Vector3.up);

            Debug.Log(gameObject.name + "  " +angleRight);

            if(Vector3.Distance(listOtherAIContact[i].position, transform.position) < 3)
            {
                if (angleRight > 0)
                {
                    right = true;
                }
            }
            else
            {
                if (angleRight > 0)
                {
                    right = false;
                    listOtherAIContact.Remove(listOtherAIContact[i]);
                }
            }

            hitLeft = RaycastAIManager.RaycastAI(transform.position, listOtherAIContact[i].position - transform.position, noMask, Color.blue, 5f);
            float angleLeft;
            angleLeft = Vector3.SignedAngle(listOtherAIContact[i].forward, transform.forward, Vector3.up);

            if (Vector3.Distance(listOtherAIContact[i].position, transform.position) < 3)
            {
                if (angleLeft < 0)
                {
                    left = true;
                }
            }
            else
            {
                if (angleLeft < 0)
                {
                    left = false;
                    listOtherAIContact.Remove(listOtherAIContact[i]);
                }
            }
        }
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    void EndAttack()
    {
        myAnimator.SetBool("Attack", false);
        baseAttackParameterAICACSOInstance.isAttacking = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(listOtherCACAI.Contains(other.gameObject))
        {
            if(!listOtherAIContact.Contains(other.transform))
                listOtherAIContact.Add(other.transform);
            Debug.Log("Contact srhab  " + Vector3.Distance(other.transform.position, transform.position));
        }
    }

    private void OnTriggerExit(Collider other)
    {
       /* if (listOtherCACAI.Contains(other.gameObject))
        {
            listOtherAIContact.Remove(other.transform);

            hit = RaycastAIManager.RaycastAI(transform.position, other.transform.position - transform.position, noMask, Color.red, 100f);
            float angle;
            angle = Vector3.SignedAngle(other.transform.forward, transform.forward, Vector3.up);

            if (angle > 0)
            {
                right = false;
            }
            else
            {
                left = false;
            }
        }*/
    }
}