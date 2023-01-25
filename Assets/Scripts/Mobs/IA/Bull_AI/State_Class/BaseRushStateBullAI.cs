using UnityEngine;
using State.AICAC;

namespace State.AIBull
{
    public class BaseRushStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;
        RushBullParameterSO rushBullSO;
        [SerializeField] Material_Instances material_Instances;
        [SerializeField] bool lockPlayer;
        bool canStartRush;

        [SerializeField] float distDestination;
        [SerializeField] float distDetectObstacle;
        [SerializeField] float distDetectGround;

        [Header("Rush Movement")]
        public Vector3 captureBasePosDistance;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.Rush;
        }

        private void OnEnable()
        {
            try
            {
                globalRef.agent.enabled = false;

                if(rushBullSO == null)
                    rushBullSO = globalRef.rushBullSO;
            }
            catch
            {
                Debug.LogWarning("Missing Ref");
            }
        }

        private void Update()
        {
            if (!canStartRush)
            {
                if (material_Instances.Material.color != material_Instances.ColorPreAtatck)
                    ShowSoonAttack(true);
            }
            else
            {
                if(material_Instances.Material.color == material_Instances.ColorPreAtatck)
                    ShowSoonAttack(false);
            }

            SmoothLookAtPlayer();

            SetDestination();

            if(canStartRush)
            {
                RushMovement();
                RushDuration();
            }
        }
        private void FixedUpdate()
        {
            CheckObstacle();
        }

        void SetDestination()
        {
            if (globalRef.distPlayer > rushBullSO.stopLockDist && !lockPlayer)
            {
                rushBullSO.rushDestination = globalRef.playerTransform.position;
            }
            else
            {
                if (!lockPlayer)
                {
                    rushBullSO.rushDestination = globalRef.playerTransform.position + globalRef.transform.forward * rushBullSO.rushInertieSetDistance;
                    lockPlayer = true;
                }
            }
        }

        void RushDuration()
        {
            distDestination = Vector3.Distance(new Vector2(globalRef.transform.position.x, globalRef.transform.position.z),
                new Vector2(rushBullSO.rushDestination.x, rushBullSO.rushDestination.z));

            if (distDestination <= 1)
            {
                //Debug.Log("Distance Stop Rush");
                StopRush();
            }

            if (!rushBullSO.isFall && !rushBullSO.isGround)
            {
                rushBullSO.isFall = true;
            }
            else if (rushBullSO.isFall && rushBullSO.isGround)
            {
                //Debug.Log("Fall Stop Rush");
                StopRush();
            }

            rushBullSO.distRush = Vector3.Distance(captureBasePosDistance, globalRef.transform.position);
            if (rushBullSO.distRush >= rushBullSO.rushDistance)
            {
                StopRush();
            }
        }

        void RushMovement()
        {
            rushBullSO.targetPos = new Vector2(rushBullSO.rushDestination.x, rushBullSO.rushDestination.z);
            rushBullSO.direction = rushBullSO.targetPos - (new Vector2(globalRef.transform.position.x, globalRef.transform.position.z));
            rushBullSO.direction = rushBullSO.direction.normalized * rushBullSO.speedMove;

            SetGravity();

            rushBullSO.move = new Vector3(rushBullSO.direction.x, rushBullSO.directionYSlope.y + rushBullSO.AIVelocity.y, rushBullSO.direction.y);
            globalRef.characterController.Move(rushBullSO.move * Time.deltaTime);
                        
            globalRef.detectOtherAICollider.enabled = true;
            globalRef.hitBox.gameObject.SetActive(true);
        }
        void SetGravity()
        {
            if (!rushBullSO.isGround)
            {
                rushBullSO.fallingTime += Time.deltaTime;
                rushBullSO.effectiveGravity = rushBullSO.gravity * rushBullSO.fallingTime;
                rushBullSO.AIVelocity.y += rushBullSO.effectiveGravity;
            }
            else
            {
                rushBullSO.AIVelocity.y = 0;
            }
        }

        void CheckObstacle()
        {
            //Check obstacle Ground
            rushBullSO.hitGround = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.transform.position, -globalRef.transform.up, rushBullSO.maskCheckObstacle, Color.red, distDetectGround);
            rushBullSO.directionYSlope = rushBullSO.move;

            if (Vector3.Angle(transform.up, rushBullSO.hitGround.normal) < globalRef.characterController.slopeLimit)
                rushBullSO.directionYSlope = (rushBullSO.directionYSlope - 
                    (Vector3.Dot(rushBullSO.directionYSlope, rushBullSO.hitGround.normal)) * rushBullSO.hitGround.normal);

            if (rushBullSO.hitGround.transform != null)
            {
                rushBullSO.isGround = true;
            }
            else
            {
               // Debug.Log("Is Falling");
                rushBullSO.isGround = false;
            }

            //Check obstacle Wall
            rushBullSO.hitObstacle = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.transform.position, globalRef.transform.forward, rushBullSO.maskCheckObstacle, Color.red, distDetectObstacle);
            if (rushBullSO.hitObstacle.transform != null)
            {
              //  Debug.Log("Obstacle Stop Rush");
              if (rushBullSO.hitObstacle.transform.CompareTag("Ennemi"))
                StopRush();
            }
        }

        void StopRush()
        {
            stateController.SetActiveState(StateControllerBull.AIState.Idle);
        }

        void SmoothLookAtPlayer()
        {
            rushBullSO.directionLookAt = rushBullSO.rushDestination;

            rushBullSO.relativePos.x = rushBullSO.directionLookAt.x - globalRef.transform.position.x;
            rushBullSO.relativePos.y = 0;
            rushBullSO.relativePos.z = rushBullSO.directionLookAt.z - globalRef.transform.position.z;

            if (rushBullSO.speedRot < rushBullSO.maxSpeedRot)
                rushBullSO.speedRot += Time.deltaTime / rushBullSO.smoothRot;
            else
            {
                if(!canStartRush)
                {
                    ShowSoonAttack(false);
                    rushBullSO.speedRot = rushBullSO.maxSpeedRot;
                    canStartRush = true;
                }
            }

            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(rushBullSO.relativePos, Vector3.up), rushBullSO.speedRot);
            globalRef.transform.rotation = rotation;
        }

        void ShowSoonAttack(bool active)
        {
            if(active)
            {
                material_Instances.Material.color = material_Instances.ColorPreAtatck;
                material_Instances.ChangeColorTexture(material_Instances.ColorPreAtatck);
            }
            else
            {
                material_Instances.Material.color = material_Instances.Color;
                material_Instances.ChangeColorTexture(material_Instances.Color);
            }
        }

        private void OnDisable()
        {
            if(rushBullSO != null)
            {
                rushBullSO.isFall = false;
                rushBullSO.isGround = true;
                rushBullSO.speedRot = 0;
                rushBullSO.stopLockPlayer = false;
            }

            canStartRush = false;
            lockPlayer = false;
            globalRef.detectOtherAICollider.enabled = false;
            globalRef.hitBox.gameObject.SetActive(false);
            globalRef.agent.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ennemi") && gameObject.activeInHierarchy && other.gameObject.name.Contains("AICAC"))
            {
                Debug.Log("Get TrashMob");

                if (!rushBullSO.ennemiInCollider.Contains(other.gameObject) || rushBullSO.ennemiInCollider == null)
                    rushBullSO.ennemiInCollider.Add(other.gameObject);

                if (rushBullSO.ennemiInCollider != null)
                {
                    for (int i = 0; i < rushBullSO.ennemiInCollider.Count; i++)
                    {
                        if (rushBullSO.ennemiInCollider[i].GetComponent<GlobalRefAICAC>() != null)
                        {
                            GlobalRefAICAC globalRefAICAC = rushBullSO.ennemiInCollider[i].GetComponent<GlobalRefAICAC>();

                            rushBullSO.hitAICAC = RaycastAIManager.instanceRaycast.RaycastAI(transform.position, transform.forward, globalRef.ennemiMask, Color.red, 10f);
                            float angle;
                            angle = Vector3.SignedAngle(transform.forward, rushBullSO.hitAICAC.normal, Vector3.up);

                            if (globalRef.agent.velocity.magnitude > 0)
                            {
                                if (angle > 0 && globalRef.agent.velocity.magnitude > 0)
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
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ennemi"))
            {
                rushBullSO.ennemiInCollider.Remove(other.gameObject);
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