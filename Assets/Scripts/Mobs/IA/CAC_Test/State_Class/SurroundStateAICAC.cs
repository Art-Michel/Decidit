using UnityEngine;
using UnityEngine.AI;

namespace State.AICAC
{
    public class SurroundStateAICAC : _StateAICAC
    {
        [SerializeField] GlobalRefAICAC globalRef;

        Vector3 destination;
        Ray ray;
        RaycastHit hit;

        [Header("Nav Link")]
        [SerializeField] float maxDurationNavLink;
        [SerializeField] bool linkIsActive;
        bool triggerNavLink;
        NavMeshLink link;
        NavMeshLink navLink;
        NavMeshHit closestHit;
        Vector3 linkDestination;

        [Header("Rate Calcule Path")]
        [SerializeField] float maxRateRepath;
        [SerializeField] float currentRateRepath;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.SurroundPlayer;
        }

        private void Update()
        {
            SmoothLookAt();
            ManageCurrentNavMeshLink();

            if (globalRef.surroundAICACSO.left || globalRef.surroundAICACSO.right)
            {
                MoveSurround();
            }
        }


        private void FixedUpdate()
        {
            ChooseDirection();
            GetSurroundDestination();
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

        public void ChooseDirection()
        {
            globalRef.spawnSurroundDodge.LookAt(globalRef.playerTransform.position);

            if (globalRef.agent.isOnOffMeshLink)
            {
                link = globalRef.agent.navMeshOwner as NavMeshLink;

                if (!triggerNavLink)
                {
                    if (Vector3.Distance(globalRef.transform.position, link.startPoint) < Vector3.Distance(globalRef.transform.position, link.endPoint))
                    {
                        linkDestination = link.endPoint;
                        triggerNavLink = true;
                        Invoke("SetCanJump", 1f);
                    }
                    else
                    {
                        linkDestination = link.startPoint;
                        triggerNavLink = true;
                        Invoke("SetCanJump", 1f);
                    }
                }
            }

            if (!globalRef.surroundAICACSO.left && !globalRef.surroundAICACSO.right)
            {
                hit = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.spawnSurroundDodge.position,
                CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer - globalRef.spawnSurroundDodge.position, globalRef.surroundAICACSO.mask, Color.red, 100f);
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
        }
        void SetCanJump()
        {
            triggerNavLink = false;
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

            destination = CheckNavMeshPoint(ray.GetPoint(globalRef.distPlayer));
        }

        void MoveSurround()
        {
            if (globalRef.agent.speed < globalRef.surroundAICACSO.surroundSpeed)
                globalRef.agent.speed += globalRef.surroundAICACSO.speedSmooth * Time.deltaTime;
            else
                globalRef.agent.speed = globalRef.surroundAICACSO.surroundSpeed;

            if (globalRef.distPlayer > globalRef.surroundAICACSO.stopSurroundDistance + 2)
            {
                if (globalRef.surroundAICACSO.left || globalRef.surroundAICACSO.right)
                {
                    CalculatePath();
                }
            }
            else
            {
                if (globalRef.surroundAICACSO.left || globalRef.surroundAICACSO.right)
                {
                    CalculatePath();
                }
            }

            if (globalRef.distPlayer <= globalRef.surroundAICACSO.stopSurroundDistance)
            {
                StopSurround();
            }
        }
        Vector3 CheckNavMeshPoint(Vector3 _destination)
        {
            if (NavMesh.SamplePosition(_destination, out closestHit, 10f, 1))
            {
                _destination = closestHit.position;
            }
            return _destination;
        }

        void CalculatePath()
        {
            if (currentRateRepath > 0)
            {
                currentRateRepath -= Time.deltaTime;
            }
            else
            {
                SlowSpeed(globalRef.isInEylau);
                currentRateRepath = maxRateRepath;
            }
        }
        void SlowSpeed(bool active)
        {
            if (active)
            {
                globalRef.slowSpeedRot = globalRef.agent.speed / globalRef.slowRatio;
                globalRef.agent.speed = globalRef.slowSpeedRot;
                globalRef.agent.SetDestination(destination);
            }
            else
            {
                if (globalRef.agent.speed == globalRef.slowSpeedRot)
                    globalRef.agent.speed *= globalRef.slowRatio;

                globalRef.agent.SetDestination(destination);
            }
        }

        void SmoothLookAt()
        {
            Vector3 direction;
            Vector3 relativePos;

            if (globalRef.agent.isOnOffMeshLink)
            {
                direction = linkDestination;
            }
            else
            {
                direction = globalRef.agent.destination;
            }

            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            SlowRotation(globalRef.isInEylau);

            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.baseMoveAICACSO.speedRot);
            globalRef.transform.rotation = rotation;
        }
        void SlowRotation(bool active)
        {
            if (active)
            {
                if (globalRef.baseMoveAICACSO.speedRot < globalRef.baseMoveAICACSO.maxSpeedRot)
                {
                    globalRef.slowSpeedRot = globalRef.baseMoveAICACSO.smoothRot * 2;
                    globalRef.baseMoveAICACSO.speedRot += Time.deltaTime / globalRef.slowSpeedRot;
                }
                else
                {
                    globalRef.baseMoveAICACSO.speedRot = globalRef.baseMoveAICACSO.maxSpeedRot;
                }
            }
            else
            {
                if (globalRef.baseMoveAICACSO.speedRot < globalRef.baseMoveAICACSO.maxSpeedRot)
                {
                    globalRef.baseMoveAICACSO.speedRot += Time.deltaTime / globalRef.baseMoveAICACSO.smoothRot;
                }
                else
                {
                    globalRef.baseMoveAICACSO.speedRot = globalRef.baseMoveAICACSO.maxSpeedRot;
                }
            }
        }

        void StopSurround()
        {
            globalRef.surroundAICACSO.right = false;
            globalRef.surroundAICACSO.left = false;
            currentRateRepath -= 0;
            globalRef.aICACVarianteState.RemoveAISelectedSurround(globalRef);
            stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseMove);
        }
    }
}