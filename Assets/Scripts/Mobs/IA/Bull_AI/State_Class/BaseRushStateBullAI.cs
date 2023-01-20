using UnityEngine;
using State.AICAC;
using System.Collections.Generic;

namespace State.AIBull
{
    public class BaseRushStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;
        RaycastHit hit;

        [SerializeField] bool lockPlayer;
        bool canStartRush;

        [SerializeField] float distDestination;
        [SerializeField] float distDetectObstacle;
        [SerializeField] float distDetectGround;

        [SerializeField] Vector3 directionYSlope;
        Vector3 move;

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
                ShowSoonAttack(true);
            }
            else
            {
                ShowSoonAttack(false);
            }

            SmoothLookAtPlayer();

            if (globalRef.distPlayer > globalRef.rushBullSO.stopLockDist && !lockPlayer)
            {
                globalRef.rushBullSO.rushDestination = globalRef.playerTransform.position;
            }
            else
            {
                if(!lockPlayer)
                {
                    globalRef.rushBullSO.rushDestination = globalRef.playerTransform.position + globalRef.transform.forward * globalRef.rushBullSO.rushInertieSetDistance;
                    lockPlayer = true;
                }
            }

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

        void RushDuration()
        {
            /* if (globalRef.hitBox.Blacklist.Count > 0) // player is hit
             {
                 StopRush();
             }*/

            distDestination = Vector3.Distance(new Vector2(globalRef.transform.position.x, globalRef.transform.position.z),
                new Vector2(globalRef.rushBullSO.rushDestination.x, globalRef.rushBullSO.rushDestination.z));

            if (distDestination <= 1)
            {
                Debug.Log("Distance Stop Rush");
                StopRush();
            }

            if (!globalRef.rushBullSO.isFall && !globalRef.rushBullSO.isGround)
            {
                globalRef.rushBullSO.isFall = true;
            }
            else if (globalRef.rushBullSO.isFall && globalRef.rushBullSO.isGround)
            {
                Debug.Log("Fall Stop Rush");
                StopRush();
            }
        }

        void RushMovement()
        {
            Vector2 targetPos = new Vector2(globalRef.rushBullSO.rushDestination.x, globalRef.rushBullSO.rushDestination.z);
            Vector2 direction = targetPos - (new Vector2(globalRef.transform.position.x, globalRef.transform.position.z));
            direction = direction.normalized * globalRef.rushBullSO.speedMove;

            SetGravity();

            move = new Vector3(direction.x, directionYSlope.y + globalRef.rushBullSO.playerVelocity.y, direction.y);
            globalRef.characterController.Move(move * Time.deltaTime);
                        
            globalRef.detectOtherAICollider.enabled = true;
            globalRef.hitBox.gameObject.SetActive(true);
        }
        void SetGravity()
        {
            if (!globalRef.rushBullSO.isGround)
            {
                globalRef.rushBullSO.fallingTime += Time.deltaTime;
                float effectiveGravity = globalRef.rushBullSO.gravity * globalRef.rushBullSO.fallingTime;
                globalRef.rushBullSO.playerVelocity.y += effectiveGravity;
            }
            else
            {
                globalRef.rushBullSO.playerVelocity.y = 0;
            }
        }

        void CheckObstacle()
        {
            //Check obstacle Ground
            hit = RaycastAIManager.RaycastAI(globalRef.transform.position, -globalRef.transform.up, globalRef.rushBullSO.maskCheckObstacle, Color.red, distDetectGround);
            directionYSlope = move;
            if (Vector3.Angle(transform.up, hit.normal) < globalRef.characterController.slopeLimit)
                directionYSlope = (directionYSlope - (Vector3.Dot(directionYSlope, hit.normal)) * hit.normal);
            if (hit.transform != null)
            {
                globalRef.rushBullSO.isGround = true;
            }
            else
            {
                Debug.Log("Is Falling");
                globalRef.rushBullSO.isGround = false;
            }

            //Check obstacle Wall
            RaycastHit hitObstacle = RaycastAIManager.RaycastAI(globalRef.transform.position, globalRef.transform.forward, globalRef.rushBullSO.maskCheckObstacle, Color.red, distDetectObstacle);
            if (hitObstacle.transform != null)
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

            direction = globalRef.rushBullSO.rushDestination;

            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            if (globalRef.rushBullSO.speedRot < globalRef.rushBullSO.maxSpeedRot)
                globalRef.rushBullSO.speedRot += Time.deltaTime / globalRef.rushBullSO.smoothRot;
            else
            {
                canStartRush = true;
                globalRef.rushBullSO.speedRot = globalRef.rushBullSO.maxSpeedRot;
            }

            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.rushBullSO.speedRot);
            globalRef.transform.rotation = rotation;
        }

        void ShowSoonAttack(bool active)
        {
            if(active)
            {
                globalRef.material_Instances.Material.color = globalRef.material_Instances.ColorPreAtatck;
                globalRef.material_Instances.ChangeColorTexture(globalRef.material_Instances.ColorPreAtatck);
            }
            else
            {
                globalRef.material_Instances.Material.color = globalRef.material_Instances.Color;
                globalRef.material_Instances.ChangeColorTexture(globalRef.material_Instances.Color);
            }
        }

        private void OnDisable()
        {
            globalRef.rushBullSO.isFall = false;
            globalRef.rushBullSO.isGround = true;
            canStartRush = false;
            lockPlayer = false;
            globalRef.detectOtherAICollider.enabled = false;
            globalRef.rushBullSO.stopLockPlayer = false;
            globalRef.hitBox.gameObject.SetActive(false);
            globalRef.agent.enabled = true;
            globalRef.rushBullSO.speedRot = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ennemi") && gameObject.activeInHierarchy)
            {
                Debug.Log("Get TrashMob");

                if (!globalRef.rushBullSO.ennemiInCollider.Contains(other.gameObject) || globalRef.rushBullSO.ennemiInCollider == null)
                    globalRef.rushBullSO.ennemiInCollider.Add(other.gameObject);

                if (globalRef.rushBullSO.ennemiInCollider != null)
                {
                    for (int i = 0; i < globalRef.rushBullSO.ennemiInCollider.Count; i++)
                    {
                        if (globalRef.rushBullSO.ennemiInCollider[i].GetComponent<GlobalRefAICAC>() != null)
                        {
                            GlobalRefAICAC globalRefAICAC = globalRef.rushBullSO.ennemiInCollider[i].GetComponent<GlobalRefAICAC>();

                            RaycastHit hit = RaycastAIManager.RaycastAI(transform.position, transform.forward, globalRef.ennemiMask, Color.red, 10f);
                            float angle;
                            angle = Vector3.SignedAngle(transform.forward, hit.normal, Vector3.up);

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