using UnityEngine;

public class BaseAttackBull
{
    public BullAI bullAI;
    public BaseAttackBullSO baseAttackBullSO;

    public void BaseAttackTimer()
    {
        bullAI.agent.speed = 0;
        SmoothLookAtPlayer();

        if (bullAI.distPlayer > baseAttackBullSO.attackRange)
        {
            bullAI.SwitchToNewState(1);
        }
        else
        {
            if (baseAttackBullSO.currentAttackRate <= 0)
            {
                //bullAI.colliderBaseAttack.gameObject.SetActive(true);
                Debug.Log("Attack");
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
            //bullAI.colliderBaseAttack.gameObject.SetActive(false);
        }
        else
        {
            baseAttackBullSO.currentAttackRate = baseAttackBullSO.maxAttackRate;
            //bullAI.colliderBaseAttack.gameObject.SetActive(false);
        }
    }

    void SmoothLookAtPlayer()
    {
        Vector3 direction;
        Vector3 relativePos;

        direction = bullAI.playerTransform.position;

        relativePos.x = direction.x - bullAI.transform.position.x;
        relativePos.y = 0;
        relativePos.z = direction.z - bullAI.transform.position.z;

        if (baseAttackBullSO.speedRot < baseAttackBullSO.maxSpeedRot)
            baseAttackBullSO.speedRot += Time.deltaTime / baseAttackBullSO.smoothRot;
        else
        {
            baseAttackBullSO.speedRot = baseAttackBullSO.maxSpeedRot;
        }

        Quaternion rotation = Quaternion.Slerp(bullAI.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), baseAttackBullSO.speedRot);
        bullAI.transform.rotation = rotation;
    }
}