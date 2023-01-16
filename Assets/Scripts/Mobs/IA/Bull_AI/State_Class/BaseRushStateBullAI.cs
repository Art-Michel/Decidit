using UnityEngine;
using UnityEngine.AI;
using State.AICAC;

namespace State.AIBull
{
    public class BaseRushStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;
        RaycastHit hit;

        [SerializeField] bool lockPlayer;

        [SerializeField] float maxDurationNavLink;
        [SerializeField] bool linkIsActive;
        NavMeshLink navLink;
        bool triggerNavLink;

        Vector3 destination;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.Rush;
        }

        private void OnEnable()
        {
            Debug.Log("Rush");

            try
            {
                globalRef.rushBullSO.rushCurrentDuration = globalRef.rushBullSO.rushMaxDuration;
            }
            catch
            {
                Debug.LogWarning("missing Reference");
            }
        }

        private void Update()
        {
            RushMovement();
            RushDuration();
            //ManageCurrentNavMeshLink();
        }
        private void FixedUpdate()
        {
            CheckPlayerDistance();
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

        void RushDuration()
        {
            if (globalRef.hitBox.Blacklist.Count > 0) // player is hit
            {
                StopRush();
            }
        }

        public void RushMovement()
        {
            if (!lockPlayer)
            {
                globalRef.rushBullSO.rushDestination = globalRef.playerTransform.position + globalRef.transform.forward * globalRef.rushBullSO.rushInertieSetDistance;
                lockPlayer = true;
            }

            globalRef.detectOtherAICollider.enabled = true;
            globalRef.agent.speed = globalRef.rushBullSO.rushSpeed;
            globalRef.agent.SetDestination(CheckNavMeshPoint(globalRef.rushBullSO.rushDestination));
            globalRef.hitBox.gameObject.SetActive(true);

            SmoothLookAtPlayer();
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

        void CheckPlayerDistance()
        {
            hit = RaycastAIManager.RaycastAI(globalRef.transform.position, globalRef.transform.forward, globalRef.rushBullSO.noMask, Color.red, 1f);

            if (hit.transform != null)
            {
                if (hit.transform.CompareTag("Wall"))
                {
                    StopRush();
                }
            }
        }

        void StopRush()
        {
            Debug.Log("StopRush");
            stateController.SetActiveState(StateControllerBull.AIState.Idle);
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
                direction = globalRef.transform.position + globalRef.agent.desiredVelocity;
            }

            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            globalRef.rushBullSO.speedRot = globalRef.rushBullSO.maxSpeedRot;

            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.rushBullSO.speedRot);
            globalRef.transform.rotation = rotation;
        }

        private void OnDisable()
        {
            lockPlayer = false;
            globalRef.detectOtherAICollider.enabled = false;
            globalRef.rushBullSO.stopLockPlayer = false;
            globalRef.hitBox.gameObject.SetActive(false);

            globalRef.rushBullSO.speedRot = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ennemi") && gameObject.activeInHierarchy)
            {
                Debug.Log("Get TrashMob");

                if (!globalRef.rushBullSO.ennemiInCollider.Contains(other.gameObject) || globalRef.rushBullSO.ennemiInCollider == null)
                    globalRef.rushBullSO.ennemiInCollider.Add(other.gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Ennemi") && gameObject.activeInHierarchy)
            {
                if (globalRef.rushBullSO.ennemiInCollider != null)
                {
                    for (int i = 0; i < globalRef.rushBullSO.ennemiInCollider.Count; i++)
                    {
                        if (globalRef.rushBullSO.ennemiInCollider[i].GetComponent<GlobalRefAICAC>() != null)
                        {
                            GlobalRefAICAC globalRefAICAC = globalRef.rushBullSO.ennemiInCollider[i].GetComponent<GlobalRefAICAC>();

                            RaycastHit hit = RaycastAIManager.RaycastAI(transform.position, transform.forward, globalRef.noMask, Color.red, 10f);
                            float angle;
                            angle = Vector3.SignedAngle(transform.forward, hit.normal, Vector3.up);

                            if (angle > 0)
                            {
                                Debug.Log("Dodge TrashMob");

                                globalRefAICAC.dodgeAICACSO.targetObjectToDodge = this.transform;
                                globalRefAICAC.dodgeAICACSO.rightDodge = true;
                                globalRefAICAC.ActiveStateDodge();
                            }
                            else
                            {
                                Debug.Log("Dodge TrashMob");
                                globalRefAICAC.dodgeAICACSO.targetObjectToDodge = this.transform;
                                globalRefAICAC.dodgeAICACSO.leftDodge = true;
                                globalRefAICAC.dodgeAICACSO.dodgeRushBull = true;
                                globalRefAICAC.ActiveStateDodge();
                            }
                        }
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ennemi"))
            {
                globalRef.rushBullSO.ennemiInCollider.Remove(other.gameObject);
            }
        }


        public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
        {
            Vector3 perp = Vector3.Cross(fwd, targetDir);
            float dir = Vector3.Dot(perp, up);

            if (dir > 0.0f)
            {
                return 1.0f;
            }
            else if (dir < 0.0f)
            {
                return -1.0f;
            }
            else
            {
                return 0.0f;
            }
        }
    }
}