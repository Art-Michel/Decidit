using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurroundAICAC
{
    public StateManagerAICAC stateManagerAICAC;
    public SurroundParameterAICAC SurroundAICACSO;
    public AICACVarianteState aICACVarianteState;

    public List<SurroundParameterAICAC> listSurrounScript = new List<SurroundParameterAICAC>();

    Vector3 destination;
    Ray ray;
    RaycastHit hit;
    public void ChooseDirection()
    {
        stateManagerAICAC.spawnSurroundDodge.LookAt(stateManagerAICAC.playerTransform.position);

        if (!SurroundAICACSO.left && !SurroundAICACSO.right)
        {
            hit = RaycastAIManager.RaycastAI(stateManagerAICAC.spawnSurroundDodge.position,
            stateManagerAICAC.playerTransform.position - stateManagerAICAC.spawnSurroundDodge.position, SurroundAICACSO.mask, Color.red, 100f);
            float angle;
            angle = Vector3.SignedAngle(stateManagerAICAC.playerTransform.forward, stateManagerAICAC.transform.forward, Vector3.up);

            if (angle > 0)
            {
                SurroundAICACSO.left = true;
            }
            else
            {
                SurroundAICACSO.right = true;
            }
        }
        else
        {
            MoveSurround();
        }
    }

    public void GetSurroundDestination()
    {
        Vector3 dir = stateManagerAICAC.playerTransform.position - stateManagerAICAC.transform.position;

        if(SurroundAICACSO.right)
        {
            Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
            
            Vector3 right = -Vector3.Cross(dir, Vector3.up).normalized;
            destination = right + (left + (stateManagerAICAC.spawnSurroundDodge.right + stateManagerAICAC.spawnSurroundDodge.forward));
        }
        else if (SurroundAICACSO.left)
        {
            Vector3 right = -Vector3.Cross(dir, Vector3.up).normalized;

            Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
            destination = left + (right + (-stateManagerAICAC.spawnSurroundDodge.right + stateManagerAICAC.spawnSurroundDodge.forward));
        }

        hit = RaycastAIManager.RaycastAI(stateManagerAICAC.spawnSurroundDodge.position, destination, SurroundAICACSO.mask, Color.red, 
            Vector3.Distance(stateManagerAICAC.playerTransform.position, stateManagerAICAC.spawnSurroundDodge.position));

        ray = new Ray(stateManagerAICAC.spawnSurroundDodge.position, destination);
    }

    public void MoveSurround()
    {
        //stateManagerAICAC.agent.speed = SurroundAICACSO.surroundSpeed;

        if(stateManagerAICAC.agent.speed < SurroundAICACSO.surroundSpeed)
            stateManagerAICAC.agent.speed += SurroundAICACSO.speedSmooth * Time.deltaTime;
        else
            stateManagerAICAC.agent.speed = SurroundAICACSO.surroundSpeed;

        if(stateManagerAICAC.distplayer > SurroundAICACSO.stopSurroundDistance+2)
        {
            if (SurroundAICACSO.left || SurroundAICACSO.right)
            {
                stateManagerAICAC.agent.SetDestination(ray.GetPoint(stateManagerAICAC.distplayer));
            }
        }
        else
        {
            if (SurroundAICACSO.left || SurroundAICACSO.right)
            {
                stateManagerAICAC.agent.SetDestination(stateManagerAICAC.playerTransform.position);
            }
        }

        if(stateManagerAICAC.distplayer <= SurroundAICACSO.stopSurroundDistance)
        {
            StopSurround();
        }
    }

    void StopSurround()
    {
        SurroundAICACSO.right = false;
        SurroundAICACSO.left = false;
        aICACVarianteState.RemoveAISelected(stateManagerAICAC);
        stateManagerAICAC.SwitchToNewState(1);
    }
}