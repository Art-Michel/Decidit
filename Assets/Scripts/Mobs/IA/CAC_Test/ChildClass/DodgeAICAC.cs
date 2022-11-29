using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DodgeAICAC
{
    public StateManagerAICAC stateManagerAICAC;
    public DodgeParameterAICAC dodgeAICACSO;

    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0f)
        {
            return 1.0f;
        }
        else if (dir < 0.0f)
        {
            return -1.0f;
        }
        else
        {
            return 0.0f;
        }
    }

    public void SetDodgePosition()
    {
        if(!dodgeAICACSO.leftDodge && !dodgeAICACSO.rightDodge)
        {
            if (Random.Range(0, 2) == 0)
            {
                dodgeAICACSO.leftDodge = false;
                dodgeAICACSO.rightDodge = true;
            }
            else
            {
                dodgeAICACSO.rightDodge = false;
                dodgeAICACSO.leftDodge = true;
            }
        }

        if (dodgeAICACSO.rightDodge) // choisi l esquive par la droite 
        {
            Vector3 dir = dodgeAICACSO.targetObjectToDodge.position - stateManagerAICAC.transform.position;
            Vector3 right = -Vector3.Cross(dir, Vector3.up).normalized;

            Ray ray = new Ray(stateManagerAICAC.spawnRayDodge.position, right);
            dodgeAICACSO.targetDodgeVector = ray.GetPoint(dodgeAICACSO.dodgeLenght);
            dodgeAICACSO.dodgeIsSet = true;
        }
        if (dodgeAICACSO.leftDodge) // choisi l esquive par la gauche 
        {
            Vector3 dir = dodgeAICACSO.targetObjectToDodge.position - stateManagerAICAC.transform.position;
            Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;

            Ray ray = new Ray(stateManagerAICAC.spawnRayDodge.position, left);
            dodgeAICACSO.targetDodgeVector = ray.GetPoint(dodgeAICACSO.dodgeLenght);
            dodgeAICACSO.dodgeIsSet = true;
        }
    }

    // fonction qui renvoie vrai si le point "dodgePoint" se trouve sur le NavMesh et faux si ce n est pas le cas
    bool DetectDodgePointIsOnNavMesh(Vector3 dodgePoint)
    {
        if (NavMesh.SamplePosition(dodgePoint, out dodgeAICACSO.navHit, 0.1f, NavMesh.AllAreas))
        {
            Debug.Log("point on nav mesh");
            return true;
        }
        else
        {
            Debug.Log("point out nav mesh");
            return false;
        }
    }

    // Apply Dodge movement
    public void Dodge()
    {
        float distDodgeDestination;
        Vector2 pos2DAI;
        Vector2 posDodgeDestination2D;

        pos2DAI.x = stateManagerAICAC.transform.position.x;
        pos2DAI.y = stateManagerAICAC.transform.position.z;

        posDodgeDestination2D.x = dodgeAICACSO.targetDodgeVector.x;
        posDodgeDestination2D.y = dodgeAICACSO.targetDodgeVector.z;

        distDodgeDestination = Vector2.Distance(pos2DAI, posDodgeDestination2D);

        //Debug.Log(distDodgeDestination);
        if (distDodgeDestination > 1.1f)
        {
            stateManagerAICAC.agent.speed = dodgeAICACSO.dodgeSpeed;
            dodgeAICACSO.isDodging = true;
            stateManagerAICAC.agent.SetDestination(dodgeAICACSO.targetDodgeVector);
        }
        else
        {
            StopDodge();
        }

        if(stateManagerAICAC.agent.velocity.magnitude == 0f)
        {
            StopDodge();
        }
    }
    void StopDodge()
    {
        dodgeAICACSO.leftDodge = false;
        dodgeAICACSO.rightDodge = false;
        dodgeAICACSO.isDodging = false;
        Debug.Log("Stop dodge");
        dodgeAICACSO.dodgeIsSet = false;

        if(!dodgeAICACSO.dodgeRushBull)
            stateManagerAICAC.SwitchToNewState(0);
        else
        {
            dodgeAICACSO.dodgeRushBull = false;
            //stateManagerAICAC.SwitchToNewState(5);
        }
    }
}