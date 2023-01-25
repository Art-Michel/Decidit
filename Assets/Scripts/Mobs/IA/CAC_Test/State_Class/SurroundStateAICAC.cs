using UnityEngine;

namespace State.AICAC
{
    public class SurroundStateAICAC : _StateAICAC
    {
        [SerializeField] GlobalRefAICAC globalRef;

        Vector3 destination;
        Ray ray;
        RaycastHit hit;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.SurroundPlayer;
        }

        private void Update()
        {
            SmoothLookAt();
        }

        private void FixedUpdate()
        {
            ChooseDirection();
            GetSurroundDestination();
        }

        public void ChooseDirection()
        {
            globalRef.spawnSurroundDodge.LookAt(globalRef.playerTransform.position);

            if (!globalRef.surroundAICACSO.left && !globalRef.surroundAICACSO.right)
            {
                hit = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.spawnSurroundDodge.position,
                globalRef.playerTransform.position - globalRef.spawnSurroundDodge.position, globalRef.surroundAICACSO.mask, Color.red, 100f);
                float angle;
                angle = Vector3.SignedAngle(globalRef.playerTransform.forward, globalRef.transform.forward, Vector3.up);

                if (angle > 0)
                {
                    globalRef.surroundAICACSO.left = true;
                }
                else
                {
                    globalRef.surroundAICACSO.right = true;
                }
            }
            else
            {
                MoveSurround();
            }
        }

        public void GetSurroundDestination()
        {
            Vector3 dir = globalRef.playerTransform.position - globalRef.transform.position;

            if (globalRef.surroundAICACSO.right)
            {
                Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;

                Vector3 right = -Vector3.Cross(dir, Vector3.up).normalized;
                destination = right + (left + (globalRef.spawnSurroundDodge.right + globalRef.spawnSurroundDodge.forward));
            }
            else if (globalRef.surroundAICACSO.left)
            {
                Vector3 right = -Vector3.Cross(dir, Vector3.up).normalized;

                Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
                destination = left + (right + (-globalRef.spawnSurroundDodge.right + globalRef.spawnSurroundDodge.forward));
            }

            hit = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.spawnSurroundDodge.position, destination, globalRef.surroundAICACSO.mask, Color.red,
                Vector3.Distance(globalRef.playerTransform.position, globalRef.spawnSurroundDodge.position));

            ray = new Ray(globalRef.spawnSurroundDodge.position, destination);
        }

        public void MoveSurround()
        {
            if (globalRef.agent.speed < globalRef.surroundAICACSO.surroundSpeed)
                globalRef.agent.speed += globalRef.surroundAICACSO.speedSmooth * Time.fixedDeltaTime;
            else
                globalRef.agent.speed = globalRef.surroundAICACSO.surroundSpeed;

            if (globalRef.distPlayer > globalRef.surroundAICACSO.stopSurroundDistance + 2)
            {
                if (globalRef.surroundAICACSO.left || globalRef.surroundAICACSO.right)
                {
                    globalRef.agent.SetDestination(ray.GetPoint(globalRef.distPlayer));
                }
            }
            else
            {
                if (globalRef.surroundAICACSO.left || globalRef.surroundAICACSO.right)
                {
                    globalRef.agent.SetDestination(globalRef.playerTransform.position);
                }
            }

            if (globalRef.distPlayer <= globalRef.surroundAICACSO.stopSurroundDistance)
            {
                StopSurround();
            }
        }

        void SmoothLookAt()
        {
            Vector3 direction;
            Vector3 relativePos;

            if (globalRef.agent.isOnOffMeshLink)
            {
                direction = destination;
            }
            else
            {
                direction = globalRef.transform.position + globalRef.agent.desiredVelocity;
            }

            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            if (globalRef.baseMoveAICACSO.speedRot < globalRef.baseMoveAICACSO.maxSpeedRot)
                globalRef.baseMoveAICACSO.speedRot += Time.deltaTime / globalRef.baseMoveAICACSO.smoothRot;
            else
            {
                globalRef.baseMoveAICACSO.speedRot = globalRef.baseMoveAICACSO.maxSpeedRot;
            }

            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.baseMoveAICACSO.speedRot);
            globalRef.transform.rotation = rotation;
        }

        void StopSurround()
        {
            globalRef.surroundAICACSO.right = false;
            globalRef.surroundAICACSO.left = false;
            globalRef.aICACVarianteState.RemoveAISelectedSurround(globalRef);
            stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseMove);
        }
    }
}