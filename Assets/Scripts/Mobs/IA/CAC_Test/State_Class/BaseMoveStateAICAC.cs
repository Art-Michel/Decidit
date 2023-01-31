using UnityEngine;
using UnityEngine.AI;

namespace State.AICAC
{
    public class BaseMoveStateAICAC : _StateAICAC
    {
        [SerializeField] GlobalRefAICAC globalRef;
        BaseMoveParameterAICAC baseMoveAICACSO;

        [Header("Nav Link")]
        [SerializeField] float maxDurationNavLink;
        [SerializeField] bool linkIsActive;
        bool triggerNavLink;
        NavMeshLink link;
        NavMeshLink navLink;
        NavMeshHit closestHit;
        Vector3 linkDestination;


        [Header("Direction Movement")]
        [SerializeField] float offset;
        [SerializeField] LayerMask mask;
        Vector3 destination;
        Vector3 dir;
        Vector3 left;
        Vector3 velocity;
        RaycastHit hitGround;

        [Header("LookAt")]
        Vector3 direction;
        Vector3 relativePos;

        [Header("Rate Calcule Path")]
        [SerializeField] float maxRateRepath;
        [SerializeField] float currentRateRepath;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.BaseMove;
        }

        private void Awake()
        {
            baseMoveAICACSO = globalRef.baseMoveAICACSO;
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

        private void FixedUpdate()
        {
            hitGround = RaycastAIManager.instanceRaycast.RaycastAI(transform.position, Vector3.down, mask, Color.blue, 100f);
        }

        void ActiveJump()
        {
            globalRef.agent.areaMask |= (1 << NavMesh.GetAreaFromName("Jump"));
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
            dir = CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer - globalRef.transform.position;
            left = Vector3.Cross(dir, Vector3.up).normalized;


            if (globalRef.agent.isOnOffMeshLink)
            {
                link = globalRef.agent.navMeshOwner as NavMeshLink;
                
                if (!triggerNavLink)
                {
                    if(Vector3.Distance(globalRef.transform.position, link.startPoint) < Vector3.Distance(globalRef.transform.position, link.endPoint))
                    {
                        linkDestination = link.endPoint;
                        triggerNavLink = true;
                    }
                    else
                    {
                        linkDestination = link.startPoint;
                        triggerNavLink = true;
                    }
                }
            }
            else
            {
                offset = Mathf.Lerp(offset, globalRef.offsetDestination, baseMoveAICACSO.offsetTransitionSmooth * Time.deltaTime);
                offset = Mathf.Clamp(offset, -Mathf.Abs(globalRef.offsetDestination), Mathf.Abs(globalRef.offsetDestination));

                destination = CheckNavMeshPoint(CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer + left * offset);

                if (triggerNavLink)
                {
                    globalRef.agent.areaMask &= ~(1 << NavMesh.GetAreaFromName("Jump"));
                    triggerNavLink = false;
                    Invoke("ActiveJump", baseMoveAICACSO.jumpRate);
                }
            }

            if (globalRef.agent.enabled && globalRef != null)
            {
                if(currentRateRepath >0)
                {
                    currentRateRepath -= Time.deltaTime;
                }
                else
                {
                    globalRef.agent.SetDestination(destination);
                    currentRateRepath = maxRateRepath;
                }

               /* if (!globalRef.agent.isOnOffMeshLink)
                {
                    globalRef.agent.updatePosition = false;
                    globalRef.agent.SetDestination(destination);
                    Vector3 updateXZ = Vector3.SmoothDamp(globalRef.transform.position, globalRef.agent.nextPosition, ref velocity, 0.1f);
                    float updateY = hitGround.point.y;

                    Vector3 updatePosition = new Vector3(updateXZ.x, updateY + 1f, updateXZ.z);
                    globalRef.transform.position = updatePosition;
                }
                else
                {
                    globalRef.agent.updatePosition = true;
                    globalRef.agent.SetDestination(destination);
                }*/
            }

            if (globalRef.distPlayer < baseMoveAICACSO.attackRange)
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
            if (NavMesh.SamplePosition(_destination, out closestHit, 1, 1))
            {
                _destination = closestHit.position;
            }
            return _destination;
        }
        void SpeedAdjusting()
        {
            if (!baseMoveAICACSO.activeAnticipDestination)
            {
                if (globalRef.distPlayer >= baseMoveAICACSO.distCanRun)
                {
                    if (globalRef.agent.speed < baseMoveAICACSO.runSpeed)
                    {
                        globalRef.agent.speed += baseMoveAICACSO.smoothSpeedRun * Time.deltaTime;
                    }
                    else
                        globalRef.agent.speed = baseMoveAICACSO.runSpeed;
                }
                else if (globalRef.distPlayer <= baseMoveAICACSO.distStopRun)
                {
                    if (globalRef.agent.speed > baseMoveAICACSO.baseSpeed)
                        globalRef.agent.speed -= baseMoveAICACSO.smoothSpeedbase * Time.deltaTime;
                    else
                        globalRef.agent.speed = baseMoveAICACSO.baseSpeed;
                }
                else
                {
                    if (globalRef.agent.speed < baseMoveAICACSO.baseSpeed)
                    {
                        globalRef.agent.speed += baseMoveAICACSO.smoothSpeedbase * Time.deltaTime;
                    }
                    else
                        globalRef.agent.speed = baseMoveAICACSO.baseSpeed;
                }
            }
            else
            {
                if (globalRef.agent.speed < baseMoveAICACSO.anticipSpeed)
                    globalRef.agent.speed += baseMoveAICACSO.smoothSpeedAnticip * Time.deltaTime;
                else
                    globalRef.agent.speed = baseMoveAICACSO.anticipSpeed;
            }
        }

        void SmoothLookAt()
        {
            if (globalRef.agent.isOnOffMeshLink)
            {
                direction = linkDestination;
            }
            else
            {
               // direction = globalRef.transform.position + globalRef.agent.desiredVelocity;
                direction = destination;
            }

            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            if (baseMoveAICACSO.speedRot < baseMoveAICACSO.maxSpeedRot)
                baseMoveAICACSO.speedRot += Time.deltaTime / baseMoveAICACSO.smoothRot;
            else
            {
                baseMoveAICACSO.speedRot = baseMoveAICACSO.maxSpeedRot;
            }

            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), baseMoveAICACSO.speedRot);
            globalRef.transform.rotation = rotation;
        }

        private void OnDisable()
        {
            globalRef.baseAttackAICACSO.isAttacking = false;
            baseMoveAICACSO.speedRot = 0;
            currentRateRepath = 0;
        }
    }
}