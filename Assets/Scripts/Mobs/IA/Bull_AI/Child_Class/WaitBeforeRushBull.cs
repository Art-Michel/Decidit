using UnityEngine;

public class WaitBeforeRushBull
{
    public BullAI bullAI;
    public CoolDownRushBullParameterSO coolDownRushBullSO;

    public void GoToStartRushPos()
    {
        if (coolDownRushBullSO.startPos == Vector3.zero)
        {
            bullAI.bullAIStartPosRush.SelectAI(bullAI);
            bullAI.agent.speed = coolDownRushBullSO.speedGoToStartPos;
            bullAI.agent.SetDestination(coolDownRushBullSO.startPos);
        }
        else if(bullAI.agent.remainingDistance ==0)
        {
            bullAI.agent.SetDestination(coolDownRushBullSO.startPos);
            CoolDownBeforeRush();
        }

        SmoothLookAtPlayer();
    }

    void CoolDownBeforeRush()
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
            bullAI.bullAIStartPosRush.ResetSelectedBox(coolDownRushBullSO.boxSelected);
            coolDownRushBullSO.boxSelected = null;
            coolDownRushBullSO.speedRot = 0;
            bullAI.SwitchToNewState(3);
        }
    }

    void SmoothLookAtPlayer()
    {
        Vector3 direction;
        Vector3 relativePos;
        
        if(bullAI.agent.remainingDistance != 0)
            direction = bullAI.agent.destination;
        else
            direction = bullAI.playerTransform.position;

        relativePos.x = direction.x - bullAI.transform.position.x;
        relativePos.y = 0;
        relativePos.z = direction.z - bullAI.transform.position.z;

        if (coolDownRushBullSO.speedRot < coolDownRushBullSO.maxSpeedRot)
            coolDownRushBullSO.speedRot += Time.deltaTime / coolDownRushBullSO.smoothRot;
        else
        {
            coolDownRushBullSO.speedRot = coolDownRushBullSO.maxSpeedRot;
        }

        Quaternion rotation = Quaternion.Slerp(bullAI.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), coolDownRushBullSO.speedRot);
        bullAI.transform.rotation = rotation;
    }
}