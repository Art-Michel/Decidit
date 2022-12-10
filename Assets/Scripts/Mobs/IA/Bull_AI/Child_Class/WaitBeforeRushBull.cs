using UnityEngine;

public class WaitBeforeRushBull
{
    public BullAI bullAI;
    public CoolDownRushBullParameterSO coolDownRushBullSO;

    RaycastHit hit;

    public void GoToStartRushPos()
    {
        CoolDownBeforeRush();

        if (coolDownRushBullSO.startPos == Vector3.zero) // cherche une nouvelle position si aucune n est defifni
        {
            SelectStartPos();
        }
        else if (bullAI.agent.remainingDistance > 0.5f) // avance vers la position
        {
            bullAI.agent.SetDestination(coolDownRushBullSO.startPos);
        }

        if (coolDownRushBullSO.startPos != Vector3.zero && bullAI.agent.remainingDistance <= 1f)
        {
            if(coolDownRushBullSO.currentDurationStay >0)
            {
                coolDownRushBullSO.currentDurationStay -= Time.deltaTime;
            }
            else
            {
                coolDownRushBullSO.currentDurationStay = coolDownRushBullSO.maxDurationStay;
                bullAI.bullAIStartPosRush.ResetSelectedBox(coolDownRushBullSO.boxSelected);
                coolDownRushBullSO.boxSelected = null;
                SelectStartPos();
            }
        }
        SmoothLookAtPlayer();
    }

    void SelectStartPos()
    {
        bullAI.bullAIStartPosRush.SelectAI(bullAI);
        bullAI.agent.speed = coolDownRushBullSO.speedPatrolToStartPos;
        bullAI.agent.SetDestination(coolDownRushBullSO.startPos);
    }

    public void CheckObstacle()
    {
        if(coolDownRushBullSO.currentCoolDownCheckObstacle <=0)
        {
            hit = RaycastAIManager.RaycastAI(bullAI.transform.position, bullAI.transform.forward, coolDownRushBullSO.mask, Color.red, coolDownRushBullSO.distDetect);

            if (hit.transform != null)
            {
                coolDownRushBullSO.currentCoolDownCheckObstacle = coolDownRushBullSO.maxCoolDownCheckObstacle;
                SelectStartPos();
            }
        }
        else
        {
            coolDownRushBullSO.currentCoolDownCheckObstacle -= Time.fixedDeltaTime;
        }
    }

    void CoolDownBeforeRush()
    {
        if (coolDownRushBullSO.currentCoolDownRush > 0)
        {
            coolDownRushBullSO.currentCoolDownRush -= Time.deltaTime;
            if (coolDownRushBullSO.currentCoolDownRush <3f)
                ShowSoonAttack();
        }
        else if(bullAI.agent.remainingDistance <= 1f)
        {
            ResetParameter(); // launch charge and reset parameter
        }
    }
    void ResetParameter()
    {
        bullAI.agent.speed = coolDownRushBullSO.stopSpeed;
        coolDownRushBullSO.rushDestination = bullAI.playerTransform.position + bullAI.transform.forward * coolDownRushBullSO.rushInertieSetDistance;
        coolDownRushBullSO.currentCoolDownRush = coolDownRushBullSO.coolDownRush;
        bullAI.agent.SetDestination(coolDownRushBullSO.rushDestination);
        coolDownRushBullSO.startPos = Vector3.zero;
        bullAI.bullAIStartPosRush.ResetSelectedBox(coolDownRushBullSO.boxSelected);
        coolDownRushBullSO.boxSelected = null;
        coolDownRushBullSO.speedRot = 0;
        bullAI.material_Instances.Material.color = bullAI.material_Instances.Color;
        bullAI.material_Instances.ChangeColorTexture(bullAI.material_Instances.Color);
        coolDownRushBullSO.currentDurationStay = coolDownRushBullSO.maxDurationStay;
        bullAI.SwitchToNewState(3);
    }
    void ShowSoonAttack()
    {
        bullAI.material_Instances.Material.color = bullAI.material_Instances.ColorPreAtatck;
        bullAI.material_Instances.ChangeColorTexture(bullAI.material_Instances.ColorPreAtatck);
        bullAI.agent.speed = coolDownRushBullSO.speedRushToStartPos;
    }

    void SmoothLookAtPlayer()
    {
        Vector3 direction;
        Vector3 relativePos;

        if (bullAI.agent.remainingDistance > 1)
            direction = bullAI.agent.destination;
        else
            direction = bullAI.playerTransform.position;

        //direction = bullAI.playerTransform.position;

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