using UnityEngine;
using UnityEngine.AI;

namespace State.AIBull
{
    public class BaseMoveStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;
        [SerializeField] float offsetYposPlayer;

        [Header("Nav Link")]
        [SerializeField] float maxDurationNavLink;
        bool triggerNavLink;
        public bool isOnNavLink;
        NavMeshLink link;
        NavMeshLink navLink;
        NavMeshHit closestHit;
        Vector3 linkDestination;
        bool lookForwardJump;
        bool hitPlayer;
        bool disableOffsetDestination;

        [SerializeField] float distDetectObstacle;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.BaseMove;
        }

        private void OnEnable()
        {
            disableOffsetDestination = false;

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

        void ManageCurrentNavMeshLink()
        {
            if (globalRef.agent.isOnOffMeshLink)
            {
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
                    globalRef.agentLinkMover.m_Curve.AddKey(0.5f, Mathf.Abs((navLink.endPoint.y - navLink.startPoint.y) / 1.5f));
                    globalRef.agentLinkMover._height = Mathf.Abs((navLink.endPoint.y - navLink.startPoint.y) / 1.5f);
                }

                if (globalRef.agentLinkMover._StopJump)
                {
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
                    if (maxDurationNavLink > 0) // jump Current duration
                    {
                        maxDurationNavLink -= Time.deltaTime;
                    }
                    else // jump End duration
                    {
                        Debug.Log("End Jump");
                        globalRef.agent.ActivateCurrentOffMeshLink(true);
                        AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "EndJump");
                    }
                }   
            }
            else
            {
                globalRef.agentLinkMover._StopJump = true;
                globalRef.baseMoveBullSO.delayBeforeJump = globalRef.baseMoveBullSO.maxDelayBeforeJump;
                lookForwardJump = false;

                if (navLink != null)
                {
                    AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "EndJump");
                    globalRef.animEventRusher.EndJump();
                    navLink.UpdateLink();
                    navLink = null;
                    maxDurationNavLink = globalRef.agentLinkMover._duration;
                }
            }
        }

        void BaseMovement()
        {
            Vector3 newDestination;
            float distPlayer = Vector3.Distance(globalRef.transform.position, globalRef.playerTransform.position);
            globalRef.agent.speed = globalRef.baseMoveBullSO.baseSpeed;

            if(distPlayer > Mathf.Abs(globalRef.offsetDestination)+1 && hitPlayer && !disableOffsetDestination)
            {
                newDestination = globalRef.playerTransform.position + (globalRef.playerTransform.right * globalRef.offsetDestination);
            }
            else
            {
                newDestination = globalRef.playerTransform.position;
                disableOffsetDestination = true;
            }

            SlowSpeed(globalRef.isInEylau);
            globalRef.agent.SetDestination(CheckNavMeshPoint(newDestination));
        }
        Vector3 CheckNavMeshPoint(Vector3 newDestination)
        {
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(newDestination, out closestHit, 1, 1))
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
            if(globalRef.distPlayer < globalRef.rushBullSO.distRush && globalRef.distPlayer > globalRef.baseAttackBullSO.distLaunchAttackState && hitPlayer)
                stateController.SetActiveState(StateControllerBull.AIState.Rush);
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

                SlowRotation(globalRef.isInEylau);
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
            ResetCoolDown();
        }
    }
}