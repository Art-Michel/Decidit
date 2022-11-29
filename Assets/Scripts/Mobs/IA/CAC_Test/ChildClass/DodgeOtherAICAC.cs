using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DodgeOtherAICAC : MonoBehaviour
{
    public StateManagerAICAC stateManagerAICAC;
    public DodgeOtherParameterAICAC dodgeOtherSO;

    Vector3 destination;
    Ray ray;
    RaycastHit hit;

    public void SetDodgeDestination()
    {
        stateManagerAICAC.spawnSurroundDodge.LookAt(stateManagerAICAC.playerTransform.position);

        if(!dodgeOtherSO.right && !dodgeOtherSO.left)
        {
            if (Random.Range(0, 2) == 0)
            {
                dodgeOtherSO.right = true;
            }
            else
            {
                dodgeOtherSO.left = true;
            }
        }

        Vector3 dir = stateManagerAICAC.playerTransform.position - stateManagerAICAC.transform.position;

        if (dodgeOtherSO.right)
        {
            Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;

            Vector3 right = -Vector3.Cross(dir, Vector3.up).normalized;
            destination = right + (left + (stateManagerAICAC.spawnSurroundDodge.right + stateManagerAICAC.spawnSurroundDodge.forward));
        }
        else if (dodgeOtherSO.left)
        {
            Vector3 right = -Vector3.Cross(dir, Vector3.up).normalized;

            Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
            destination = left + (right + (-stateManagerAICAC.spawnSurroundDodge.right + stateManagerAICAC.spawnSurroundDodge.forward));
        }

        hit = RaycastAIManager.RaycastAI(stateManagerAICAC.spawnSurroundDodge.position, destination, dodgeOtherSO.mask, Color.red,
            Vector3.Distance(stateManagerAICAC.playerTransform.position, stateManagerAICAC.spawnSurroundDodge.position));

        ray = new Ray(stateManagerAICAC.spawnSurroundDodge.position, destination);
    }

    public void MoveDodge()
    {
        stateManagerAICAC.agent.speed = dodgeOtherSO.speed;

        if (dodgeOtherSO.left)
        {
            stateManagerAICAC.agent.SetDestination(ray.GetPoint(stateManagerAICAC.distplayer));
        }
        else if (dodgeOtherSO.right)
        {
            stateManagerAICAC.agent.SetDestination(ray.GetPoint(stateManagerAICAC.distplayer));
        }

        if (stateManagerAICAC.listOtherAIContact.Count == 0 || stateManagerAICAC.distplayer < 2)
            StopDodge();
    }

    void StopDodge()
    {
        dodgeOtherSO.right = false;
        dodgeOtherSO.left = false;
        stateManagerAICAC.SwitchToNewState(1);
    }
}