using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMoveFly
{
    public FlyAINavMesh flyAINavMesh;
    public BaseMoveFlySO baseMoveFlySO;
    public BaseAttackFlySO baseAttackFlySO;

    public void SmoothLookAtYAxisPatrol()
    {
        Vector3 relativePos;

        relativePos.x = baseMoveFlySO.destinationFinal.x - flyAINavMesh.transform.position.x;
        relativePos.y = baseMoveFlySO.destinationFinal.y - flyAINavMesh.transform.position.y;
        relativePos.z = baseMoveFlySO.destinationFinal.z - flyAINavMesh.transform.position.z;

        Quaternion rotation = Quaternion.Slerp(flyAINavMesh.childObj.localRotation, Quaternion.LookRotation(relativePos, Vector3.up), baseMoveFlySO.speedRotationAIPatrol);
        flyAINavMesh.childObj.localRotation = rotation;

        if (baseMoveFlySO.speedRotationAIPatrol < baseMoveFlySO.maxSpeedRotationAIPatrol)
        {
            baseMoveFlySO.speedRotationAIPatrol += (Time.deltaTime / baseMoveFlySO.smoothRotationPatrol);
            baseMoveFlySO.lerpSpeedYValuePatrol += (Time.deltaTime / baseMoveFlySO.ySpeedSmootherPatrol);
        }

        ApplyFlyingMove();
        DelayBeforeAttack();
    }
    ////////////// Set Destination \\\\\\\\\\\\\\\\\\\\\
    Vector3 SearchNewPos() // défini la position aléatoire choisi dans la fonction "RandomPointInBounds()" si la distance entre le point et l'IA est suffisament grande
    {
        Debug.Log("searchPos");

        if (baseMoveFlySO.distDestinationFinal < 20)
        {
            baseMoveFlySO.destinationFinal = RandomPointInBounds(flyAINavMesh.myCollider.bounds);

            baseMoveFlySO.newPosIsSet = false;
            baseMoveFlySO.speedRotationAIPatrol = 0;
            baseMoveFlySO.currentSpeedYPatrol = 0;
            baseMoveFlySO.lerpSpeedYValuePatrol = 0;
            return baseMoveFlySO.newPos = new Vector3(baseMoveFlySO.destinationFinal.x, baseMoveFlySO.destinationFinal.z);
        }
        else
        {
            Debug.Log("pos is find");
            baseMoveFlySO.newPosIsSet = true;
            baseMoveFlySO.speedRotationAIPatrol = 0;
            baseMoveFlySO.currentSpeedYPatrol = 0;
            baseMoveFlySO.lerpSpeedYValuePatrol = 0;

            Vector3 destinationFinal2D = new Vector2(baseMoveFlySO.destinationFinal.x, baseMoveFlySO.destinationFinal.z);
            Vector3 transformPos2D = new Vector2(flyAINavMesh.transform.position.x, flyAINavMesh.transform.position.z);

            baseMoveFlySO.timeGoToDestinationPatrol = Vector3.Distance(destinationFinal2D, transformPos2D) / flyAINavMesh.agent.speed;
            baseMoveFlySO.maxSpeedYTranslationPatrol = Mathf.Abs(baseMoveFlySO.destinationFinal.y - flyAINavMesh.transform.position.y) / baseMoveFlySO.timeGoToDestinationPatrol;

            return baseMoveFlySO.newPos = new Vector3(baseMoveFlySO.destinationFinal.x, baseMoveFlySO.destinationFinal.z);
        }
    }
    Vector3 RandomPointInBounds(Bounds bounds) // renvoie une position aléatoire dans un trigger collider
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    void ApplyFlyingMove()
    {
        baseMoveFlySO.distDestinationFinal = Vector3.Distance(flyAINavMesh.transform.position, baseMoveFlySO.destinationFinal);
        flyAINavMesh.agent.speed = baseMoveFlySO.baseMoveSpeed;

        if (baseMoveFlySO.newPosIsSet)
        {
            if (baseMoveFlySO.distDestinationFinal > 7)
            {
                flyAINavMesh.agent.SetDestination(new Vector3(flyAINavMesh.transform.position.x, 0, flyAINavMesh.transform.position.z) + flyAINavMesh.childObj.TransformDirection(Vector3.forward));
                baseMoveFlySO.currentSpeedYPatrol = Mathf.Lerp(baseMoveFlySO.currentSpeedYPatrol, baseMoveFlySO.maxSpeedYTranslationPatrol, baseMoveFlySO.lerpSpeedYValuePatrol);

                if (Mathf.Abs(flyAINavMesh.transform.position.y - baseMoveFlySO.destinationFinal.y) > 1)
                {
                    if (flyAINavMesh.transform.position.y < baseMoveFlySO.destinationFinal.y)
                    {
                        flyAINavMesh.agent.baseOffset += baseMoveFlySO.currentSpeedYPatrol * Time.deltaTime;
                    }
                    else
                    {
                        flyAINavMesh.agent.baseOffset -= baseMoveFlySO.currentSpeedYPatrol * Time.deltaTime;
                    }
                }
            }
            else
                baseMoveFlySO.newPosIsSet = false;
        }
        else
            SearchNewPos();
    }

    void ResetParameter()
    {
        baseMoveFlySO.timeGoToDestinationPatrol = 0;
        baseMoveFlySO.maxSpeedYTranslationPatrol = 0;
        baseMoveFlySO.currentSpeedYPatrol = 0;
        baseMoveFlySO.lerpSpeedYValuePatrol = 0;
        baseMoveFlySO.speedRotationAIPatrol = 0;


        baseAttackFlySO.speedRotationAIAttack = 0;
        baseAttackFlySO.currentSpeedYAttack = 0;
        baseAttackFlySO.lerpSpeedYValueAttack = 0;

        baseMoveFlySO.newPosIsSet = false;
    }

    void DelayBeforeAttack()
    {
        if (baseMoveFlySO.currentRateAttack > 0)
        {
            baseMoveFlySO.currentRateAttack -= Time.deltaTime;
        }
        else
        {
            baseMoveFlySO.currentRateAttack = baseMoveFlySO.maxRateAttack;
            ResetParameter();
            flyAINavMesh.SwitchToNewState(1);
        }
    }

    public void ForceLaunchAttack()
    {
        baseMoveFlySO.currentRateAttack =0;
    }
}