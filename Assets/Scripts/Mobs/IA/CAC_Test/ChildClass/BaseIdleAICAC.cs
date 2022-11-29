using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseIdleAICAC
{
    public StateManagerAICAC virtual_AICAC;
    public BaseIdleParameterAICAC baseIdleParameterSO;

    public void StateIdle()
    {
        if (baseIdleParameterSO.currentDelayIdleState > 0)
        {
            baseIdleParameterSO.currentDelayIdleState -= Time.deltaTime;
        }
        else
        {
            baseIdleParameterSO.currentDelayIdleState = baseIdleParameterSO.maxDelayIdleState;

            virtual_AICAC.SwitchToNewState(1);
        }
    }
}
