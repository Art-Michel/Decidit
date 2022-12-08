using UnityEngine;

public class BaseAttackFly
{
    public FlyAINavMesh flyAINavMesh;
    public BaseAttackFlySO baseAttackFlySO;
    public LockPlayerFly lockPlayerFly;
    public LockPlayerFlySO lockPlayerFlySO;

    public void Attack()
    {
        baseAttackFlySO.distDestinationFinal = Vector3.Distance(lockPlayerFlySO.destinationFinal, flyAINavMesh.transform.position);
        flyAINavMesh.colliderBaseAttack.gameObject.SetActive(true);

        ApplyFlyingMove();

        if (baseAttackFlySO.distDestinationFinal > lockPlayerFlySO.distStopLockPlayer)
        {
            lockPlayerFly.LockPlayer();
            lockPlayerFly.SmoothLookAtYAxisAttack();
        }

        if (baseAttackFlySO.distDestinationFinal <= 1.5f)
        {
            flyAINavMesh.SwitchToNewState(0);
        }
    }

    void ApplyFlyingMove()
    {
        flyAINavMesh.agent.speed = baseAttackFlySO.baseAttackSpeed;

        flyAINavMesh.agent.SetDestination(new Vector3(flyAINavMesh.transform.position.x, 0, flyAINavMesh.transform.position.z) + flyAINavMesh.childObj.TransformDirection(Vector3.forward));
        baseAttackFlySO.currentSpeedYAttack = Mathf.Lerp(baseAttackFlySO.currentSpeedYAttack, baseAttackFlySO.maxSpeedYTranslationAttack, baseAttackFlySO.lerpSpeedYValueAttack);

        baseAttackFlySO.lerpSpeedYValueAttack += (Time.deltaTime / baseAttackFlySO.ySpeedSmootherAttack);

        if (Mathf.Abs(flyAINavMesh.transform.position.y - lockPlayerFlySO.destinationFinal.y) > 1)
        {
            if (flyAINavMesh.transform.position.y < lockPlayerFlySO.destinationFinal.y)
            {
                flyAINavMesh.agent.baseOffset += baseAttackFlySO.currentSpeedYAttack * Time.deltaTime;
            }
            else
            {
                flyAINavMesh.agent.baseOffset -= baseAttackFlySO.currentSpeedYAttack * Time.deltaTime;
            }
        }
    }
}
