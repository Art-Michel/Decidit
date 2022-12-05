using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitBeforeRushBull
{
    public BullAI bullAI;
    public CoolDownRushBullParameterSO coolDownRushBullSO;

    public void GoToStartRushPos()
    {
        if(coolDownRushBullSO.startPos != Vector3.zero)
        {
            bullAI.agent.speed = coolDownRushBullSO.speedGoToStartPos;
            bullAI.agent.SetDestination(coolDownRushBullSO.startPos);
        }

        if(bullAI.agent.remainingDistance ==0)
        {
            CoolDownBeforeRush();
        }
    }

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
            coolDownRushBullSO.startPos = Vector3.zero;
            bullAI.bullAITeam.ResetSelectedBox(coolDownRushBullSO.boxSelected);
            coolDownRushBullSO.boxSelected = null;
            bullAI.SwitchToNewState(3);
        }
    }
}