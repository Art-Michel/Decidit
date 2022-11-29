using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitBeforeRushBull
{
    public BullAI bullAI;
    public CoolDownRushBullParameterSO coolDownRushBullSO;

    public void CoolDownBeforeRush()
    {
        bullAI.agent.speed = coolDownRushBullSO.stopSpeed;
        if (coolDownRushBullSO.currentCoolDownRush> 0)
        {
            coolDownRushBullSO.rushDestination = bullAI.playerTransform.position + bullAI.transform.forward * coolDownRushBullSO.rushInertieSetDistance;
            coolDownRushBullSO.currentCoolDownRush -= Time.deltaTime;
        }
        else
        {
            coolDownRushBullSO.currentCoolDownRush = coolDownRushBullSO.coolDownRush;
            bullAI.agent.SetDestination(coolDownRushBullSO.rushDestination);
            bullAI.SwitchToNewState(3);
        }
    }
}