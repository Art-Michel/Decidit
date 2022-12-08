using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseIdleBull
{
    public BullAI bullAI;
    public BaseIdleBullSO baseIdleBullSO;

    public void BaseIdle() // Switch state BaseIdle To BaseMovement and restart RushMovementVariable
    {
        bullAI.agent.speed = baseIdleBullSO.stopSpeed;

        if (baseIdleBullSO.currentTransition > 0)
            baseIdleBullSO.currentTransition -= Time.deltaTime;
        else
        {
            baseIdleBullSO.currentTransition = baseIdleBullSO.transitionDurationMax;
            bullAI.SwitchToNewState(2);
        }
    }
}
