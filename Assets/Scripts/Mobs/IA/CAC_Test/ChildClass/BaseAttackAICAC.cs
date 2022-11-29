using System.Collections;
using System.Collections.Generic;
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
}
