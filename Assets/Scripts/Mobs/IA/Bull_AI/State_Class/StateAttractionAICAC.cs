using UnityEngine;

namespace State.AICAC
{
    public class StateAttractionAICAC : _StateAICAC
    {
        [SerializeField] private float friction;
        [SerializeField] private float knockBackMultiplier;

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

            if (isGround && isFall)
            {
                ActiveMoveState();
            }
            else if (globalRef.characterController.velocity.magnitude <= 1)
            {
                ActiveMoveState();
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
            move = new Vector3(knockBackDirection.x, knockBackDirection.y + (globalRef.knockBackAICACSO.AIVelocity.y), knockBackDirection.z);
            globalRef.characterController.Move(move * knockBackMultiplier * deltaTime);
        }

        void SetGravity()
        {
            if (!isGround)
            {
                globalRef.knockBackAICACSO.AIVelocity.y = globalRef.knockBackAICACSO.AIVelocity.y - 9.8f * 4 * deltaTime;
            }
            else
            {
                globalRef.knockBackAICACSO.AIVelocity.y = 0;
            }
        }

        void ActiveMoveState()
        {
            stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseMove);
        } 

        private void OnDisable()
        {
            knockBackDirection = Vector3.zero;
            globalRef.agent.enabled = true;
        }
    }
}