using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State.AICAC
{
    public class KnockBackAICAC : _StateAICAC
    {
        [SerializeField] private const float friction = 20f;
        [SerializeField] GlobalRefAICAC globalRef;

        [Header("KnockBack Direction")]
        [SerializeField] Vector3 knockBackDirection;
        [SerializeField] float distDetectGround;
        [SerializeField] bool isGround;
        [SerializeField] bool isFall;

        float deltaTime;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.KnockBack;
        }

        private void OnEnable()
        {
            try
            {
                isFall = false;
                isGround = false;
                globalRef.agent.enabled = false;
                globalRef.characterController.enabled = true;
                knockBackDirection = globalRef.enemyHealth.KnockBackDir;
                globalRef.characterController.Move(Vector3.zero);
            }
            catch
            {
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (Time.timeScale > 0)
                deltaTime = Time.deltaTime;

            ApplyKnockBack();

            if (!isGround)
            {
                isFall = true;
            }

           /* if (knockBackDirection == Vector3.zero)
                ActiveIdleState();*/

            if (isGround && isFall)
            {
                knockBackDirection = Vector3.zero;
                ActiveIdleState();
            }
           /* else if (globalRef.characterController.velocity.magnitude == 0)
            {
                knockBackDirection = Vector3.zero;
                ActiveIdleState();
            }*/
        }

        private void FixedUpdate()
        {
            CheckGround();
        }

        void CheckGround()
        {
            RaycastHit hit = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.transform.position, -globalRef.transform.up,
                                                                        globalRef.knockBackAICAC.maskCheckObstacle, Color.red, distDetectGround);
            if (hit.transform != null)
                isGround = true;
            else
                isGround = false;
        }

        void ApplyKnockBack()
        {
            Vector3 move;

            SetGravity();
            knockBackDirection = (knockBackDirection.normalized * (knockBackDirection.magnitude - friction * deltaTime));

            move = new Vector3(knockBackDirection.x, knockBackDirection.y + (globalRef.knockBackAICAC.AIVelocity.y), knockBackDirection.z);
            globalRef.characterController.Move(move * deltaTime);
        }

        void SetGravity()
        {
            if (!isGround)
            {
                globalRef.knockBackAICAC.AIVelocity.y = globalRef.knockBackAICAC.AIVelocity.y - 9.8f * 4 * deltaTime;
                Debug.Log("Active Gravity" );
            }
            else
            {
                globalRef.knockBackAICAC.AIVelocity.y = 0;
            }
        }

        void ActiveIdleState()
        {
            stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseIdle);
        }

        private void OnDisable()
        {
            globalRef.agent.enabled = true;
        }
    }
}