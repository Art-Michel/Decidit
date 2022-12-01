using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMoveWallAI
{
    public WallAI wallAI;
    public BaseMoveWallAISO baseMoveWallAISO;
    RaycastHit hit;

    public void MoveAI()
    {
        wallAI.agent.SetDestination(baseMoveWallAISO.newPos);
        wallAI.agent.speed = baseMoveWallAISO.speedMovement;

        if (!wallAI.agent.isOnOffMeshLink)
        {
            WallCrackEffect();
        }

        if (!IsMoving())
            baseMoveWallAISO.findNewPos = false;

        LaunchDelayBeforeAttack();
    }
    bool IsMoving()
    {
        if (wallAI.agent.remainingDistance == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void SelectNewPos()
    {
        if (!IsMoving())
        {
            baseMoveWallAISO.selectedWall = Random.Range(0, 4);
            Debug.Log(baseMoveWallAISO.selectedWall);
            baseMoveWallAISO.newPos = SearchNewPos(wallAI.walls[baseMoveWallAISO.selectedWall].bounds);

            hit = RaycastAIManager.RaycastAI(baseMoveWallAISO.newPos, wallAI.playerTransform.position - baseMoveWallAISO.newPos, baseMoveWallAISO.mask, 
                Color.blue, Vector3.Distance(baseMoveWallAISO.newPos, wallAI.playerTransform.position));

            if (hit.transform != wallAI.playerTransform)
            {
                baseMoveWallAISO.findNewPos = false;
            }
            else
            {
                baseMoveWallAISO.findNewPos = true;
            }
        }
    }
    Vector3 SearchNewPos(Bounds bounds)
    {
        return new Vector3(
           Random.Range(bounds.min.x, bounds.max.x),
           Random.Range(bounds.min.y, bounds.max.y),
           Random.Range(bounds.min.z, bounds.max.z)
       );
    }
    public void WallCrackEffect()
    {
        /*baseMoveWallAISO.distSinceLast = Vector3.Distance(wallAI.transform.parent.position, baseMoveWallAISO.lastWallCrack.transform.position);

        wallAI.orientation = wallAI.walls[baseMoveWallAISO.selectedWall].transform.localEulerAngles.y-90;

        if (baseMoveWallAISO.distSinceLast >= baseMoveWallAISO.decalage)
        {
            baseMoveWallAISO.lastWallCrack = MonoBehaviour.Instantiate(baseMoveWallAISO.wallCrackPrefab, wallAI.transform.parent.position, Quaternion.Euler(0, wallAI.orientation, 0));
        }*/
    }

    void LaunchDelayBeforeAttack()
    {
        if (baseMoveWallAISO.rateAttack > 0)
        {
            baseMoveWallAISO.rateAttack -= Time.deltaTime;
        }
        else
        {
            if (!wallAI.agent.isOnOffMeshLink)
            {
                baseMoveWallAISO.rateAttack = baseMoveWallAISO.maxRateAttack;
                baseMoveWallAISO.findNewPos = false;
                wallAI.agent.SetDestination(wallAI.transform.position);
                wallAI.SwitchToNewState(1);
            }
        }
    }
}