using UnityEngine;
using UnityEngine.AI;

namespace State.AICAC
{
    public class DodgeStateAICAC : _StateAICAC
    {
        [SerializeField] GlobalRefAICAC globalRef;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.Dodge;
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

            if (globalRef.dodgeAICACSO.rightDodge) // choisi l esquive par la droite 
            {
                Vector3 dir = globalRef.dodgeAICACSO.targetObjectToDodge.position - globalRef.transform.position;
                Vector3 right = -Vector3.Cross(dir, Vector3.up).normalized;

                Ray ray = new Ray(globalRef.spawnRayDodge.position, right);
                globalRef.dodgeAICACSO.targetDodgeVector = ray.GetPoint(globalRef.dodgeAICACSO.dodgeLenght);
                globalRef.dodgeAICACSO.dodgeIsSet = true;

                Debug.DrawRay(globalRef.spawnRayDodge.position, right);
            }
            if (globalRef.dodgeAICACSO.leftDodge) // choisi l esquive par la gauche 
            {
                Vector3 dir = globalRef.dodgeAICACSO.targetObjectToDodge.position - globalRef.transform.position;
                Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;

                Ray ray = new Ray(globalRef.spawnRayDodge.position, left);
                globalRef.dodgeAICACSO.targetDodgeVector = ray.GetPoint(globalRef.dodgeAICACSO.dodgeLenght);
                globalRef.dodgeAICACSO.dodgeIsSet = true;
                Debug.DrawRay(globalRef.spawnRayDodge.position, left);
            }
        }

        // fonction qui renvoie vrai si le point "dodgePoint" se trouve sur le NavMesh et faux si ce n est pas le cas
        bool DetectDodgePointIsOnNavMesh(Vector3 dodgePoint)
        {
            if (NavMesh.SamplePosition(dodgePoint, out globalRef.dodgeAICACSO.navHit, 0.1f, NavMesh.AllAreas))
            {
                Debug.Log("point on nav mesh");
                return true;
            }
            else
            {
                Debug.Log("point out nav mesh");
                return false;
            }
        }

        // Apply Dodge movement
        public void Dodge()
        {
            float distDodgeDestination;
            Vector2 pos2DAI;
            Vector2 posDodgeDestination2D;

            pos2DAI.x = globalRef.transform.position.x;
            pos2DAI.y = globalRef.transform.position.z;

            posDodgeDestination2D.x = globalRef.dodgeAICACSO.targetDodgeVector.x;
            posDodgeDestination2D.y = globalRef.dodgeAICACSO.targetDodgeVector.z;

            distDodgeDestination = Vector2.Distance(pos2DAI, posDodgeDestination2D);

            //Debug.Log(distDodgeDestination);
            if (distDodgeDestination > 1.1f)
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

            if (globalRef.agent.velocity.magnitude == 0f)
            {
                StopDodge();
            }
        }
        void StopDodge()
        {
            stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseIdle);
            /*if (!globalRef.dodgeAICACSO.dodgeRushBull)
            {
                //globalRef.SwitchToNewState(0);
                stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseIdle);
            }
            else
            {
                //stateManagerAICAC.SwitchToNewState(5);
            }*/
        }

        public void SmoothLookAt()
        {
            Vector3 direction;
            Vector3 relativePos;

            direction = globalRef.playerTransform.position;
            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

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
            globalRef.dodgeAICACSO.dodgeRushBull = false;
            globalRef.dodgeAICACSO.speedRot = 0;
            globalRef.dodgeAICACSO.leftDodge = false;
            globalRef.dodgeAICACSO.rightDodge = false;
            globalRef.dodgeAICACSO.isDodging = false;
            globalRef.dodgeAICACSO.dodgeIsSet = false;
        }
    }
}