using UnityEngine;

namespace State.AICAC
{
    public class StateAttractionAICAC : _StateAICAC
    {
        [SerializeField] GlobalRefAICAC globalRef;
        [Header("Attraction Direction")]
        [SerializeField] Vector3 attractionDirection;
        [SerializeField] float distDestination;

        [Header("Attraction Active")]
        [SerializeField] float distDetectGround;
        [SerializeField] bool isGround;
        [SerializeField] bool applyGravity;

        float deltaTime;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);
            state = StateControllerAICAC.AIState.Attraction;
        }

        private void OnEnable()
        {
            globalRef.agent.enabled = false;
            Debug.Log("On Enable Attraction");
        }
        void Update()
        {
            if (Time.timeScale > 0)
                deltaTime = Time.deltaTime;

            distDestination = Vector3.Distance(globalRef.transform.position, globalRef.AttractionSO.pointBlackHole);

            ApplyAttraction();

            if (!globalRef.isInSynergyAttraction)
            {
                applyGravity = true;
            }
            if (isGround)
            {
                ActiveMoveState();
            }
        }

        private void FixedUpdate()
        {
            if(applyGravity)
                CheckGround();
        }

        void CheckGround()
        {
            RaycastHit hit = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.transform.position, -globalRef.transform.up, 
                                                                        globalRef.knockBackAICACSO.maskCheckObstacle, Color.red, distDetectGround);
            if (hit.transform != null)
            {
                isGround = true;
                Debug.Log(hit.transform.position);
            }
            else
                isGround = false;
        }

        void ApplyAttraction()
        {
            Vector3 move;

            if (!applyGravity)
            {
                attractionDirection = globalRef.AttractionSO.pointBlackHole - globalRef.transform.position;
                attractionDirection = (attractionDirection.normalized * (attractionDirection.magnitude - globalRef.AttractionSO.friction * deltaTime));
                move = new Vector3(attractionDirection.x, attractionDirection.y + (globalRef.knockBackAICACSO.AIVelocity.y), attractionDirection.z);
            }
            else
            {
                SetGravity();
                move = new Vector3(0, globalRef.knockBackAICACSO.AIVelocity.y, 0);
            }
            globalRef.characterController.Move(move * globalRef.AttractionSO.speed * deltaTime);
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
            Debug.Log("IsGround");
            stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseMove);
        } 

        private void OnDisable()
        {
            attractionDirection = Vector3.zero;
            globalRef.agent.enabled = true;
            isGround = false;
            applyGravity = false;
        }
    }
}