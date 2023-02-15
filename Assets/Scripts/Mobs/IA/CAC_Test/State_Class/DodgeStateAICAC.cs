using UnityEngine;
using UnityEngine.AI;

namespace State.AICAC
{
    public class DodgeStateAICAC : _StateAICAC
    {
        [SerializeField] GlobalRefAICAC globalRef;

        NavMeshHit closestHit;
        [SerializeField] LayerMask noMask;

        bool checkMove;

        [Header("Diretcion Move")]
        float distDodgeDestination;
        Vector2 pos2DAI;
        Vector2 posDodgeDestination2D;
        Vector3 dir;
        Vector3 right;
        Vector3 left;

       [Header("Diretcion LoookAt")]
        Vector3 direction;
        Vector3 relativePos;

        float durationDodge;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.Dodge;
        }

        private void OnEnable()
        {
            durationDodge = globalRef.dodgeAICACSO.dodgeLenght / globalRef.dodgeAICACSO.dodgeSpeed;

            Invoke("CheckISMoving", 1f);
            Invoke("DodgeMaxDuration", durationDodge);
        }

        void CheckISMoving()
        {
            checkMove = true;
        }

        void DodgeMaxDuration()
        {
            StopDodge();
        }

        private void Update()
        {
            SmoothLookAt();
            if (globalRef.dodgeAICACSO.dodgeIsSet)
                Dodge();
        }
        private void FixedUpdate()
        {
            if (!globalRef.dodgeAICACSO.dodgeIsSet)
                SetDodgePosition();
            else
            {
                RaycastHit hit = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.dodgeAICACSO.targetDodgeVector, Vector3.down, noMask, Color.red, 100f);
                if (DetectDodgePointIsOnNavMesh(hit.point) == false)
                {
                    if (globalRef.dodgeAICACSO.leftDodge) // choisi l esquive par la droite 
                    {
                        GetRightPoint();
                    }
                    else // choisi l esquive par la gauche 
                    {
                        GetLeftPoint();
                    }
                }
            }
        }

        float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
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

        public void SetDodgePosition()
        {
            globalRef.dodgeAICACSO.targetObjectToDodge = globalRef.playerTransform;

            if (!globalRef.dodgeAICACSO.leftDodge && !globalRef.dodgeAICACSO.rightDodge)
            {
                if (Random.Range(0, 2) == 0)
                {
                    globalRef.dodgeAICACSO.leftDodge = false;
                    globalRef.dodgeAICACSO.rightDodge = true;
                }
                else
                {
                    globalRef.dodgeAICACSO.rightDodge = false;
                    globalRef.dodgeAICACSO.leftDodge = true;
                }
            }
            else
            {
                if (globalRef.dodgeAICACSO.rightDodge) // choisi l esquive par la droite 
                {
                    GetRightPoint();
                }
                else // choisi l esquive par la gauche 
                {
                    GetLeftPoint();
                }
            }
        }

        void GetRightPoint()
        {
            dir = globalRef.dodgeAICACSO.targetObjectToDodge.position - globalRef.transform.position;
            right = -Vector3.Cross(dir, Vector3.up).normalized;

            Ray ray = new Ray(globalRef.spawnRayDodge.position, right);
            globalRef.dodgeAICACSO.targetDodgeVector = CheckNavMeshPoint(ray.GetPoint(globalRef.dodgeAICACSO.dodgeLenght));
            globalRef.dodgeAICACSO.dodgeIsSet = true;
        }
        void GetLeftPoint()
        {
            dir = globalRef.dodgeAICACSO.targetObjectToDodge.position - globalRef.transform.position;
            left = Vector3.Cross(dir, Vector3.up).normalized;

            Ray ray = new Ray(globalRef.spawnRayDodge.position, left);
            globalRef.dodgeAICACSO.targetDodgeVector = CheckNavMeshPoint(ray.GetPoint(globalRef.dodgeAICACSO.dodgeLenght));
            globalRef.dodgeAICACSO.dodgeIsSet = true;
        }

        // fonction qui renvoie vrai si le point "dodgePoint" se trouve sur le NavMesh et faux si ce n est pas le cas
        bool DetectDodgePointIsOnNavMesh(Vector3 dodgePoint)
        {
            if (NavMesh.SamplePosition(dodgePoint, out globalRef.dodgeAICACSO.navHit, 1f, NavMesh.AllAreas))
            {
                return true;
            }
            else
            {
                if (globalRef.dodgeAICACSO.rightDodge) // choisi l esquive par la droite 
                {
                    GetLeftPoint();
                    return false;
                }
                else // choisi l esquive par la gauche 
                {
                    GetRightPoint();
                    return false;
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

        // Apply Dodge movement
        public void Dodge()
        {
            pos2DAI.x = globalRef.transform.position.x;
            pos2DAI.y = globalRef.transform.position.z;

            posDodgeDestination2D.x = globalRef.dodgeAICACSO.targetDodgeVector.x;
            posDodgeDestination2D.y = globalRef.dodgeAICACSO.targetDodgeVector.z;

            distDodgeDestination = Vector2.Distance(pos2DAI, posDodgeDestination2D);

            if (distDodgeDestination > 1.1f && globalRef.agent.enabled)
            {
                globalRef.baseMoveAICACSO.speedRot = 0;
                globalRef.agent.speed = globalRef.dodgeAICACSO.dodgeSpeed;
                globalRef.dodgeAICACSO.isDodging = true;
                globalRef.agent.SetDestination(globalRef.dodgeAICACSO.targetDodgeVector);
            }
            else
            {
                StopDodge();
            }

            if(checkMove)
            {
                if(globalRef.agent.velocity.magnitude == 0)
                {
                    StopDodge();
                }
            }
        }
        void StopDodge()
        {
            if(stateControllerAICAC != null)
                stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseMove);
        }

        public void SmoothLookAt()
        {
            //direction = globalRef.playerTransform.position;
            //direction = globalRef.transform.forward;
            direction = globalRef.agent.desiredVelocity;
            relativePos.x = direction.x;
            relativePos.y = 0;
            relativePos.z = direction.z;

            if (globalRef.dodgeAICACSO.speedRot < globalRef.dodgeAICACSO.maxSpeedRot)
                globalRef.dodgeAICACSO.speedRot += Time.deltaTime / globalRef.dodgeAICACSO.smoothRot;
            else
            {
                globalRef.dodgeAICACSO.speedRot = globalRef.dodgeAICACSO.maxSpeedRot;
            }

            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.dodgeAICACSO.speedRot);
            globalRef.transform.rotation = rotation;
        }

        private void OnDisable()
        {
            globalRef.dodgeAICACSO.speedRot = 0;
            globalRef.dodgeAICACSO.leftDodge = false;
            globalRef.dodgeAICACSO.rightDodge = false;
            globalRef.dodgeAICACSO.isDodging = false;
            globalRef.dodgeAICACSO.dodgeIsSet = false;
            globalRef.agent.SetDestination(globalRef.playerTransform.position);
        }
    }
}