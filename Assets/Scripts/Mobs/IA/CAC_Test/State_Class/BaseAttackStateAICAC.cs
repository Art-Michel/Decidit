using UnityEngine;

namespace State.AICAC
{
    public class BaseAttackStateAICAC : _StateAICAC
    {
        [SerializeField] GlobalRefAICAC globalRef;
        RaycastHit hit;
        [SerializeField] Transform tongue;

        [SerializeField] bool endAttack;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.BaseAttack;
        }

        private void Update()
        {
            BaseAttack();
            SmoothLookAt();
        }

        private void LateUpdate()
        {
            tongue.LookAt(globalRef.playerTransform.position);
        }

        public void BaseAttack()
        {
            globalRef.agent.speed = 0;

            if (globalRef.baseAttackAICACSO.currentAttackRate <= 0)
            {
                globalRef.myAnimator.SetBool("Attack", true);
                globalRef.baseAttackAICACSO.isAttacking = true;
                globalRef.baseAttackAICACSO.currentAttackRate = globalRef.baseAttackAICACSO.maxAttackRate;
            }
            else if (!globalRef.baseAttackAICACSO.isAttacking)
            {
                Debug.Log("Red Color préattack");

                if (globalRef.distPlayer > globalRef.baseAttackAICACSO.attackRange && globalRef.baseAttackAICACSO.currentAttackRate == globalRef.baseAttackAICACSO.maxAttackRate)
                {
                    stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseMove);
                    return;
                }

                globalRef.material_Instances.Material.color = globalRef.material_Instances.ColorPreAtatck;
                globalRef.material_Instances.ChangeColorTexture(globalRef.material_Instances.ColorPreAtatck);
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
            globalRef.material_Instances.Material.color = globalRef.material_Instances.Color;
            globalRef.material_Instances.ChangeColorTexture(globalRef.material_Instances.Color);
            globalRef.baseAttackAICACSO.currentAttackRate = globalRef.baseAttackAICACSO.maxAttackRate;
            globalRef.baseAttackAICACSO.speedRot = 0;
        }
    }
}