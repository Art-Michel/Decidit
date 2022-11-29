using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICAC : MonoBehaviour
{
    [SerializeField] public enum State { BaseIdle, BaseMovement, Dodge, BaseAttack, Death };
    [SerializeField] public State state;
    Transform playerTransform;
    public NavMeshAgent agent;
    AILife aILife;
    Animator animator;
    NavMeshHit navHit;
    RaycastHit hit;
    Ray dodgeRay;

    [SerializeField] LayerMask noMask;

    [Header("Distance Player")]
    [SerializeField] float distplayer;

    [Header("SpeedParameter")]
    [SerializeField] float currentSpeed;
    [SerializeField] float baseSpeed;
    [SerializeField] float dodgeSpeed;
    [SerializeField] float stopSpeed;
    [SerializeField] float setSpeedSmoothRot;
    [SerializeField] float speedSmoothRot;
    [SerializeField] float rotSpeed;

    [Header("Attack Parameter")]
    [SerializeField] float attackRange;
    [SerializeField] float currentAttackRate;
    [SerializeField] float maxAttackRate;
    [SerializeField] int damageBaseAttack;
    [SerializeField] bool isAttacking;
    [SerializeField] BoxCollider attackCollider;

    [Header("Dodge")]
    [SerializeField] float dodgeLenght;
    [SerializeField] bool isDodging;
    [SerializeField] bool rightDodge;
    [SerializeField] bool leftDodge;
    [SerializeField] bool dodgeIsSet;
    [SerializeField] Vector3 targetDodgeVector;
    [SerializeField] Transform spawnRayDodge;

    [Header("Dodge")]
    [SerializeField] float maxDelayIdleState;
    [SerializeField] float currentDelayIdleState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        aILife = GetComponent<AILife>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //distplayer = Vector3.Distance(playerTransform.position, transform.position);

        /*if (state != State.Death)
            SmoothLookForward();*/
        if (state != State.Death)
            transform.LookAt(playerTransform.position);

        SpeedManager();

        if (aILife.hp <= 0)
        {
            state = State.Death;
        }

        switch (state)
        {
            case State.BaseIdle:
                StateIdle();
                break;

            case State.BaseMovement:
                //BaseMove();
                break;

            case State.Dodge:
                if (dodgeIsSet)
                    Dodge();
                break;

            case State.BaseAttack:
                BaseAttackTimer();
                break;

            case State.Death:
                Death();
                break;
        }
    }
    private void FixedUpdate()
    {
        if (state == State.Dodge)
        {
            if (!dodgeIsSet)
                SetDodgePosition();
        }
    }

    void SmoothLookForward()
    {
        Vector3 direction;
        Vector3 relativePos;

        direction = playerTransform.position;

        //direction = agent.velocity + transform.forward * 100f;

        relativePos.x = direction.x - transform.position.x;
        relativePos.y = 0;
        relativePos.z = direction.z - transform.position.z;

        speedSmoothRot = setSpeedSmoothRot;

        Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), speedSmoothRot);
        transform.rotation = rotation;
    }

    void StateIdle()
    {
        if(currentDelayIdleState > 0)
        {
            currentDelayIdleState -= Time.deltaTime;
        }
        else
        {
            currentDelayIdleState = maxDelayIdleState;

            state = State.BaseMovement;
        }
    }


    /// <summary>
    /// State Base Movement 
    /// </summary>
    void BaseMove()
    {
        agent.SetDestination(playerTransform.position);

        if(distplayer < attackRange)
        {
            state = State.BaseAttack;
        }
    }


    /// <summary>
    /// State Dodge
    /// </summary>
    void SetDodgePosition()
    {
        if (Random.Range(0,2) ==0)
        {
            leftDodge = false;
            rightDodge = true;
        }
        else
        {
            rightDodge = false;
            leftDodge = true;
        }

        if (rightDodge) // choisi l esquive par la droite 
        {
            spawnRayDodge.localEulerAngles = new Vector3(spawnRayDodge.localEulerAngles.x, 90, 0);
            hit = RaycastAIManager.RaycastAI(spawnRayDodge.position, spawnRayDodge.forward, noMask, Color.red, dodgeLenght);

            if (!DetectDodgePointIsOnNavMesh(hit.point))
            {
                if (!leftDodge)
                {
                    leftDodge = true;
                }
                else
                {
                    isDodging = false;
                }
            }
            else
            {
                targetDodgeVector = hit.point;
                dodgeIsSet = true;
            }
        }
        if (leftDodge) // choisi l esquive par la gauche 
        {
            spawnRayDodge.localEulerAngles = new Vector3(spawnRayDodge.localEulerAngles.x, -90, 0);
            hit = RaycastAIManager.RaycastAI(spawnRayDodge.position, spawnRayDodge.forward, noMask, Color.red, dodgeLenght);

            if (!DetectDodgePointIsOnNavMesh(hit.point))
            {
                if (!rightDodge)
                {
                    rightDodge = true;
                }
                else
                {
                    isDodging = false;
                }
            }
            else
            {
                targetDodgeVector = hit.point;
                dodgeIsSet = true;
            }
        }
    }
    void Dodge()
    {
        if (Vector3.Distance(transform.position, targetDodgeVector) > 1.1f)
        {
            isDodging = true;
            agent.SetDestination(targetDodgeVector);
        }
        else
        {
            isDodging = false;
            Debug.Log("Stop dodge");
            dodgeIsSet = false;
            state = State.BaseIdle;
        }
    }


    /// <summary>
    /// State Base Attack 
    /// </summary>
    void BaseAttackTimer()
    {
        if (distplayer > attackRange && !isAttacking)
        {
            currentAttackRate = 0;
            state = State.BaseMovement;
        }
        else
        {
            if (currentAttackRate <= 0)
            {
                animator.SetBool("Attack", true);
                isAttacking = true;
                currentAttackRate = maxAttackRate;
            }
            else if(!isAttacking)
            {
                currentAttackRate -= Time.deltaTime;
            }
        }
    }


    /// <summary>
    /// State Death 
    /// </summary>
    void Death()
    {

    }


    public void ActiveDodge()
    {
        if(!isDodging)
        {
            currentAttackRate = 0;
            animator.SetBool("Attack", false);
            isAttacking = false;

            state = State.Dodge;
        }
    }


    void SpeedManager()
    {
        switch (state)
        {
            case State.BaseMovement:
                currentSpeed = baseSpeed;
                break;

            case State.Dodge:
                currentSpeed = dodgeSpeed;
                break;

            case State.BaseAttack:
                currentSpeed = stopSpeed;
                break;

            case State.Death:
                currentSpeed = stopSpeed;
                break;
        }
        agent.speed = currentSpeed;
    }

    // fonction qui renvoie vrai si le point "dodgePoint" se trouve sur le NavMesh et faux si ce n est pas le cas
    bool DetectDodgePointIsOnNavMesh(Vector3 dodgePoint)
    {
        if (NavMesh.SamplePosition(dodgePoint, out navHit, 0.1f, NavMesh.AllAreas))
        {
            Debug.Log("point on nav mesh");
            return true;
        }
        else
        {
            Debug.Log("point out nav mesh");
            return false;
        }
    }


    /// <summary>
    /// Animation Event
    /// </summary>
    void EndAttack()
    {
        Debug.Log(animator);
        animator.SetBool("Attack", false);
        isAttacking = false;
    }
}