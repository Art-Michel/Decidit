using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushBull
{
    public BullAI bullAI;
    public RushBullParameterSO rushBullSO;
    RaycastHit hit;

    public void RushMovement()
    {
        Debug.Log("RushState");
        if (bullAI.distPlayer < 5f)
        {
            rushBullSO.stopLockPlayer = true;
        }
        else if (!rushBullSO.stopLockPlayer)
        {
            rushBullSO.rushDestination = bullAI.playerTransform.position + bullAI.transform.forward * rushBullSO.rushInertieSetDistance;
        }

        bullAI.colliderRush.gameObject.SetActive(true);
        bullAI.detectOtherAICollider.enabled = true;
        bullAI.agent.speed = rushBullSO.rushSpeed;
        bullAI.agent.SetDestination(rushBullSO.rushDestination);

        if (bullAI.agent.remainingDistance == 0)
        {
            StopRush();
        }
    }

    public void CheckObstacleOnPath()
    {
        hit = RaycastAIManager.RaycastAI(bullAI.transform.position, bullAI.transform.forward, rushBullSO.mask, Color.red, 2f);

        if (hit.transform != null)
        {
            Debug.Log("Obstacle Stop Rush");
            StopRush();
        }
    }

    void StopRush()
    {
        bullAI.colliderRush.gameObject.SetActive(false);
        bullAI.detectOtherAICollider.enabled = false;
        rushBullSO.stopLockPlayer = false;

        bullAI.SwitchToNewState(0);
    }
}