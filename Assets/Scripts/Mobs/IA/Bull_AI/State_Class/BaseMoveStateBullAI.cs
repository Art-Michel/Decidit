using UnityEngine;
using UnityEngine.AI;

namespace State.AIBull
{
    public class BaseMoveStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;
        [SerializeField] float offsetYposPlayer;
        NavMeshQueryFilter filter;
        NavMeshPath path;
        [SerializeField] Vector3 Destination;
        [SerializeField] bool variantMove;

        [Header("Nav Link")]
        [SerializeField] float maxDurationNavLink;
        bool triggerNavLink;
        public bool isOnNavLink;
        NavMeshLink link;
        NavMeshLink navLink;
        NavMeshHit closestHit;
        Vector3 linkDestination;
        bool lookForwardJump;
        [SerializeField] bool hitPlayer;

        [SerializeField] float distDetectObstacle;

        [Header("Animation")]
        AnimatorStateInfo animStateInfo;
        AnimatorClipInfo[] currentClipInfo;
        [SerializeField] string currentAnimName;
        [SerializeField] float animTime;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.BaseMove;
        }

        private void Start()
        {
            filter.agentTypeID = GetNavMeshFilter("GroundAI");
            filter.areaMask = 1 << 0;
        }

        private void OnEnable()
        {
            if (globalRef != null && globalRef.myAnimator != null)
                AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Walk");

            ResetCoolDown();
        }

        void ResetCoolDown()
        {
            globalRef.rushBullSO.currentRangeTimeRush = (int)Random.Range(globalRef.rushBullSO.rangeTimerRush.x, globalRef.rushBullSO.rangeTimerRush.y);
        }

        private void Update()
        {
            DecrementCoolDownRush();

            ManageCurrentNavMeshLink();
            SmoothLookAtPlayer();
            ManageJumpAnimation();

            if (!isOnNavLink)
            {
                BaseMovement();

                if (globalRef.distPlayer < globalRef.baseMoveBullSO.distActiveRush && globalRef.distPlayer > globalRef.baseMoveBullSO.distActiveAttack)
                {
                    globalRef.launchRush = true;
                }
                else
                {
                    if(globalRef.distPlayer < globalRef.baseMoveBullSO.distActiveAttack && hitPlayer)
                    {
                        stateController.SetActiveState(StateControllerBull.AIState.BaseAttack);
                        //stateController.SetActiveState(StateControllerBull.AIState.Rush);
                    }
                }
            }
        }

        void DecrementCoolDownRush()
        {
            if (globalRef.rushBullSO.currentRangeTimeRush > 0)
            {
                globalRef.rushBullSO.currentRangeTimeRush -= Time.deltaTime;
            }
            else
            {
                LaunchRush();
            }
        }

        private void FixedUpdate()
        {
            CheckObstacle();
        }
        void CheckObstacle()
        {
            RaycastHit hit = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.rayCheckRush.position, 
                                                new Vector3(globalRef.playerTransform.position.x, globalRef.playerTransform.position.y- offsetYposPlayer, globalRef.playerTransform.position.z)
                                                - globalRef.rayCheckRush.position,
                                                   globalRef.rushBullSO.maskCheckCanRush, Color.blue,
                                                    Vector3.Distance(globalRef.transform.position, globalRef.playerTransform.position));
            if(hit.transform == globalRef.playerTransform)
            {
                /*if(globalRef.launchRush)
                    LaunchRush();*/

                hitPlayer = true;
            }
            else
            {
                hitPlayer = false;
            }
        }

        #region Jump
        void ManageCurrentNavMeshLink()
        {
            if (currentAnimName == "Run" && animTime > 1)
            {
                isOnNavLink = false;
            }

            if (globalRef.agent.isOnOffMeshLink)
            {
                globalRef.agentLinkMover.enabled = true;
                lookForwardJump = true;
                globalRef.agent.autoTraverseOffMeshLink = false;

                if (!isOnNavLink)
                {
                    isOnNavLink = true;
                    globalRef.agent.speed = 0;
                }

                if (navLink == null)
                {
                    globalRef.agent.ActivateCurrentOffMeshLink(false);
                    navLink = globalRef.agent.navMeshOwner as NavMeshLink;
                    linkDestination = navLink.transform.position - transform.position;
                    if (navLink != null)
                    {
                        globalRef.agentLinkMover.m_Curve.AddKey(0.5f, Mathf.Abs((navLink.endPoint.y - navLink.startPoint.y) / 1.5f));
                        globalRef.agentLinkMover._height = Mathf.Abs((navLink.endPoint.y - navLink.startPoint.y) / 1.5f);
                    }
                    else
                    {
                        globalRef.agentLinkMover._StopJump = false;
                    }
                }

                if (globalRef.agentLinkMover._StopJump)
                {
                    if (globalRef.globalRefAnimator.currentAnimName != "StartJump")
                        AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "StartJump");

                    if (globalRef.baseMoveBullSO.delayBeforeJump <= 0)
                    {
                        globalRef.agentLinkMover._StopJump = false;
                    }
                    else
                    {
                        globalRef.baseMoveBullSO.delayBeforeJump -= Time.deltaTime;
                    }
                }
                else
                {
                    if (maxDurationNavLink >= 0) // jump Current duration
                    {
                        maxDurationNavLink -= Time.deltaTime;
                    }
                    else // jump End duration
                    {
                        AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "EndJump");
                        globalRef.agentLinkMover._StopJump = true;
                    }
                }   
            }
            else
            {
                globalRef.agent.ActivateCurrentOffMeshLink(true);
                globalRef.agentLinkMover._StopJump = true;
                globalRef.baseMoveBullSO.delayBeforeJump = globalRef.baseMoveBullSO.maxDelayBeforeJump;
                lookForwardJump = false;

                if (navLink != null)
                {
                    AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "EndJump");
                    DisableJump();
                    navLink.UpdateLink();
                    navLink = null;
                    maxDurationNavLink = globalRef.agentLinkMover._duration;
                }
                else if (CheckEndAnimation("Jump Fall"))
                {
                    Invoke("ActiveJump", 1f);
                    ReturnWalkState();
                }
            }
        }
        void ManageJumpAnimation()
        {
            currentClipInfo = globalRef.myAnimator.GetCurrentAnimatorClipInfo(0);
            currentAnimName = currentClipInfo[0].clip.name;

            animStateInfo = globalRef.myAnimator.GetCurrentAnimatorStateInfo(0);
            animTime = animStateInfo.normalizedTime;
        }
        bool CheckEndAnimation(string animName)
        {
            if (animTime > 1.0f && currentAnimName == animName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        void ReturnWalkState()
        {
            isOnNavLink = false;
           // globalRef.agent.autoTraverseOffMeshLink = true;
            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Walk");
        }
        void ActiveJump()
        {
            globalRef.agent.areaMask |= (1 << NavMesh.GetAreaFromName("Jump"));
        }
        void DisableJump()
        {
            globalRef.agent.areaMask &= ~(1 << NavMesh.GetAreaFromName("Jump"));
        }
        #endregion


        void BaseMovement()
        {
            Vector3 newDestination;
            float distPlayer = Vector3.Distance(globalRef.transform.position, globalRef.playerTransform.position);
            globalRef.agent.speed = globalRef.baseMoveBullSO.baseSpeed;

            if(distPlayer > 10 && variantMove)
            {
                Vector3 dir = globalRef.playerTransform.position - globalRef.transform.position;
                Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
                newDestination = globalRef.playerTransform.position + (left * globalRef.offsetDestination);
            }
            else
            {
                newDestination = globalRef.playerTransform.position;
            }

            SlowSpeed(globalRef.isInEylau || globalRef.IsZap);

            path = new NavMeshPath();
            Destination = CheckNavMeshPoint(newDestination);
            globalRef.agent.CalculatePath(Destination, path);
            globalRef.agent.SetDestination(Destination);
        }

        int GetNavMeshFilter(string name)
        {
            for (int i = 0; i < NavMesh.GetSettingsCount(); ++i)
            {
                var settings = NavMesh.GetSettingsByIndex(i);

                int agentTypeID = settings.agentTypeID;

                var settingsName = NavMesh.GetSettingsNameFromID(agentTypeID);

                if (settingsName == name)
                {
                    // Debug.Log(settings.agentTypeID);
                    return settings.agentTypeID;
                }

            }//end for
            return 0;
        }
        Vector3 CheckNavMeshPoint(Vector3 newDestination)
        {
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(newDestination, out closestHit, 1000, filter))
            {
                newDestination = closestHit.position;
            }
            return newDestination;
        }
        void SlowSpeed(bool active)
        {
            if (active)
            {
                globalRef.slowSpeed = globalRef.agent.speed / globalRef.slowRatio;
                globalRef.agent.speed = globalRef.slowSpeed;
            }
            else
            {
                if (globalRef.agent.speed == globalRef.slowRatio)
                    globalRef.agent.speed *= globalRef.slowRatio;
            }
        }

        void LaunchRush()
        {
            if(globalRef.distPlayer < globalRef.rushBullSO.rushDistance && globalRef.distPlayer > globalRef.baseAttackBullSO.distLaunchAttackState && hitPlayer && !isOnNavLink)
                stateController.SetActiveState(StateControllerBull.AIState.Rush);

            if (!hitPlayer)
                variantMove = false;

        }

        void SmoothLookAtPlayer()
        {
            Vector3 direction;
            Vector3 relativePos;

            if(lookForwardJump)
            {
                direction = linkDestination;

                relativePos.x = direction.x;
                relativePos.y = 0;
                relativePos.z = direction.z;

                SlowRotation(globalRef.isInEylau);
                Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.baseMoveBullSO.speedRot);
                globalRef.transform.rotation = rotation;
            }
            else if(!isOnNavLink)
            {
                direction = globalRef.agent.desiredVelocity;

                relativePos.x = direction.x;
                relativePos.y = 0;
                relativePos.z = direction.z;

                SlowRotation(globalRef.isInEylau || globalRef.IsZap);
                Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.baseMoveBullSO.speedRot);
                globalRef.transform.rotation = rotation;
            }
        }
        void SlowRotation(bool active)
        {
            if (active)
            {
                if (globalRef.baseMoveBullSO.speedRot < (globalRef.baseMoveBullSO.maxSpeedRot / globalRef.slowRatio))
                {
                    globalRef.baseMoveBullSO.speedRot += Time.deltaTime / (globalRef.baseMoveBullSO.smoothRot * globalRef.slowRatio);
                }
                else
                {
                    globalRef.baseMoveBullSO.speedRot = (globalRef.baseMoveBullSO.maxSpeedRot / globalRef.slowRatio);
                }
            }
            else
            {
                if (globalRef.baseMoveBullSO.speedRot < globalRef.baseMoveBullSO.maxSpeedRot)
                {
                    globalRef.baseMoveBullSO.speedRot += Time.deltaTime / globalRef.baseMoveBullSO.smoothRot;
                }
                else
                {
                    globalRef.baseMoveBullSO.speedRot = globalRef.baseMoveBullSO.maxSpeedRot;
                }
            }
        }

        private void OnDisable()
        {
            globalRef.baseMoveBullSO.speedRot = 0;
            globalRef.agent.speed = globalRef.baseMoveBullSO.stopSpeed;
            variantMove = true;
            ResetCoolDown();
        }
    }
}