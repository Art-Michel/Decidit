using UnityEngine;

public class BaseAttackAICAC
{
    public StateManagerAICAC stateManagerAICAC;
    public BaseAttackParameterAICAC baseAttackParameterSO;

    public void BaseAttack(Animator animator)
    {
        stateManagerAICAC.agent.speed = 0;

        if (stateManagerAICAC.distplayer > baseAttackParameterSO.attackRange && !baseAttackParameterSO.isAttacking)
        {
            baseAttackParameterSO.currentAttackRate = 0;
            baseAttackParameterSO.speedRot = 0;
            stateManagerAICAC.SwitchToNewState(1);
        }
        else
        {
            if (baseAttackParameterSO.currentAttackRate <= 0)
            {
                animator.SetBool("Attack", true);
                baseAttackParameterSO.isAttacking = true;
                baseAttackParameterSO.currentAttackRate = baseAttackParameterSO.maxAttackRate;
            }
            else if (!baseAttackParameterSO.isAttacking)
            {
                baseAttackParameterSO.currentAttackRate -= Time.deltaTime;
            }
        }
    }

    public void SmoothLookAt()
    {
        Vector3 direction;
        Vector3 relativePos;

        direction = stateManagerAICAC.playerTransform.position;
        relativePos.x = direction.x - stateManagerAICAC.transform.position.x;
        relativePos.y = 0;
        relativePos.z = direction.z - stateManagerAICAC.transform.position.z;

        if (baseAttackParameterSO.speedRot < baseAttackParameterSO.maxSpeedRot)
            baseAttackParameterSO.speedRot += Time.deltaTime / baseAttackParameterSO.smoothRot;
        else
        {
            baseAttackParameterSO.speedRot = baseAttackParameterSO.maxSpeedRot;
        }

        Quaternion rotation = Quaternion.Slerp(stateManagerAICAC.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), baseAttackParameterSO.speedRot);
        stateManagerAICAC.transform.rotation = rotation;
    }
}
