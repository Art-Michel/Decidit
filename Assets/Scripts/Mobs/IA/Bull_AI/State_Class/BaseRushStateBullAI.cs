using UnityEngine;
using UnityEngine.AI;
using State.AICAC;

namespace State.AIBull
{
    public class BaseRushStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;
        RaycastHit hit;


        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.Rush;
        }

        private void Update()
        {
            RushMovement();
        }
        private void FixedUpdate()
        {
            CheckObstacleOnPath();
        }

        public void RushMovement()
        {
            if (globalRef.distPlayer < globalRef.rushBullSO.stopLockDist)
            {
                globalRef.rushBullSO.stopLockPlayer = true;
            }
            else if (!globalRef.rushBullSO.stopLockPlayer)
            {
                globalRef.rushBullSO.rushDestination = globalRef.playerTransform.position + globalRef.transform.forward * globalRef.rushBullSO.rushInertieSetDistance;
            }

            globalRef.detectOtherAICollider.enabled = true;
            globalRef.agent.speed = globalRef.rushBullSO.rushSpeed;
            globalRef.agent.SetDestination(CheckNavMeshPoint(globalRef.rushBullSO.rushDestination));
            globalRef.hitBox.SetActive(true);

            if (globalRef.agent.remainingDistance == 0)
            {
                StopRush();
            }

            SmoothLookAtPlayer();
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

        public void CheckObstacleOnPath()
        {
            hit = RaycastAIManager.RaycastAI(globalRef.transform.position, globalRef.transform.forward, globalRef.rushBullSO.maskCheckObstacle, Color.red, 2f);

            if (hit.transform != null)
            {
                Debug.Log("Obstacle Stop Rush");
                StopRush();
            }
        }

        void StopRush()
        {
            stateController.SetActiveState(StateControllerBull.AIState.Idle);
        }

        void SmoothLookAtPlayer()
        {
            Vector3 direction;
            Vector3 relativePos;

            direction = globalRef.agent.destination;

            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            globalRef.rushBullSO.speedRot = globalRef.rushBullSO.maxSpeedRot;

            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.rushBullSO.speedRot);
            globalRef.transform.rotation = rotation;
        }

        private void OnDisable()
        {

            globalRef.detectOtherAICollider.enabled = false;
            globalRef.rushBullSO.stopLockPlayer = false;
            globalRef.hitBox.SetActive(false);

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