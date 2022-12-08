using UnityEngine;

public class BaseIdleAICAC
{
    public StateManagerAICAC stateManagerAICAC;
    public BaseIdleParameterAICAC baseIdleParameterSO;

    public void StateIdle()
    {
        if (baseIdleParameterSO.currentDelayIdleState > 0)
        {
            stateManagerAICAC.agent.speed = 0;
            baseIdleParameterSO.currentDelayIdleState -= Time.deltaTime;
        }
        else
        {
            baseIdleParameterSO.currentDelayIdleState = baseIdleParameterSO.maxDelayIdleState;

            stateManagerAICAC.SwitchToNewState(1);
        }
    }
}