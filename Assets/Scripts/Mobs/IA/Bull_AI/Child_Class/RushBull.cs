using UnityEngine;
using UnityEngine.AI;

public class RushBull
{
    public BullAI bullAI;
    public RushBullParameterSO rushBullSO;
    RaycastHit hit;

    public void RushMovement()
    {
        SmoothLookAtPlayer();
        if (bullAI.distPlayer < rushBullSO.stopLockDist)
        {
            rushBullSO.stopLockPlayer = true;
        }
        else if (!rushBullSO.stopLockPlayer)
        {
            rushBullSO.rushDestination = bullAI.playerTransform.position + bullAI.transform.forward * rushBullSO.rushInertieSetDistance;
        }

        //bullAI.colliderRush.gameObject.SetActive(true);
        bullAI.detectOtherAICollider.enabled = true;
        bullAI.agent.speed = rushBullSO.rushSpeed;
        bullAI.agent.SetDestination(CheckNavMeshPoint(rushBullSO.rushDestination));

        if (bullAI.agent.remainingDistance == 0)
        {
            StopRush();
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
        //bullAI.colliderRush.gameObject.SetActive(false);
        bullAI.detectOtherAICollider.enabled = false;
        rushBullSO.stopLockPlayer = false;

        rushBullSO.speedRot = 0;
        bullAI.SwitchToNewState(0);
    }

    void SmoothLookAtPlayer()
    {
        Vector3 direction;
        Vector3 relativePos;

        direction = bullAI.agent.destination;

        relativePos.x = direction.x - bullAI.transform.position.x;
        relativePos.y = 0;
        relativePos.z = direction.z - bullAI.transform.position.z;

        if (rushBullSO.speedRot < rushBullSO.maxSpeedRot)
            rushBullSO.speedRot += Time.deltaTime / rushBullSO.smoothRot;
        else
        {
            rushBullSO.speedRot = rushBullSO.maxSpeedRot;
        }

        Quaternion rotation = Quaternion.Slerp(bullAI.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), rushBullSO.speedRot);
        bullAI.transform.rotation = rotation;
    }
}