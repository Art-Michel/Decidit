using UnityEngine;
using UnityEngine.AI;

public class BaseMoveBull
{
    public BullAI bullAI;
    public BaseMoveBullParameterSO baseMoveBullSO;
    public BaseAttackBullSO baseAttackBullSO;
    
    public void BaseMovement()
    {
        Vector3 newDestination;
        bullAI.agent.speed = baseMoveBullSO.baseSpeed;
        if(bullAI.distPlayer > 5)
            newDestination = bullAI.playerTransform.position + (bullAI.playerTransform.right * bullAI.offsetDestination);
        else
            newDestination = bullAI.playerTransform.position;

        bullAI.agent.SetDestination(CheckNavMeshPoint(newDestination));
        SmoothLookAtPlayer();
        if (bullAI.distPlayer < baseAttackBullSO.attackRange)
        {
            baseMoveBullSO.speedRot = 0;
            bullAI.SwitchToNewState(4);
        }
    }
    Vector3 CheckNavMeshPoint(Vector3 newDestination)
    {
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(newDestination, out closestHit, 20, 1))
        {
            newDestination = closestHit.position;
        }
        return newDestination;
    }

    public void CoolDownRush()
    {
        if(bullAI.distPlayer < baseMoveBullSO.distActiveCoolDownRush)
        {
            if (baseMoveBullSO.currentCoolDownWaitRush > 0)
            {
                baseMoveBullSO.currentCoolDownWaitRush -= Time.deltaTime;
            }
            else
            {
                baseMoveBullSO.currentCoolDownWaitRush = baseMoveBullSO.maxCoolDownRush;
                baseMoveBullSO.speedRot = 0;
                bullAI.SwitchToNewState(2);
            }
        }
    }

    void SmoothLookAtPlayer()
    {
        Vector3 direction;
        Vector3 relativePos;

        direction = bullAI.agent.destination;

        relativePos.x = direction.x - bullAI.transform.position.x;
        relativePos.y = 0;
        relativePos.z = direction.z - bullAI.transform.position.z;

        if (baseMoveBullSO.speedRot < baseMoveBullSO.maxSpeedRot)
            baseMoveBullSO.speedRot += Time.deltaTime / baseMoveBullSO.smoothRot;
        else
        {
            baseMoveBullSO.speedRot = baseMoveBullSO.maxSpeedRot;
        }

        Quaternion rotation = Quaternion.Slerp(bullAI.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), baseMoveBullSO.speedRot);
        bullAI.transform.rotation = rotation;
    }
}