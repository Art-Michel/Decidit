using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WallAI : MonoBehaviour
{
    [SerializeField] enum State { BaseMovement, BaseAttack, Death };
    [SerializeField] State state;

    public Transform areaWallAI;
    public NavMeshAgent agent;
    public Animator animator;
    public PlayerController playerController;
    public AILife aILife;
    EnemyHealth enemyHealth;

    [Header("*Search new Position")]
    public BoxCollider[] walls;
    public Transform playerTransform;

    [Header("*Attack")]
    public Transform spawnBullet;

    [Header("*Wall Crack Effect")]
    public float orientation;

    BaseMoveWallAI baseMoveWallAI;
    public BaseMoveWallAISO baseMoveWallAISO;
    public BaseMoveWallAISO baseMoveWallAISOClone;

    BaseAttackWallAI baseAttackWallAI;
    public BaseAttackWallAISO baseAttackWallAISO;
    public BaseAttackWallAISO baseAttackWallAISOClone;

    DeathWallAI deathWallAI;
    public DeathWallAISO deathWallAISO;
    public DeathWallAISO deathWallAISOInstance;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        areaWallAI = GameObject.Find("Area_WAllAi").transform;
        aILife = GetComponent<AILife>();
        enemyHealth = GetComponent<EnemyHealth>();

        for (int i=0; i < areaWallAI.childCount; i++)
        {
            walls[i] = areaWallAI.GetChild(i).GetComponent<BoxCollider>();
        }

        baseMoveWallAI = new BaseMoveWallAI();
        baseMoveWallAI.wallAI = this;
        baseMoveWallAISOClone = Instantiate(baseMoveWallAISO);
        baseMoveWallAI.baseMoveWallAISO = baseMoveWallAISOClone;
        baseMoveWallAISOClone.lastWallCrack = Instantiate(baseMoveWallAISOClone.wallCrackPrefab, transform.parent.position, Quaternion.Euler(0, orientation, 0));

        baseAttackWallAI = new BaseAttackWallAI();
        baseAttackWallAI.wallAI = this;
        baseAttackWallAISOClone = Instantiate(baseAttackWallAISO);
        baseAttackWallAI.baseAttackWallAISO = baseAttackWallAISOClone;

        deathWallAI = new DeathWallAI();
        deathWallAI.wallAI = this;
        deathWallAISOInstance = Instantiate(deathWallAISO);
        deathWallAI.deathWallAISO = deathWallAISOInstance;
    }

    public void SwitchToNewState(int indexState)
    {
        state = (State)indexState;
    }

    private void FixedUpdate()
    {
        if(!baseMoveWallAISOClone.findNewPos && state == State.BaseMovement)
            baseMoveWallAI.SelectNewPos();
    }

    void Update()
    {
        if (enemyHealth._hp <= 0)
            state = State.Death;

        switch (state)
        {
            case State.BaseMovement:
                baseMoveWallAI.MoveAI();
                break;

            case State.BaseAttack:
                baseAttackWallAI.LaunchAttack();
                break;

            case State.Death:
                deathWallAI.Death();
                break;
        }
    }

    ////////////////////////  ANIMATION EVENT CALL BY PARENT SCRIPT\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    public void StartAttack()
    {
        if(state != State.Death)
        {
            baseAttackWallAI.CalculateSpeedProjectile();
            baseAttackWallAI.ThrowProjectile();
        }
    }

    public void EndAttack()
    {
        baseAttackWallAI.ReturnBaseMoveState();
    }
}