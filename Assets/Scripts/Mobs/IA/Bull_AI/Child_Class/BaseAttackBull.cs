using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackBull
{
    public BullAI bullAI;
    public BaseAttackBullSO baseAttackBullSO;

    public void BaseAttackTimer()
    {
        bullAI.agent.speed = 0;

        if (bullAI.distPlayer > baseAttackBullSO.attackRange)
        {
            bullAI.SwitchToNewState(1);
        }
        else
        {
            if (baseAttackBullSO.currentAttackRate <= 0)
            {
                bullAI.colliderBaseAttack.gameObject.SetActive(true);
                bullAI.attackCollider.enabled = true;
            }
            else
            {
                baseAttackBullSO.currentAttackRate -= Time.deltaTime;
            }
        }
    }
    public void LaunchBaseAttack(bool touchPlayer)
    {
        if (touchPlayer)
        {
            baseAttackBullSO.currentAttackRate = baseAttackBullSO.maxAttackRate;
            bullAI.colliderBaseAttack.gameObject.SetActive(false);
            bullAI.attackCollider.enabled = false;
        }
        else
        {
            baseAttackBullSO.currentAttackRate = baseAttackBullSO.maxAttackRate;
            bullAI.colliderBaseAttack.gameObject.SetActive(false);
            bullAI.attackCollider.enabled = false;
        }
    }
}