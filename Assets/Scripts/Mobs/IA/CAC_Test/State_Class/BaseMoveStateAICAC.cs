using UnityEngine;
using UnityEngine.AI;

namespace State.AICAC
{
    public class BaseMoveStateAICAC : _StateAICAC
    {
        [SerializeField] GlobalRefAICAC globalRef;

        Vector3 destination;

        [SerializeField] float maxDurationNavLink;
        [SerializeField] bool linkIsActive;
        NavMeshLink navLink;
        bool triggerNavLink;

        RaycastHit hit;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.BaseMove;
        }

        private void Update()
        {
            //globalRef.agent.areaMask |= (1 << NavMesh.GetAreaFromName("Walkable"));
            //globalRef.agent.areaMask |= (1 << NavMesh.GetAreaFromName("Not Walkable"));
           // globalRef.agent.areaMask |= (1 << NavMesh.GetAreaFromName("Jump"));

            SmoothLookAt();
            BaseMovement();
            ManageCurrentNavMeshLink();
        }

        void ManageCurrentNavMeshLink()
        {
            if (globalRef.agent.isOnOffMeshLink)
            {
                if (maxDurationNavLink > 0)
                {
                    globalRef.agent.ActivateCurrentOffMeshLink(false);
                    linkIsActive = false;
                    Debug.Log(linkIsActive + "Disable");
                    maxDurationNavLink -= Time.deltaTime;
                }
                else
                {
                    linkIsActive = true;
                    Debug.Log(linkIsActive + "Enable");
                    globalRef.agent.ActivateCurrentOffMeshLink(true);
                }

                globalRef.agent.speed = 3;
                if (navLink == null)
                    navLink = globalRef.agent.navMeshOwner as NavMeshLink;

            }
            else
            {
                if (navLink != null)
                {
                    navLink.UpdateLink();
                    navLink = null;
                }
                maxDurationNavLink = globalRef.agentLinkMover._duration;
            }
        }

        void BaseMovement()
        {
            Vector3 dir = CheckPlayerDownPos.positionPlayer - globalRef.transform.position;
            Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;


            if (globalRef.agent.isOnOffMeshLink)
            {
                NavMeshLink link;
                link = globalRef.agent.navMeshOwner as NavMeshLink;
                
                if (!triggerNavLink)
                {
                    if(Vector3.Distance(globalRef.transform.position, link.startPoint) < Vector3.Distance(globalRef.transform.position, link.endPoint))
                    {
                        destination = link.endPoint;
                        triggerNavLink = true;
                    }
                    else
                    {
                        destination = link.startPoint;
                        triggerNavLink = true;
                    }
                }
            }
            else
            {
                destination = CheckPlayerDownPos.positionPlayer + left * globalRef.offsetDestination;

                if (triggerNavLink)
                {
                    globalRef.agent.areaMask &= ~(1 << NavMesh.GetAreaFromName("Jump"));
                    triggerNavLink = false;
                    Invoke("ActiveJump", globalRef.baseMoveAICACSO.jumpRate);
                }
            }

            globalRef.debugDestination = CheckNavMeshPoint(destination);
            globalRef.agent.SetDestination(CheckNavMeshPoint(destination));

            if (globalRef.distPlayer < globalRef.baseMoveAICACSO.attackRange)
            {
                stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseAttack);
            }
            else
            {
                if (!globalRef.agent.isOnOffMeshLink)
                    SpeedAdjusting();
            }
        }
        Vector3 CheckNavMeshPoint(Vector3 _destination)
        {
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(_destination, out closestHit, 1, 1))
            {
                _destination = closestHit.position;
            }
            return _destination;
        }
        void SpeedAdjusting()
        {
            if (!globalRef.baseMoveAICACSO.activeAnticipDestination)
            {
                if (globalRef.distPlayer >= globalRef.baseMoveAICACSO.distCanRun)
                {
                    if (globalRef.agent.speed < globalRef.baseMoveAICACSO.runSpeed)
                    {
                        globalRef.agent.speed += globalRef.baseMoveAICACSO.smoothSpeedRun * Time.deltaTime;
                    }
                    else
                        globalRef.agent.speed = globalRef.baseMoveAICACSO.runSpeed;
                }
                else if (globalRef.distPlayer <= globalRef.baseMoveAICACSO.distStopRun)
                {
                    if (globalRef.agent.speed > globalRef.baseMoveAICACSO.baseSpeed)
                        globalRef.agent.speed -= globalRef.baseMoveAICACSO.smoothSpeedbase * Time.deltaTime;
                    else
                        globalRef.agent.speed = globalRef.baseMoveAICACSO.baseSpeed;
                }
                else
                {
                    if (globalRef.agent.speed < globalRef.baseMoveAICACSO.baseSpeed)
                    {
                        globalRef.agent.speed += globalRef.baseMoveAICACSO.smoothSpeedbase * Time.deltaTime;
                    }
                    else
                        globalRef.agent.speed = globalRef.baseMoveAICACSO.baseSpeed;
                }
            }
            else
            {
                if (globalRef.agent.speed < globalRef.baseMoveAICACSO.anticipSpeed)
                    globalRef.agent.speed += globalRef.baseMoveAICACSO.smoothSpeedAnticip * Time.deltaTime;
                else
                    globalRef.agent.speed = globalRef.baseMoveAICACSO.anticipSpeed;
            }
        }

        void SmoothLookAt()
        {
            Vector3 direction;
            Vector3 relativePos;

            direction = destination;
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

        void ActiveJump()
        {
            globalRef.agent.areaMask |= (1 << NavMesh.GetAreaFromName("Jump"));
        }

        private void OnDisable()
        {
            globalRef.baseAttackAICACSO.isAttacking = false;
            globalRef.baseMoveAICACSO.speedRot = 0;
        }
    }
}