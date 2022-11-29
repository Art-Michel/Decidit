using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackWallAI
{
    public WallAI wallAI;
    public BaseAttackWallAISO baseAttackWallAISO;

    public void LaunchAttack()
    {
        wallAI.agent.speed = baseAttackWallAISO.stopSpeed;
        wallAI.animator.SetBool("LaunchAttack", true);
    }

    public float CalculateSpeedProjectile()
    {
        // v= d/t;
        // t = d/v;
        // d = v*t;

        Vector3 directionPlayer;
        directionPlayer.x = wallAI.playerController.controller.velocity.normalized.x;
        directionPlayer.y = wallAI.playerController.controller.velocity.normalized.y;
        directionPlayer.z = wallAI.playerController.controller.velocity.normalized.z;

        switch (wallAI.playerController.localVelocity)
        {
            case 0:
                baseAttackWallAISO.playerPredicDir = wallAI.playerTransform.position;
                baseAttackWallAISO.vPlayer = wallAI.playerController.playerSpeed;
                baseAttackWallAISO.vProjectileGotToPredicPos = baseAttackWallAISO.defaultForceBullet;
                break;
            default:
                baseAttackWallAISO.playerPredicDir = wallAI.playerTransform.position + (directionPlayer * baseAttackWallAISO.distAnticip);
                baseAttackWallAISO.vPlayer = wallAI.playerController.playerSpeed;

                baseAttackWallAISO.timePlayerGoToPredicPos = Vector3.Distance(wallAI.playerTransform.position, baseAttackWallAISO.playerPredicDir) / baseAttackWallAISO.vPlayer;
                baseAttackWallAISO.vProjectileGotToPredicPos = Vector3.Distance(wallAI.transform.parent.position, baseAttackWallAISO.playerPredicDir) / baseAttackWallAISO.timePlayerGoToPredicPos;
                break;
        }
        return baseAttackWallAISO.vProjectileGotToPredicPos;
    }

    public void ThrowProjectile()
    {
        wallAI.spawnBullet.LookAt(baseAttackWallAISO.playerPredicDir);
        Rigidbody cloneBullet = MonoBehaviour.Instantiate(baseAttackWallAISO.bulletPrefab, wallAI.spawnBullet.position, wallAI.spawnBullet.rotation);
        cloneBullet.AddRelativeForce(Vector3.forward * CalculateSpeedProjectile(), ForceMode.VelocityChange);
    }

    public void ReturnBaseMoveState()
    {
        wallAI.animator.SetBool("LaunchAttack", false);
        wallAI.SwitchToNewState(0);
    }
}