using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseMoveAICAC
{
    public StateManagerAICAC stateManagerAICAC;
    public BaseMoveParameterAICAC baseMoveParameterSO;
    public BaseAttackParameterAICAC baseAttackParameterSO;

    public void BaseMovement(NavMeshAgent agent, Transform playerTransform, Transform transform, float distplayer)
    {
        Vector3 dir = playerTransform.position - stateManagerAICAC.transform.position;
        Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;

        //distplayer = Vector3.Distance(playerTransform.position, transform.position);

        Vector3 destination = playerTransform.position + left* stateManagerAICAC.offsetDestination;

        agent.SetDestination(destination);

        if (stateManagerAICAC.distplayer < baseMoveParameterSO.attackRange)
        {
            baseAttackParameterSO.isAttacking = false;
            baseMoveParameterSO.speedRot = 0;
            stateManagerAICAC.SwitchToNewState(3);
        }
        else
        {
            SpeedAdjusting();
        }
    }
    void SpeedAdjusting()
    {
        if (stateManagerAICAC.distplayer >= baseMoveParameterSO.distCanRun)
        {
            if(stateManagerAICAC.agent.speed < baseMoveParameterSO.runSpeed)
                stateManagerAICAC.agent.speed += baseMoveParameterSO.smoothSpeedRun * Time.deltaTime;
            else
                stateManagerAICAC.agent.speed = baseMoveParameterSO.runSpeed;
        }
        else if (stateManagerAICAC.distplayer <= baseMoveParameterSO.distStopRun)
        {
            if(stateManagerAICAC.agent.speed > baseMoveParameterSO.baseSpeed)
                stateManagerAICAC.agent.speed -= baseMoveParameterSO.smoothSpeedbase * Time.deltaTime;
            else
                stateManagerAICAC.agent.speed = baseMoveParameterSO.baseSpeed;
        }
    }

    public void SmoothLookAt()
    {
        Vector3 direction;
        Vector3 relativePos;

        direction = stateManagerAICAC.agent.destination;
        relativePos.x = direction.x - stateManagerAICAC.transform.position.x;
        relativePos.y = 0;
        relativePos.z = direction.z - stateManagerAICAC.transform.position.z;

        if (baseMoveParameterSO.speedRot < baseMoveParameterSO. maxSpeedRot)
            baseMoveParameterSO.speedRot += Time.deltaTime / baseMoveParameterSO.smoothRot;
        else
        {
            baseMoveParameterSO.speedRot = baseMoveParameterSO.maxSpeedRot;
        }

        Quaternion rotation = Quaternion.Slerp(stateManagerAICAC.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), baseMoveParameterSO.speedRot);
        stateManagerAICAC.transform.rotation = rotation;
    }
}