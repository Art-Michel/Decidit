using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMoveBull
{
    public BullAI bullAI;
    public BaseMoveBullParameterSO baseMoveBullSO;

    public void BaseMovement()
    {
        bullAI.agent.speed = baseMoveBullSO.baseSpeed;
        bullAI.agent.SetDestination(bullAI.playerTransform.position);

        if (bullAI.distPlayer < baseMoveBullSO.attackRange)
        {
            bullAI.SwitchToNewState(4);
        }
    }
    public void CoolDownRush()
    {
        if (baseMoveBullSO.currentCoolDownWaitRush > 0)
        {
            baseMoveBullSO.currentCoolDownWaitRush -= Time.deltaTime;
        }
        else
        {
            baseMoveBullSO.currentCoolDownWaitRush = baseMoveBullSO.maxCoolDownRush;
            bullAI.SwitchToNewState(2);
        }
    }
}