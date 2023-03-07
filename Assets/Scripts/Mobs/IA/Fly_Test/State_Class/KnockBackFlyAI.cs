using UnityEngine;

namespace State.FlyAI
{
    public class KnockBackFlyAI : _StateFlyAI
    {
        [SerializeField] private float friction;
        [SerializeField] private float knockBackMultiplier;

        [SerializeField] GlobalRefFlyAI globalRef;
        [SerializeField] Transform childflyAI;

        [Header("KnockBack Direction")]
        [SerializeField] Vector3 knockBackDirection;
        [SerializeField] float distDetectGround;
        [SerializeField] bool isGround;
        [SerializeField] bool isFall;

        float deltaTime;

        bool once;

        public override void InitState(StateControllerFlyAI stateController)
        {
            base.InitState(stateController);

            state = StateControllerFlyAI.AIState.KnockBack;
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

            if (isGround && isFall)
            {
                knockBackDirection = Vector3.zero;
                ActiveIdleState();
            }
            else if (globalRef.characterController.velocity.magnitude == 0)
            {
                ActiveIdleState();
            }
        }

        private void FixedUpdate()
        {
            CheckGround();
        }

        void CheckGround()
        {
            RaycastHit hit = RaycastAIManager.instanceRaycast.RaycastAI(childflyAI.position, -childflyAI.up,
                                                                        globalRef.KnockBackFlySO.maskCheckObstacle, Color.red, distDetectGround);
            if (hit.transform != null)
                isGround = true;
            else
                isGround = false;
        }

        void ApplyKnockBack()
        {
            Vector3 move;

            //SetGravity();
            knockBackDirection = (knockBackDirection.normalized * (knockBackDirection.magnitude - friction * deltaTime));

            //move = new Vector3(knockBackDirection.x, knockBackDirection.y + (globalRef.KnockBackFlySO.AIVelocity.y), knockBackDirection.z);
            move = new Vector3(knockBackDirection.x, 0, knockBackDirection.z);
            globalRef.characterController.Move(move * knockBackMultiplier * deltaTime);
        }

        void SetGravity()
        {
            if (!isGround)
            {
                globalRef.KnockBackFlySO.AIVelocity.y = globalRef.KnockBackFlySO.AIVelocity.y - 9.8f * 4 * deltaTime;
            }
            else
            {
                globalRef.KnockBackFlySO.AIVelocity.y = 0;
            }
        }

        void ActiveIdleState()
        {
            stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
        }

        private void OnDisable()
        {
            globalRef.agent.enabled = true;
            if (once)
            {
                globalRef.agent.baseOffset = childflyAI.transform.position.y;
                Vector3 positionChild = new Vector3(childflyAI.transform.position.x,
                                                    globalRef.transform.position.y,
                                                    childflyAI.transform.position.z);

                globalRef.transform.position = positionChild;
                childflyAI.transform.position = globalRef.transform.position;
            }
            once = true;
        }
    }
}