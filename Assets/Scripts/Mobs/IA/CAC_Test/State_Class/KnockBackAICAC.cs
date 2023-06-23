using UnityEngine;

namespace State.AICAC
{
    public class KnockBackAICAC : _StateAICAC
    {
        [SerializeField] private float friction;
        [SerializeField] private float knockBackMultiplier;
        [SerializeField] private float forceYAxis;
        [SerializeField] private float gravityMultiplier;

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
            else if (globalRef.characterController.velocity.magnitude <= 1 && isGround)
            {
                knockBackDirection = Vector3.zero;
                ActiveIdleState();
            }

            if (isFall)
                if (globalRef.characterController.isGrounded)
                {
                    isGround = true;
                }
        }

        private void FixedUpdate()
        {
            CheckGround();
        }

        void CheckGround()
        {
            RaycastHit hit = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.transform.position, -globalRef.transform.up,
                                                                        globalRef.knockBackAICACSO.maskCheckObstacle, Color.red, distDetectGround);
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

            Vector2 Ymove = new Vector2(knockBackDirection.x, knockBackDirection.z);
            Ymove = Ymove.normalized;
            //move = new Vector3(knockBackDirection.x, knockBackDirection.y * 5 + (globalRef.knockBackAICACSO.AIVelocity.y), knockBackDirection.z);
            move = new Vector3(knockBackDirection.x, Ymove.magnitude * forceYAxis + (globalRef.knockBackAICACSO.AIVelocity.y), knockBackDirection.z);
            globalRef.characterController.Move(move * knockBackMultiplier * deltaTime);
        }

        void SetGravity()
        {
            if (!isGround)
            {
                globalRef.knockBackAICACSO.AIVelocity.y = globalRef.knockBackAICACSO.AIVelocity.y - 9.8f * gravityMultiplier * deltaTime;
            }
            else
            {
                globalRef.knockBackAICACSO.AIVelocity.y = 0;
            }
        }

        void ActiveIdleState()
        {
            stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseIdle);
        }

        private void OnDisable()
        {
            knockBackDirection = Vector3.zero;
            globalRef.agent.enabled = true;
            globalRef.knockBackAICACSO.AIVelocity.y = 0;
        }
    }
}