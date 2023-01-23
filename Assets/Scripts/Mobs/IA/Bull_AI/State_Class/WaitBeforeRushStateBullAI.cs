using UnityEngine;
using UnityEngine.AI;

namespace State.AIBull
{
    public class WaitBeforeRushStateBullAI : _StateBull
    {
        [SerializeField] BaseRushStateBullAI baseRushStateBullAI;
        [SerializeField] GlobalRefBullAI globalRef;
        RaycastHit hit;

        [SerializeField] float maxDurationNavLink;
        [SerializeField] bool linkIsActive;
        NavMeshLink navLink;
        bool triggerNavLink;

        Vector3 destination;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.WaitBeforeRush;
        }

        private void OnEnable()
        {
            try 
            { 
                globalRef.agent.speed = globalRef.coolDownRushBullSO.speedPatrolToStartPos;
                PreSelectingStartPos();
            }
            catch
            {
                Debug.LogWarning("missing Reference");
            }
        }

        private void Update()
        {
            ManageCurrentNavMeshLink();

            if(globalRef.coolDownRushBullSO.currentNumberOfPatrol < globalRef.coolDownRushBullSO.maxNumberOfPatrol)
            {
                GoToStartRushPos();
            }
            else
            {
                SwitchToStateRush();
            }

            SmoothLookAtPlayer();
        }
        private void FixedUpdate()
        {
            CheckObstacle();
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

        void GoToStartRushPos()
        {
            if (globalRef.coolDownRushBullSO.startPos == Vector3.zero) // cherche une nouvelle position si aucune n est defifni
            {
                SelectStartPos();
            }
            else if (globalRef.agent.remainingDistance > 0.5f) // avance vers la position
            {
                globalRef.agent.SetDestination(CheckNavMeshPoint(globalRef.coolDownRushBullSO.startPos));
            }

            if (globalRef.coolDownRushBullSO.startPos != Vector3.zero && globalRef.agent.remainingDistance <= 1f)
            {
                if (globalRef.coolDownRushBullSO.currentDurationStay > 0)
                {
                    globalRef.coolDownRushBullSO.currentDurationStay -= Time.deltaTime;
                }
                else
                {
                    globalRef.coolDownRushBullSO.currentNumberOfPatrol++;
                    PreSelectingStartPos();
                }
            }
        }

        void PreSelectingStartPos()
        {
            Debug.Log("count");

            globalRef.coolDownRushBullSO.currentDurationStay = globalRef.coolDownRushBullSO.maxDurationStay;
            globalRef.bullAIStartPosRush.ResetSelectedBox(globalRef.coolDownRushBullSO.boxSelected);
            SelectStartPos();
        }
        void SelectStartPos()
        {
            globalRef.bullAIStartPosRush.SelectAI(globalRef);
            globalRef.agent.speed = globalRef.coolDownRushBullSO.speedPatrolToStartPos;
            globalRef.agent.SetDestination(CheckNavMeshPoint(globalRef.coolDownRushBullSO.startPos));
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

        void CheckObstacle()
        {
            if (globalRef.coolDownRushBullSO.currentCoolDownCheckObstacle <= 0)
            {
                hit = RaycastAIManager.RaycastAI(globalRef.transform.position, globalRef.transform.forward, globalRef.coolDownRushBullSO.mask, Color.red, globalRef.coolDownRushBullSO.distDetect);

                if (hit.transform != null)
                {
                    globalRef.coolDownRushBullSO.currentCoolDownCheckObstacle = globalRef.coolDownRushBullSO.maxCoolDownCheckObstacle;
                    SelectStartPos();
                }
            }
            else
            {
                globalRef.coolDownRushBullSO.currentCoolDownCheckObstacle -= Time.fixedDeltaTime;
            }
        }

        void SwitchToStateRush()
        {
            stateController.SetActiveState(StateControllerBull.AIState.Rush);
        }

        Vector3 SwitchToLoockLinkDestination()
        {
            NavMeshLink link;
            link = globalRef.agent.navMeshOwner as NavMeshLink;

            if (!triggerNavLink)
            {
                if (Vector3.Distance(globalRef.transform.position, link.startPoint) < Vector3.Distance(globalRef.transform.position, link.endPoint))
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
            return destination;
        }

        void SmoothLookAtPlayer()
        {
            Vector3 direction;
            Vector3 relativePos;

            if (globalRef.agent.isOnOffMeshLink)
            {
                direction = SwitchToLoockLinkDestination();
            }
            else
            {
                triggerNavLink = false;

                if(globalRef.agent.isActiveAndEnabled)
                {
                    if (globalRef.agent.remainingDistance > 1)
                        direction = globalRef.transform.position + globalRef.agent.desiredVelocity;
                    else
                        direction = globalRef.playerTransform.position;
                }
                else
                    direction = globalRef.playerTransform.position;
            }

            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            if (globalRef.coolDownRushBullSO.speedRot < globalRef.coolDownRushBullSO.maxSpeedRot)
                globalRef.coolDownRushBullSO.speedRot += Time.deltaTime / globalRef.coolDownRushBullSO.smoothRot;
            else
            {
                globalRef.coolDownRushBullSO.speedRot = globalRef.coolDownRushBullSO.maxSpeedRot;
            }

            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.coolDownRushBullSO.speedRot);
            globalRef.transform.rotation = rotation;
        }

        private void OnDisable()
        {
            baseRushStateBullAI.captureBasePosDistance = globalRef.transform.position;

            globalRef.bullAIStartPosRush.ResetSelectedBox(globalRef.coolDownRushBullSO.boxSelected);
            //globalRef.coolDownRushBullSO.boxSelected = null;
            globalRef.coolDownRushBullSO.currentNumberOfPatrol = 0;
            globalRef.agent.speed = globalRef.coolDownRushBullSO.stopSpeed;
            /*globalRef.coolDownRushBullSO.rushDestination = globalRef.playerTransform.position + globalRef.transform.forward * globalRef.rushBullSO.rushInertieSetDistance;
            globalRef.rushBullSO.rushDestination = globalRef.playerTransform.position + globalRef.transform.forward * globalRef.rushBullSO.rushInertieSetDistance;
            globalRef.agent.SetDestination(globalRef.coolDownRushBullSO.rushDestination);*/
            globalRef.coolDownRushBullSO.startPos = Vector3.zero;
            globalRef.coolDownRushBullSO.speedRot = 0;
            globalRef.coolDownRushBullSO.currentDurationStay = globalRef.coolDownRushBullSO.maxDurationStay;
        }
    }

    public class CoolDown
    {
        float _maxTime;
        System.Action _CallBack;
        public bool isDone => _maxTime <= 0;


        public CoolDown(float maxTim, System.Action CallBack)
        {
            _maxTime = maxTim;
            _CallBack = CallBack;
        }

        public bool Progress(float elapsedTime)
        {
            _maxTime -= elapsedTime;

            if (_maxTime <= 0)
            {
                _CallBack?.Invoke();
                return true;
            }
            else
                return false;
        }
    }
}