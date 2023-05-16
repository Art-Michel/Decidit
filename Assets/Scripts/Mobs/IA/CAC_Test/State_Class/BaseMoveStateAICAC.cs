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
        [SerializeField] bool triggerNavLink;
        public bool isOnNavLink;
        [SerializeField] NavMeshLink navLink;
        NavMeshHit closestHit;
        [SerializeField] Vector3 linkDestination;

        [Header("Direction Movement")]
        [SerializeField] float offset;
        [SerializeField] LayerMask mask;
        [SerializeField] Vector3 destination;
        Vector3 dir;
        Vector3 left;

        [Header("LookAt")]
        Vector3 direction;
        Vector3 relativePos;
        bool lookForwardJump;

        [Header("Rate Calcule Path")]
        [SerializeField] float maxRateRepath;
        [SerializeField] float currentRateRepath;

        [SerializeField] bool activeSurround;

        [SerializeField] float distToCirclePos;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.BaseMove;
        }

        void OnEnable()
        {
            if (baseMoveAICACSO != null)
                baseMoveAICACSO.currentCoolDownAttack = Random.Range(baseMoveAICACSO.maxCoolDownAttack.x, baseMoveAICACSO.maxCoolDownAttack.y);

            if (globalRef != null && globalRef.myAnimator != null)
                AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Walk");
        }

        private void Start()
        {
            baseMoveAICACSO = globalRef.baseMoveAICACSO;
        }

        private void Update()
        {
            //sphereDebug.position = destination;
            //globalRef.agent.areaMask |= (1 << NavMesh.GetAreaFromName("Walkable"));
            //globalRef.agent.areaMask |= (1 << NavMesh.GetAreaFromName("Not Walkable"));
            // globalRef.agent.areaMask |= (1 << NavMesh.GetAreaFromName("Jump"));
            distToCirclePos = Vector3.Distance(destination, globalRef.transform.position);
            distToCirclePos = Vector3.Distance(globalRef.playerTransform.position, globalRef.transform.position);


            SmoothLookAt();
            ManageCurrentNavMeshLink();

            BaseMovement();
        }

        void CoolDownAttack()
        {
            if(baseMoveAICACSO.currentCoolDownAttack >0)
            {
                baseMoveAICACSO.currentCoolDownAttack -= Time.deltaTime;
                destination = CheckNavMeshPoint(globalRef.destinationSurround);
                
                if (Vector3.Distance(destination, globalRef.transform.position) < baseMoveAICACSO.distStopSurroundNearPlayer || globalRef.agent.velocity.magnitude <1f)
                {
                    baseMoveAICACSO.currentCoolDownAttack = 0;
                }
            }
            else
            {
                baseMoveAICACSO.currentCoolDownAttack = Random.Range(baseMoveAICACSO.maxCoolDownAttack.x, baseMoveAICACSO.maxCoolDownAttack.y);
            }
        }

        void ActiveJump()
        {
            globalRef.agent.areaMask |= (1 << NavMesh.GetAreaFromName("Jump"));
        }
        void ManageCurrentNavMeshLink()
        {
            if (globalRef.agent.isOnOffMeshLink)
            {
                lookForwardJump = true;
                globalRef.agent.autoTraverseOffMeshLink = false;
                globalRef.agentLinkMover.enabled = true;

                if (!isOnNavLink)
                {
                    isOnNavLink = true;
                    globalRef.agent.speed = 0;
                }

                if (navLink == null) // Recover Current NavMEshLink
                {
                    globalRef.agent.ActivateCurrentOffMeshLink(false);
                    navLink = globalRef.agent.navMeshOwner as NavMeshLink;
                    globalRef.agentLinkMover.m_Curve.AddKey(0.5f, Mathf.Abs((navLink.endPoint.y - navLink.startPoint.y) / 1.5f));
                    globalRef.agentLinkMover._height = Mathf.Abs((navLink.endPoint.y - navLink.startPoint.y) / 1.5f);
                }

                if (globalRef.agentLinkMover._StopJump) // Lock Jump
                {
                    AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "StartJump");
                    if (baseMoveAICACSO.delayBeforeJump <= 0) // Delay Befor Jump
                    {
                        globalRef.agentLinkMover._StopJump = false;
                    }
                    else
                    {
                        baseMoveAICACSO.delayBeforeJump -= Time.deltaTime;
                    }
                }
                else // Active Jump 
                {
                    if (maxDurationNavLink > 0) // jump Current duration
                    {
                        maxDurationNavLink -= Time.deltaTime;
                    }
                    else // jump End duration
                    {
                        globalRef.agent.ActivateCurrentOffMeshLink(true);
                    }
                }
            }
            else // End Jump
            {
                globalRef.agentLinkMover._StopJump = true;
                baseMoveAICACSO.delayBeforeJump = baseMoveAICACSO.maxDelayBeforeJump;
                lookForwardJump = false;

                if (navLink != null)
                {
                    AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "EndJump");
                    globalRef.animEventAICAC.EndJump();
                    navLink.UpdateLink();
                    navLink = null;
                    maxDurationNavLink = globalRef.agentLinkMover._duration;
                }
            }
        }

        void BaseMovement()
        {
            if (isOnNavLink)
            {
                if (!triggerNavLink)
                {
                    linkDestination = navLink.transform.position - transform.position;
                    triggerNavLink = true;
                }
            }
            else
            {
                if(globalRef.offsetDestination !=0)
                {
                    offset = Mathf.Lerp(offset, globalRef.offsetDestination, baseMoveAICACSO.offsetTransitionSmooth * Time.deltaTime);
                    offset = Mathf.Clamp(offset, -Mathf.Abs(globalRef.offsetDestination), Mathf.Abs(globalRef.offsetDestination));
                }

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
                    if (activeSurround)
                    {
                        if (Vector3.Distance(globalRef.destinationSurround, globalRef.transform.position) > baseMoveAICACSO.distStopSurroundNearPlayer &&
                            Vector3.Distance(globalRef.transform.position, CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer) > baseMoveAICACSO.distStopSurroundNearPlayer)
                        {
                            destination = CheckNavMeshPoint(globalRef.destinationSurround);
                        }
                        else
                            activeSurround = false;
                    }
                    else
                    {
                        activeSurround = false;

                        if (baseMoveAICACSO.currentCoolDownAttack > 0)
                            CoolDownAttack();
                        else
                        {
                            dir = CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer - globalRef.transform.position;
                            left = Vector3.Cross(dir, Vector3.up).normalized;
                            Vector3 playerPosAnticip = CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer + (left * offset);
                            destination = CheckNavMeshPoint(playerPosAnticip);
                        }

                        if (Vector3.Distance(globalRef.playerTransform.position, globalRef.transform.position) > (globalRef.surroundManager.radius + baseMoveAICACSO.distStopSurroundNearPlayer))
                        {
                            activeSurround = true;
                        }
                    }

                    if(!isOnNavLink)
                    {
                        SpeedAdjusting();
                        SlowSpeed(globalRef.isInEylau || globalRef.IsZap);
                        globalRef.agent.SetDestination(destination);
                    }
                    currentRateRepath = maxRateRepath;
                }
            }

            if (Vector3.Distance(CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer, globalRef.transform.position) < baseMoveAICACSO.attackRange)//(globalRef.distPlayer < baseMoveAICACSO.attackRange)
            {
                if (!isOnNavLink)
                {
                    stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseAttack);
                }
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
                if (Vector3.Distance(destination, globalRef.transform.position) >= baseMoveAICACSO.distCanRun)
                {
                    if (baseMoveAICACSO.currentSpeed < baseMoveAICACSO.runSpeed)
                    {
                        baseMoveAICACSO.currentSpeed += baseMoveAICACSO.smoothSpeedRun * Time.deltaTime;
                    }
                    else
                        baseMoveAICACSO.currentSpeed = baseMoveAICACSO.runSpeed;
                }
                else if (Vector3.Distance(destination, globalRef.transform.position) <= baseMoveAICACSO.distStopRun)
                {
                    if (baseMoveAICACSO.currentSpeed > baseMoveAICACSO.baseSpeed)
                        baseMoveAICACSO.currentSpeed -= baseMoveAICACSO.smoothSpeedbase * Time.deltaTime;
                    else
                        baseMoveAICACSO.currentSpeed = baseMoveAICACSO.baseSpeed;
                }
                else
                {
                    if (baseMoveAICACSO.currentSpeed < baseMoveAICACSO.baseSpeed)
                    {
                        baseMoveAICACSO.currentSpeed += baseMoveAICACSO.smoothSpeedbase * Time.deltaTime;
                    }
                    else
                        baseMoveAICACSO.currentSpeed = baseMoveAICACSO.baseSpeed;
                }
            }
            else
            {
                if (baseMoveAICACSO.currentSpeed < baseMoveAICACSO.anticipSpeed)
                    baseMoveAICACSO.currentSpeed += baseMoveAICACSO.smoothSpeedAnticip * Time.deltaTime;
                else
                    baseMoveAICACSO.currentSpeed = baseMoveAICACSO.anticipSpeed;
            }
        }

        void SlowSpeed(bool active)
        {
            if(active)
            {
                globalRef.agent.speed = baseMoveAICACSO.currentSpeed / globalRef.slowRatio;
            }
            else
            {
                globalRef.agent.speed = baseMoveAICACSO.currentSpeed;
            }
        }

        void SmoothLookAt()
        {
            if(lookForwardJump)
            {
                relativePos.x = linkDestination.x;
                relativePos.y = 0;
                relativePos.z = linkDestination.z;

                SlowRotation(globalRef.isInEylau || globalRef.IsZap);
                Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), baseMoveAICACSO.speedRot);
                globalRef.transform.rotation = rotation;
            }
            else if(!isOnNavLink)
            {
                // direction = globalRef.transform.position + globalRef.agent.desiredVelocity;
                direction = globalRef.agent.desiredVelocity;

                relativePos.x = direction.x;
                relativePos.y = 0;
                relativePos.z = direction.z;

                SlowRotation(globalRef.isInEylau || globalRef.IsZap);
                Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), baseMoveAICACSO.speedRot);
                globalRef.transform.rotation = rotation;
            }
        }
        void SlowRotation(bool active)
        {
            if(active)
            {
                if (baseMoveAICACSO.speedRot < (baseMoveAICACSO.maxSpeedRot / globalRef.slowRatio))
                {
                    baseMoveAICACSO.speedRot += Time.deltaTime / (baseMoveAICACSO.smoothRot * globalRef.slowRatio);
                }
                else
                {
                    baseMoveAICACSO.speedRot = (baseMoveAICACSO.maxSpeedRot / globalRef.slowRatio);
                }
            }
            else
            {
                if (baseMoveAICACSO.speedRot < baseMoveAICACSO.maxSpeedRot)
                {
                    baseMoveAICACSO.speedRot += Time.deltaTime / baseMoveAICACSO.smoothRot;
                }
                else
                {
                    baseMoveAICACSO.speedRot = baseMoveAICACSO.maxSpeedRot;
                }
            }
        }

        private void OnDisable()
        {
            globalRef.baseAttackAICACSO.isAttacking = false;
            currentRateRepath = 0;

            if (baseMoveAICACSO != null)
            {
                baseMoveAICACSO.currentSpeed = 0;
                baseMoveAICACSO.speedRot = 0;
            }
        }
    }
}