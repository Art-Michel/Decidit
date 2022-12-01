using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BullAI : MonoBehaviour
{
    [SerializeField] enum State { BaseIdle, BaseMovement, WaitBeforeRush, RushMovement, BaseAttack, Death};
    [SerializeField] State state;

    public NavMeshAgent agent;
    AILife aILife;
    public Transform playerTransform;
    [SerializeField] LayerMask noMask;
    RaycastHit hit;
    EnemyHealth enemyHealth;

    [Header("Health Point")]
    public int hp;

    [Header("Distance ennemi / player")]
    [SerializeField]public float distPlayer;

    [Header("SpeedParameter")]
    [SerializeField] float setSpeedSmoothRot;
    [SerializeField] float speedSmoothRot;

    [Header("Attack Parameter")]
    public BoxCollider attackCollider;
    public BoxCollider detectOtherAICollider;

    BaseIdleBull baseIdleBull;
    public BaseIdleBullSO baseIdleBullSO;
    public BaseIdleBullSO baseIdleBullSOInstance;

    BaseMoveBull baseMoveBull;
    public BaseMoveBullParameterSO baseMoveBullParameterSO;
    public BaseMoveBullParameterSO baseMoveBullParameterSOInstance;

    WaitBeforeRushBull waitBeforeRushBull;
    public CoolDownRushBullParameterSO coolDownRushBullParameterSO;
    public CoolDownRushBullParameterSO coolDownRushBullParameterSOInstance;

    RushBull rushBull;
    public RushBullParameterSO rushBullParameterSO;
    public RushBullParameterSO rushBullParameterSOInstance;

    BaseAttackBull baseAttackBull;
    public BaseAttackBullSO baseAttackBullSO;
    public BaseAttackBullSO baseAttackBullSOInstance;

    DeathBull deathBull;
    public DeathBullParameterSO deathBullParameterSO;
    public DeathBullParameterSO deathBullParameterSOInstance;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        aILife = GetComponent<AILife>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        enemyHealth = GetComponent<EnemyHealth>();

        baseIdleBullSOInstance = Instantiate(baseIdleBullSO);
        baseMoveBullParameterSOInstance = Instantiate(baseMoveBullParameterSO);
        coolDownRushBullParameterSOInstance = Instantiate(coolDownRushBullParameterSO);
        rushBullParameterSOInstance = Instantiate(rushBullParameterSO);
        baseAttackBullSOInstance = Instantiate(baseAttackBullSO);
        deathBullParameterSOInstance = Instantiate(deathBullParameterSO);


        baseIdleBull = new BaseIdleBull();
        baseIdleBull.bullAI = this;
        baseIdleBull.baseIdleBullSO = this.baseIdleBullSOInstance;

        baseMoveBull = new BaseMoveBull();
        baseMoveBull.bullAI = this;
        baseMoveBull.baseMoveBullSO = this.baseMoveBullParameterSOInstance;

        waitBeforeRushBull = new WaitBeforeRushBull();
        waitBeforeRushBull.bullAI = this;
        waitBeforeRushBull.coolDownRushBullSO = this.coolDownRushBullParameterSOInstance;

        rushBull = new RushBull();
        rushBull.bullAI = this;
        rushBull.rushBullSO = rushBullParameterSOInstance;

        baseAttackBull = new BaseAttackBull();
        baseAttackBull.bullAI = this;
        baseAttackBull.baseAttackBullSO = baseAttackBullSOInstance;

        deathBull = new DeathBull();
        deathBull.bullAI = this;
        deathBull.deathBullParameterSO = this.deathBullParameterSOInstance;
    }

    public void SwitchToNewState(int indexState)
    {
        state = (State)indexState;
    }

    // Update is called once per frame
    void Update()
    {
        distPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (state != State.Death)
            SmoothLookAtPlayer();

        // v= d/t;
        // t = d/v;
        // d = v*t;

        if(enemyHealth._hp <=0)
        {
            state = State.Death;
        }

        switch(state)
        {
            case State.BaseIdle:
                baseIdleBull.BaseIdle();
                break;
            case State.BaseMovement:
                baseMoveBull.BaseMovement();
                baseMoveBull.CoolDownRush();
                break;
            case State.WaitBeforeRush:
                waitBeforeRushBull.CoolDownBeforeRush();
                break;
            case State.RushMovement:
                rushBull.RushMovement();
                break;
            case State.BaseAttack:
                baseAttackBull.BaseAttackTimer();
                break;
            case State.Death:
                deathBull.Death();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (state == State.RushMovement)
        {
            rushBull.CheckObstacleOnPath();
        }
    }

    void SmoothLookAtPlayer()
    {
        Vector3 direction;
        Vector3 relativePos;

        direction = playerTransform.position;

        relativePos.x = direction.x - transform.position.x;
        relativePos.y = 0;
        relativePos.z = direction.z - transform.position.z;

        if (rushBullParameterSOInstance.stopLockPlayer)
            speedSmoothRot = 0;
        else
            speedSmoothRot = setSpeedSmoothRot;

        Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), speedSmoothRot);
        transform.rotation = rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && state == State.BaseAttack)
        {
            baseAttackBull.LaunchBaseAttack(true);
        }
        else
        {
            baseAttackBull.LaunchBaseAttack(false);
        }

        if (other.CompareTag("Ennemi") && state == State.RushMovement)
        {
            if (!rushBullParameterSOInstance.ennemiInCollider.Contains(other.gameObject) || rushBullParameterSOInstance.ennemiInCollider == null)
                rushBullParameterSOInstance.ennemiInCollider.Add(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ennemi") && state == State.RushMovement)
        {
            if (rushBullParameterSOInstance.ennemiInCollider != null)
            {
                for (int i = 0; i < rushBullParameterSOInstance.ennemiInCollider.Count; i++)
                {
                    if (rushBullParameterSOInstance.ennemiInCollider[i].GetComponent<StateManagerAICAC>() != null)
                    {
                        RaycastHit hit = RaycastAIManager.RaycastAI(transform.position, transform.forward, noMask, Color.red, 10f);
                        float angle;
                        angle = Vector3.SignedAngle(transform.forward, hit.normal, Vector3.up);

                        if (angle > 0)
                        {
                            rushBullParameterSOInstance.ennemiInCollider[i].GetComponent<StateManagerAICAC>().dodgeParameterAICACSOInstance.targetObjectToDodge = this.transform;
                            rushBullParameterSOInstance.ennemiInCollider[i].GetComponent<StateManagerAICAC>().dodgeParameterAICACSOInstance.rightDodge = true;
                            rushBullParameterSOInstance.ennemiInCollider[i].GetComponent<StateManagerAICAC>().SwitchToNewState(2);
                        }
                        else
                        {
                            rushBullParameterSOInstance.ennemiInCollider[i].GetComponent<StateManagerAICAC>().dodgeParameterAICACSOInstance.targetObjectToDodge = this.transform;
                            rushBullParameterSOInstance.ennemiInCollider[i].GetComponent<StateManagerAICAC>().dodgeParameterAICACSOInstance.leftDodge = true;
                            rushBullParameterSOInstance.ennemiInCollider[i].GetComponent<StateManagerAICAC>().dodgeParameterAICACSOInstance.dodgeRushBull = true;
                            rushBullParameterSOInstance.ennemiInCollider[i].GetComponent<StateManagerAICAC>().SwitchToNewState(2);
                        }
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ennemi"))
        {
            rushBullParameterSOInstance.ennemiInCollider.Remove(other.gameObject);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player") && state == State.RushMovement)
        {
            PlayerController.ApplyDamage(rushBullParameterSOInstance.damageRushAttack);
        }
    }


    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0f)
        {
            return 1.0f;
        }
        else if (dir < 0.0f)
        {
            return -1.0f;
        }
        else
        {
            return 0.0f;
        }
    }
}