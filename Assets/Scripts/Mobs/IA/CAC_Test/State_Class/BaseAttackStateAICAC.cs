using UnityEngine;

namespace State.AICAC
{
    public class BaseAttackStateAICAC : _StateAICAC
    {
        [SerializeField] GlobalRefAICAC globalRef;

        RaycastHit hit;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.BaseAttack;
        }

        private void Update()
        {
            if (globalRef.distPlayer > globalRef.baseAttackAICACSO.attackRange && !globalRef.baseAttackAICACSO.isAttacking)
            {
                stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseMove);
            }
            else
            {
                BaseAttack();
            }

            SmoothLookAt();
        }

        public void BaseAttack()
        {
            globalRef.agent.speed = 0;

            if (globalRef.baseAttackAICACSO.currentAttackRate <= 0)
            {
                Debug.Log("launch attack");

                globalRef.myAnimator.SetBool("Attack", true);
                globalRef.baseAttackAICACSO.isAttacking = true;
                globalRef.baseAttackAICACSO.currentAttackRate = globalRef.baseAttackAICACSO.maxAttackRate;
            }
            else if (!globalRef.baseAttackAICACSO.isAttacking)
            {
                globalRef.baseAttackAICACSO.currentAttackRate -= Time.deltaTime;
            }
        }

        public void SmoothLookAt()
        {
            Vector3 direction;
            Vector3 relativePos;

            direction = globalRef.playerTransform.position;
            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            if (globalRef.baseAttackAICACSO.speedRot < globalRef.baseAttackAICACSO.maxSpeedRot)
                globalRef.baseAttackAICACSO.speedRot += Time.deltaTime / globalRef.baseAttackAICACSO.smoothRot;
            else
            {
                globalRef.baseAttackAICACSO.speedRot = globalRef.baseAttackAICACSO.maxSpeedRot;
            }

            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.baseAttackAICACSO.speedRot);
            globalRef.transform.rotation = rotation;
        }

        private void OnDisable()
        {
            globalRef.baseAttackAICACSO.currentAttackRate = 0;
            globalRef.baseAttackAICACSO.speedRot = 0;
        }
    }
}