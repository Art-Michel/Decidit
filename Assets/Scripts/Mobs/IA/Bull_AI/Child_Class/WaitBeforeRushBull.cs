using UnityEngine;

public class WaitBeforeRushBull
{
    public BullAI bullAI;
    public CoolDownRushBullParameterSO coolDownRushBullSO;

    public void GoToStartRushPos()
    {
        CoolDownBeforeRush();

        bullAI.material_Instances.material.color = bullAI.material_Instances.colorPreAtatck;
        bullAI.material_Instances.ChangeColorTexture(bullAI.material_Instances.colorPreAtatck);

        if (coolDownRushBullSO.startPos == Vector3.zero)
        {
            SelectStartPos();
        }
        else if(bullAI.agent.remainingDistance <= 0.5f)
        {
            bullAI.agent.SetDestination(coolDownRushBullSO.startPos);
        }

        if(coolDownRushBullSO.startPos != Vector3.zero && bullAI.agent.remainingDistance <= 0.5f)
        {
            SelectStartPos();
        }
        SmoothLookAtPlayer();
    }

    void CoolDownBeforeRush()
    {
        if (coolDownRushBullSO.currentCoolDownRush> 0)
        {
            coolDownRushBullSO.currentCoolDownRush -= Time.deltaTime;
        }
        else
        {
            bullAI.agent.speed = coolDownRushBullSO.stopSpeed;
            coolDownRushBullSO.rushDestination = bullAI.playerTransform.position + bullAI.transform.forward * coolDownRushBullSO.rushInertieSetDistance;
            coolDownRushBullSO.currentCoolDownRush = coolDownRushBullSO.coolDownRush;
            bullAI.agent.SetDestination(coolDownRushBullSO.rushDestination);
            coolDownRushBullSO.startPos = Vector3.zero;
            bullAI.bullAIStartPosRush.ResetSelectedBox(coolDownRushBullSO.boxSelected);
            coolDownRushBullSO.boxSelected = null;
            coolDownRushBullSO.speedRot = 0;
            bullAI.material_Instances.material.color = bullAI.material_Instances.color;
            bullAI.material_Instances.ChangeColorTexture(bullAI.material_Instances.color);
            bullAI.SwitchToNewState(3);
        }
    }

    void SelectStartPos()
    {
        bullAI.bullAIStartPosRush.SelectAI(bullAI);
        bullAI.agent.speed = coolDownRushBullSO.speedGoToStartPos;
        bullAI.agent.SetDestination(coolDownRushBullSO.startPos);
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