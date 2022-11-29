using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseMoveAICAC
{
    public StateManagerAICAC virtual_AICAC;
    public BaseMoveParameterAICAC baseMoveParameterSO;
    public BaseAttackParameterAICAC baseAttackParameterSO;

    public void BaseMovement(NavMeshAgent agent, Transform playerTransform, Transform transform, float distplayer)
    {
        distplayer = Vector3.Distance(playerTransform.position, transform.position);


        agent.SetDestination(playerTransform.position);

        if (distplayer < baseMoveParameterSO.attackRange)
        {
            baseAttackParameterSO.isAttacking = false;
            virtual_AICAC.SwitchToNewState(3);
        }
        else
        {
            if (distplayer >= baseMoveParameterSO.distCanRun)
                agent.speed = baseMoveParameterSO.runSpeed;
            else if(distplayer <= baseMoveParameterSO.distStopRun)
                agent.speed = baseMoveParameterSO.baseSpeed;
        }
    }
}