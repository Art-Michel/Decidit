using UnityEngine;
using UnityEngine.AI;

namespace State.WallAI
{
    public class BaseMoveWallAIState : _StateWallAI
    {
        protected StateControllerWallAI stateControllerWallAI;

        public GlobalRefWallAI globalRef;

        RaycastHit hit;

        public override void InitState(StateControllerWallAI stateController)
        {
            base.InitState(stateController);
            stateControllerWallAI = stateController;
            state = StateControllerWallAI.WallAIState.BaseMove;
        }

        private void OnEnable()
        {
            try
            {
                globalRef.meshRenderer.enabled = false;
            }
            catch
            {
            }
        }

        private void Update()
        {
            MoveAI();

            if (globalRef.enemyHealth._hp <= 0)
            {
                stateControllerWallAI.SetActiveState(StateControllerWallAI.WallAIState.Death, true);
            }
        }

        private void FixedUpdate()
        {
            if (!globalRef.baseMoveWallAISO.findNewPos)
                SelectNewPos();
        }

        public void MoveAI()
        {
            if (!globalRef.agent.isOnOffMeshLink)
            {
                WallCrackEffect();
            }

            if (!IsMoving())
                globalRef.baseMoveWallAISO.findNewPos = false;

            globalRef.agent.SetDestination(globalRef.baseMoveWallAISO.newPos);
            globalRef.agent.speed = globalRef.baseMoveWallAISO.speedMovement;

            LaunchDelayBeforeAttack();
        }
        bool IsMoving()
        {
            if (globalRef.agent.remainingDistance == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        void SelectNewPos()
        {
            if (!IsMoving())
            {
                globalRef.baseMoveWallAISO.selectedWall = Random.Range(0, 4);
                globalRef.baseMoveWallAISO.newPos = SearchNewPos(globalRef.walls[globalRef.baseMoveWallAISO.selectedWall].bounds);

                hit = RaycastAIManager.RaycastAI(globalRef.baseMoveWallAISO.newPos, globalRef.playerTransform.position - globalRef.baseMoveWallAISO.newPos, globalRef.baseMoveWallAISO.mask,
                    Color.blue, Vector3.Distance(globalRef.baseMoveWallAISO.newPos, globalRef.playerTransform.position));

                if (hit.transform != globalRef.playerTransform)
                {
                    globalRef.baseMoveWallAISO.findNewPos = false;
                }
                else
                {
                    globalRef.baseMoveWallAISO.findNewPos = true;
                }
            }
        }
        Vector3 SearchNewPos(Bounds bounds)
        {
            return new Vector3(
               Random.Range(bounds.min.x, bounds.max.x),
               Random.Range(bounds.min.y, bounds.max.y),
               Random.Range(bounds.min.z, bounds.max.z)
           );
        }
        public void WallCrackEffect()
        {
            globalRef.baseMoveWallAISO.distSinceLast = Vector3.Distance(globalRef.transform.position, globalRef.baseMoveWallAISO.lastWallCrack.transform.position);

            if (globalRef.baseMoveWallAISO.distSinceLast >= globalRef.baseMoveWallAISO.decalage)
            {
                globalRef.baseMoveWallAISO.lastWallCrack = Instantiate(globalRef.baseMoveWallAISO.wallCrackPrefab,
                    globalRef.transform.position,
                    Quaternion.Euler(0, globalRef.orientation, 0));
            }
        }

        void LaunchDelayBeforeAttack()
        {
            if (globalRef.baseMoveWallAISO.rateAttack > 0)
            {
                globalRef.baseMoveWallAISO.rateAttack -= Time.deltaTime;
            }
            else
            {
                if (!globalRef.agent.isOnOffMeshLink && !IsMoving())
                {
                    stateControllerWallAI.SetActiveState(StateControllerWallAI.WallAIState.BaseAttack);
                }
            }
        }

        // Reset Value When Change State
        private void OnDisable()
        {
            globalRef.baseAttackWallAISO.bulletCount = globalRef.baseAttackWallAISO.maxBulletCount;
            globalRef.baseMoveWallAISO.rateAttack = globalRef.baseMoveWallAISO.maxRateAttack;
            globalRef.baseMoveWallAISO.findNewPos = false;
            globalRef.agent.SetDestination(globalRef.transform.position);
        }
    }
}