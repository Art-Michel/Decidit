using UnityEngine;

namespace State.AIBull
{
    public class AttractionStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;
        [SerializeField] Transform posAttraction;
        [SerializeField] Vector3 dirAttraction;
        float deltaTime;
        [SerializeField] bool applyGravity;
        [SerializeField] bool isGround;
        [SerializeField] float distDetectGround;
        [SerializeField] float distDestination;
        float velocityY;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.Attraction;
        }

        private void OnEnable()
        {
            globalRef.agent.enabled = false;
            globalRef.characterController.enabled = true;
        }

        void Update()
        {
            deltaTime = Time.deltaTime;
            distDestination = Vector3.Distance(globalRef.transform.position, posAttraction.position);
            ApplyAttraction();

            if (!globalRef.isInSynergyAttraction)
            {
                applyGravity = true;
            }
            if(isGround)
            {
                stateController.SetActiveState(StateControllerBull.AIState.Idle);
            }
        }

        private void FixedUpdate()
        {
            if(applyGravity)
                CheckGround();
        }

        void ApplyAttraction()
        {
            Vector3 move;
            if (!applyGravity)
            {
                dirAttraction = posAttraction.position - globalRef.transform.position;
                dirAttraction = (dirAttraction.normalized * (dirAttraction.magnitude - globalRef.AttractionSO.friction * deltaTime));
                move = new Vector3(dirAttraction.x, dirAttraction.y, dirAttraction.z);
            }
            else
            {
                SetGravity();
                move = new Vector3(0, 0 + velocityY, 0);
            }

            globalRef.characterController.Move(move * globalRef.AttractionSO.speed * deltaTime);
        }
        void SetGravity()
        {
            if (!isGround)
            {
                velocityY = velocityY - 9.8f * 4 * deltaTime;
            }
            else
            {
                velocityY = 0;
            }
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

        private void OnDisable()
        {
            velocityY = 0;
            globalRef.agent.enabled = true;
            applyGravity = false;
            isGround = false;
        }
    }
}