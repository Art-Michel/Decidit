using UnityEngine;
using UnityEngine.AI;

namespace State.AIBull
{
    public class BaseMoveStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.BaseMove;
        }

        private void Update()
        {
            BaseMovement();
            Debug.Log(globalRef.agent.isOnOffMeshLink);
            
            if(!globalRef.agent.isOnOffMeshLink)
            {
                if (globalRef.launchRush)
                    LaunchRush();
                else if (globalRef.distPlayer < globalRef.baseMoveBullSO.distActiveRush)
                {
                    globalRef.launchRush = true;
                    LaunchRush();
                }
            }
        }

        void BaseMovement()
        {
            Vector3 newDestination;
            globalRef.agent.speed = globalRef.baseMoveBullSO.baseSpeed;
            newDestination = globalRef.playerTransform.position + (globalRef.playerTransform.right * globalRef.offsetDestination);

            globalRef.agent.SetDestination(CheckNavMeshPoint(newDestination));
            SmoothLookAtPlayer();
            if (globalRef.distPlayer < globalRef.baseAttackBullSO.attackRange)
            {
                //stateController.SetActiveState(StateControllerBull.AIState.WaitBeforeRush);
            }
        }
        Vector3 CheckNavMeshPoint(Vector3 newDestination)
        {
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(newDestination, out closestHit, 20, 1))
            {
                newDestination = closestHit.position;
            }
            return newDestination;
        }

        void LaunchRush()
        {
            stateController.SetActiveState(StateControllerBull.AIState.Rush);
        }

        void SmoothLookAtPlayer()
        {
            Vector3 direction;
            Vector3 relativePos;

            direction = globalRef.agent.destination;

            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            if (globalRef.baseMoveBullSO.speedRot < globalRef.baseMoveBullSO.maxSpeedRot)
                globalRef.baseMoveBullSO.speedRot += Time.deltaTime / globalRef.baseMoveBullSO.smoothRot;
            else
            {
                globalRef.baseMoveBullSO.speedRot = globalRef.baseMoveBullSO.maxSpeedRot;
            }

            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.baseMoveBullSO.speedRot);
            globalRef.transform.rotation = rotation;
        }

        private void OnDisable()
        {
            globalRef.baseMoveBullSO.speedRot = 0;
            globalRef.agent.speed = globalRef.baseMoveBullSO.stopSpeed;
        }
    }
}