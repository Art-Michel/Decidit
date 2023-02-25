using UnityEngine;

namespace State.AIBull
{
    public class StateKnockBackBullAI : _StateBull
    {
        [SerializeField] private float friction = 20f;

        [SerializeField] GlobalRefBullAI globalRef;

        [Header("KnockBack Direction")]
        [SerializeField] Vector3 knockBackDirection;
        [SerializeField] float distDetectGround;
        [SerializeField] bool isGround;
        [SerializeField] bool isFall;

        float deltaTime;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.KnockBack;
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

        // Start is called before the first frame update
        void Start()
        {

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

            /*if (knockBackDirection == Vector3.zero)
                ActiveIdleState();*/

            if (isGround && isFall)
            {
                knockBackDirection = Vector3.zero;
            }
            else if (globalRef.characterController.velocity.magnitude == 0)
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
            RaycastHit hit = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.transform.position, -globalRef.transform.up, 
                                                                        globalRef.rushBullSO.maskCheckObstacle, Color.red, distDetectGround);
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

            move = new Vector3(knockBackDirection.x, knockBackDirection.y + (globalRef.rushBullSO.AIVelocity.y), knockBackDirection.z);
            globalRef.characterController.Move(move * deltaTime);
        }

        void SetGravity()
        {
            if (!isGround)
            {
                globalRef.rushBullSO.AIVelocity.y = globalRef.rushBullSO.AIVelocity.y - 9.8f * 4 * deltaTime;
            }
            else
            {
                globalRef.rushBullSO.AIVelocity.y = 0;
            }
        }

        void ActiveIdleState()
        {
            stateController.SetActiveState(StateControllerBull.AIState.Idle);
        }

        private void OnDisable()
        {
            globalRef.agent.enabled = true;
        }
    }
}