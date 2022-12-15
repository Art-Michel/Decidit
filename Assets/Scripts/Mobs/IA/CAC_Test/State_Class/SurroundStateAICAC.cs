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
            if (state == StateControllerAICAC.AIState.SurroundPlayer)
            {
            }
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
                hit = RaycastAIManager.RaycastAI(globalRef.spawnSurroundDodge.position,
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

            hit = RaycastAIManager.RaycastAI(globalRef.spawnSurroundDodge.position, destination, globalRef.surroundAICACSO.mask, Color.red,
                Vector3.Distance(globalRef.playerTransform.position, globalRef.spawnSurroundDodge.position));

            ray = new Ray(globalRef.spawnSurroundDodge.position, destination);
        }

        public void MoveSurround()
        {
            if (globalRef.agent.speed < globalRef.surroundAICACSO.surroundSpeed)
                globalRef.agent.speed += globalRef.surroundAICACSO.speedSmooth * Time.deltaTime;
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

        void StopSurround()
        {
            globalRef.surroundAICACSO.right = false;
            globalRef.surroundAICACSO.left = false;
            globalRef.aICACVarianteState.RemoveAISelected(globalRef);
            stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseMove);
        }
    }
}