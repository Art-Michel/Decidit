using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyAINavMesh : MonoBehaviour
{
    [SerializeField] enum State { BaseMovement, LockPlayer, BaseAttack, Death };
    [SerializeField] State state;

    public NavMeshAgent agent;
    public Transform playerTransform, childObj;
    public AILife aILife;

    [Header("Destination Variable (Debug)")]
    [SerializeField]public float distDestination;
    [SerializeField]public BoxCollider myCollider;

    BaseMoveFly baseMoveFly;
    public BaseMoveFlySO baseMoveFlySO;
    public BaseMoveFlySO baseMoveFlySOInstance;

    LockPlayerFly lockPlayerFly;
    public LockPlayerFlySO lockPlayerFlySO;
    public LockPlayerFlySO lockPlayerFlySOInstance;

    BaseAttackFly baseAttackFly;
    public BaseAttackFlySO baseAttackFlySO;
    public BaseAttackFlySO baseAttackFlySOInstance;

    DeathFly deathFly;
    public DeathFlySO deathFlySO;
    public DeathFlySO deathFlySOInstance;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        childObj = transform.parent;
        aILife = GetComponent<AILife>();

        baseMoveFly = new BaseMoveFly();
        lockPlayerFly = new LockPlayerFly();
        baseAttackFly = new BaseAttackFly();
        deathFly = new DeathFly();

        baseMoveFly.flyAINavMesh = this;
        lockPlayerFly.flyAINavMesh = this;
        baseAttackFly.flyAINavMesh = this;
        deathFly.flyAINavMesh = this;

        baseMoveFlySOInstance = Instantiate(baseMoveFlySO);
        lockPlayerFlySOInstance = Instantiate(lockPlayerFlySO);
        baseAttackFlySOInstance = Instantiate(baseAttackFlySO);
        deathFlySOInstance = Instantiate(deathFlySO);

        baseMoveFly.baseMoveFlySO = baseMoveFlySOInstance;
        baseMoveFly.baseAttackFlySO = baseAttackFlySOInstance;
        lockPlayerFly.lockPlayerFlySO = lockPlayerFlySOInstance;
        lockPlayerFly.baseAttackFlySO = baseAttackFlySOInstance;
        baseAttackFly.baseAttackFlySO = baseAttackFlySOInstance;
        baseAttackFly.lockPlayerFly = lockPlayerFly;
        baseAttackFly.lockPlayerFlySO = lockPlayerFlySOInstance;
        deathFly.deathFlySO = deathFlySOInstance;
    }

    public void SwitchToNewState(int indexState)
    {
        state = (State)indexState;
    }

    // Update is called once per frame
    void Update()
    {
        if (aILife.hp <= 0)
            state = State.Death;

        switch(state)
        {
            case State.BaseMovement:
                 baseMoveFly.SmoothLookAtYAxisPatrol();
                break;

            case State.LockPlayer:
                lockPlayerFly.LockPlayer();
                lockPlayerFly.SmoothLookAtYAxisAttack();
                break;

            case State.BaseAttack:
                baseAttackFly.Attack();
                break;

            case State.Death:
                deathFly.Death();
                break;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            if(state == State.BaseMovement)
            {
                baseMoveFly.ForceLaunchAttack();
            }
        }
        else if(collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("WallAI") || collision.gameObject.CompareTag("Obstacle"))
        {
            if(state == State.BaseAttack)
                state = State.BaseMovement;
        }
        else if(collision.gameObject.CompareTag("Player"))
        {
            if(state == State.BaseAttack)
            {
                PlayerController.ApplyDamage(10);
            }
        }
    }
}