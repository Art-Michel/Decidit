using UnityEngine;

namespace State.FlyAI
{
    public class KnockBackFlyAI : _StateFlyAI
    {
        [SerializeField] private float friction = 20f;

        [SerializeField] GlobalRefFlyAI globalRef;
        [SerializeField] Transform childflyAI;

        [Header("KnockBack Direction")]
        [SerializeField] Vector3 knockBackDirection;
        [SerializeField] float distDetectGround;
        [SerializeField] bool isGround;
        [SerializeField] bool isFall;

        float deltaTime;

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

            SetGravity();
            knockBackDirection = (knockBackDirection.normalized * (knockBackDirection.magnitude - friction * deltaTime));

            move = new Vector3(knockBackDirection.x, knockBackDirection.y + (globalRef.KnockBackFlySO.AIVelocity.y), knockBackDirection.z);
            globalRef.characterController.Move(move * deltaTime);
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
            Vector3 positionChild = childflyAI.transform.position;
            globalRef.transform.position = positionChild;
            childflyAI.transform.position = globalRef.transform.position;
            globalRef.agent.baseOffset = 0.3f;
            globalRef.agent.enabled = true;
        }
    }
}