using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPlayerFly
{
    public FlyAINavMesh flyAINavMesh;
    public LockPlayerFlySO lockPlayerFlySO;
    public BaseAttackFlySO baseAttackFlySO;

    public void LockPlayer()
    {
        flyAINavMesh.colliderBaseAttack.gameObject.SetActive(false);

        lockPlayerFlySO.destinationFinal = new Vector3(flyAINavMesh.playerTransform.position.x, flyAINavMesh.playerTransform.position.y - 0.5f, flyAINavMesh.playerTransform.position.z);

        if (baseAttackFlySO.speedRotationAIAttack >= 1f)
        {
            flyAINavMesh.material_Instances.material.color = flyAINavMesh.material_Instances.color;
            flyAINavMesh.material_Instances.ChangeColorTexture(flyAINavMesh.material_Instances.color);

            Vector3 destinationFinal2D = new Vector2(lockPlayerFlySO.destinationFinal.x, lockPlayerFlySO.destinationFinal.z);
            Vector3 transformPos2D = new Vector2(flyAINavMesh.transform.position.x, flyAINavMesh.transform.position.z);

            baseAttackFlySO.timeGoToDestinationAttack = Vector3.Distance(destinationFinal2D, transformPos2D) / baseAttackFlySO.baseAttackSpeed;
            baseAttackFlySO.maxSpeedYTranslationAttack = Mathf.Abs(lockPlayerFlySO.destinationFinal.y - flyAINavMesh.transform.position.y) / baseAttackFlySO.timeGoToDestinationAttack;
            flyAINavMesh.SwitchToNewState(2);
        }
        else
        {
            flyAINavMesh.material_Instances.material.color = flyAINavMesh.material_Instances.colorPreAtatck;
            flyAINavMesh.material_Instances.ChangeColorTexture(flyAINavMesh.material_Instances.colorPreAtatck);
        }
    }
    public void SmoothLookAtYAxisAttack()
    {
        Vector3 relativePos;

        relativePos.x = lockPlayerFlySO.destinationFinal.x - flyAINavMesh.transform.position.x;
        relativePos.y = lockPlayerFlySO.destinationFinal.y - flyAINavMesh.transform.position.y;
        relativePos.z = lockPlayerFlySO.destinationFinal.z - flyAINavMesh.transform.position.z;

        Quaternion rotation = Quaternion.Slerp(flyAINavMesh.childObj.localRotation, Quaternion.LookRotation(relativePos, Vector3.up), baseAttackFlySO.speedRotationAIAttack);
        flyAINavMesh.childObj.localRotation = rotation;

        if (baseAttackFlySO.speedRotationAIAttack < baseAttackFlySO.maxSpeedRotationAIAttack)
        {
            baseAttackFlySO.speedRotationAIAttack += (Time.deltaTime / baseAttackFlySO.smoothRotationAttack);
            //lerpSpeedYValueAttack += (Time.deltaTime / ySpeedSmootherAttack);
        }
    }
}
