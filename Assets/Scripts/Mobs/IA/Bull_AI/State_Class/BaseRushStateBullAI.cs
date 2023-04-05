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
        [SerializeField] bool canStartRush;

        [SerializeField] float distDestination;
        [SerializeField] float distDetectObstacle;
        [SerializeField] float distDetectGround;
        [SerializeField] float distFallStopRush;

        [Header("Rush Movement")]
        public Vector3 captureBasePosDistance;
        public float gravityMultiplier;
        [SerializeField] float distParcourue;

        [Header("Position 2D")]
        Vector2 posPlayer;
        Vector2 posAI;

        int indexRay;
        [SerializeField] float delayInertieRushInWall;
        [SerializeField] float maxDelayInertieRushInWall;

        [SerializeField] bool stopLockPlayerRush;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.Rush;
        }

        private void OnEnable()
        {
            captureBasePosDistance = globalRef.transform.position;
            stopLockPlayerRush = false;
            try
            {
                globalRef.agent.enabled = false;
                AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "PreAttack");
                //SoundManager.instance.PlaySoundMobOneShot(globalRef.audioSourceBull, SoundManager.instance.soundAndVolumeRushMob[0]);
                // TO DO lucas va te faire encul�
                //Play SOUND PRE ATTACK RUSHER
            }
            catch
            {
                //Debug.LogWarning("Missing Ref");
            }
        }

        private void Start()
        {
            if (rushBullSO == null)
                rushBullSO = globalRef.rushBullSO;
        }

        private void Update()
        {
            if (Time.timeScale > 0)
            {
                SetDestination();

                if(!canStartRush)
                    SmoothLookAtPlayer();

                if (!canStartRush)
                {
                    if (material_Instances.Material[0].mainTexture != material_Instances.TextureBase)
                        ShowSoonAttack(true);
                }
                else
                {
                    if (material_Instances.Material[0].mainTexture == material_Instances.TextureBase)
                    {
                        ShowSoonAttack(false);
                    }
                }

                if (canStartRush)
                {
                    RushMovement();
                    RushDuration();

                    if(!stopLockPlayerRush)
                        stopLockPlayerRush = CheckPlayerIsBack();
                    else
                    {
                        if(CheckDistDone() > rushBullSO.rushInertieSetDistance)
                            stateController.SetActiveState(StateControllerBull.AIState.Idle);
                    }
                    if (!stopLockPlayerRush)
                        SmoothLookAtPlayerRush();
                }
            }
        }
        private void FixedUpdate()
        {
            CheckObstacle();
        }

        float CheckDistDone()
        {
            return distParcourue = Vector3.Magnitude(captureBasePosDistance - globalRef.transform.position);
        }

        void SetDestination()
        {
            if (!canStartRush && !lockPlayer)
            {
                rushBullSO.rushDestination = globalRef.playerTransform.position;
            }
            else
            {
                if (!lockPlayer)
                {
                    rushBullSO.rushDestination = globalRef.playerTransform.position + globalRef.transform.forward * rushBullSO.rushInertieSetDistance;
                    lockPlayer = true;
                    //Invoke("CheckSpeed", 1f);
                }
            }
        }

        void RushMovement()
        {
            // TODO lucas va te faire encul�
            //SoundManager.Instance.PlaySound("event:/SFX_IA/ShredNoss_SFX(Dash)/Attack", 1f, gameObject);
            //Play SOUND ATTACK RUSHER

            rushBullSO.targetPos = new Vector2(rushBullSO.rushDestination.x, rushBullSO.rushDestination.z);
            posAI = new Vector2(globalRef.transform.position.x, globalRef.transform.position.z);

            /*rushBullSO.direction = rushBullSO.targetPos - posAI;
            rushBullSO.direction = rushBullSO.direction.normalized * rushBullSO.speedMove;*/
            Vector3 dir = globalRef.transform.forward * rushBullSO.speedMove;

            SetGravity();
            SlowSpeed(globalRef.isInEylau);
            //rushBullSO.move = new Vector3(rushBullSO.direction.x, rushBullSO.directionYSlope.y + rushBullSO.AIVelocity.y, rushBullSO.direction.y);
            rushBullSO.move = new Vector3(dir.x, rushBullSO.directionYSlope.y + rushBullSO.AIVelocity.y, dir.z);
            globalRef.characterController.Move(rushBullSO.move * Time.deltaTime);

            globalRef.detectOtherAICollider.enabled = true;
            globalRef.hitBoxRush.gameObject.SetActive(true);
        }

        void SetGravity()
        {
            if (!rushBullSO.isGround)
            {
                globalRef.rushBullSO.AIVelocity.y = globalRef.rushBullSO.AIVelocity.y - 9.8f * gravityMultiplier * Time.deltaTime;
            }
            else
            {
                globalRef.rushBullSO.AIVelocity.y = 0;
            }
        }

        void SlowSpeed(bool active)
        {
            if (active)
            {
                globalRef.slowSpeed = rushBullSO.speedMove / globalRef.slowRatio;
                rushBullSO.direction = rushBullSO.direction.normalized * globalRef.slowSpeed;
            }
            else
            {
                rushBullSO.direction = rushBullSO.direction.normalized * rushBullSO.speedMove;
            }
        }

        void RushDuration()
        {
            posPlayer = new Vector2(globalRef.transform.position.x, globalRef.transform.position.z);
            distDestination = Vector3.Distance(posPlayer, rushBullSO.targetPos);

            if (distDestination <= 1)
            {
                StopRush();
            }

            rushBullSO.distRush = Vector3.Distance(captureBasePosDistance, globalRef.transform.position);
            if (rushBullSO.distRush >= rushBullSO.rushDistance)
            {
                StopRush();
            }
        }
        /*void CheckSpeed()
        {
            if(globalRef.characterController.velocity.magnitude ==0)
                StopRush();
        }*/


        void CheckObstacle()
        {
            //Check obstacle Ground
            rushBullSO.hitGround = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.transform.position, -globalRef.transform.up, rushBullSO.maskCheckObstacle, Color.red, 100f);
            rushBullSO.directionYSlope = rushBullSO.move;

            if (Vector3.Angle(transform.up, rushBullSO.hitGround.normal) < globalRef.characterController.slopeLimit)
                rushBullSO.directionYSlope = (rushBullSO.directionYSlope - 
                    (Vector3.Dot(rushBullSO.directionYSlope, rushBullSO.hitGround.normal)) * rushBullSO.hitGround.normal);

            if (rushBullSO.hitGround.transform != null)
            {
                if (rushBullSO.isGround && rushBullSO.hitGround.distance > distFallStopRush)
                {
                    rushBullSO.isFall = true;
                }
                if(rushBullSO.hitGround.distance <= distDetectGround)
                {
                    rushBullSO.isGround = true;
                }
                else
                {
                    rushBullSO.isGround = false;
                }
            }
            else
            {
                rushBullSO.isGround = false;
            }


            //Check obstacle Wall
            if(canStartRush)
            {
                switch (indexRay)
                {
                    case 0:
                        CheckWall(globalRef.RayRushRight);
                        break;
                    case 1:
                        CheckWall(globalRef.RayRushMiddle);
                        break;
                    case 2:
                        CheckWall(globalRef.RayRushLeft);
                        break;
                }
            }
        }
        void CheckWall(Transform rayRush)
        {
            rushBullSO.hitObstacle = RaycastAIManager.instanceRaycast.RaycastAI(rayRush.position, rayRush.forward, 
                                                      rushBullSO.maskCheckObstacle, Color.red, distDetectObstacle);
            if (rushBullSO.hitObstacle.transform != null)
            {
                if (delayInertieRushInWall > 0)
                    delayInertieRushInWall -= Time.fixedDeltaTime;
                else
                    StopRush();
            }

            if (indexRay < 2)
                indexRay++;
            else
                indexRay = 0;
        }

        bool CheckPlayerIsBack()
        {
            Vector3 playerForward = globalRef.playerTransform.GetChild(0).forward;
            Vector3 thisForward = globalRef.transform.forward;

            float dot = Vector3.Dot(thisForward, (globalRef.playerTransform.position - globalRef.transform.position).normalized);
            if (dot < 0.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void SmoothLookAtPlayer()
        {
            rushBullSO.directionLookAt = rushBullSO.rushDestination;

            rushBullSO.relativePos.x = rushBullSO.directionLookAt.x - globalRef.transform.position.x;
            rushBullSO.relativePos.y = 0;
            rushBullSO.relativePos.z = rushBullSO.directionLookAt.z - globalRef.transform.position.z;

            SlowRotationLock(globalRef.isInEylau);
            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(rushBullSO.relativePos, Vector3.up), rushBullSO.speedRotLock);
            globalRef.transform.rotation = rotation;
        }
        void SmoothLookAtPlayerRush()
        {
            rushBullSO.directionLookAt = globalRef.playerTransform.position;

            rushBullSO.relativePos.x = rushBullSO.directionLookAt.x - globalRef.transform.position.x;
            rushBullSO.relativePos.y = 0;
            rushBullSO.relativePos.z = rushBullSO.directionLookAt.z - globalRef.transform.position.z;

            SlowRotationRush();
            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(rushBullSO.relativePos, Vector3.up), rushBullSO.speedRotRush);
            globalRef.transform.rotation = rotation;
        }
        void SlowRotationLock(bool active)
        {
            switch (active)
            {
                case true:
                    if (rushBullSO.speedRotLock < rushBullSO.maxSpeedRotLock)
                    {
                        rushBullSO.speedRotLock += Time.deltaTime / globalRef.slowSpeedRot;
                    }
                    else
                    {
                        if (!canStartRush)
                        {
                            ShowSoonAttack(false);
                            rushBullSO.speedRotLock = rushBullSO.maxSpeedRotLock;
                            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Rush");
                            canStartRush = true;
                        }
                    }
                    break;


                case false:
                    if (rushBullSO.speedRotLock < rushBullSO.maxSpeedRotLock)
                    {
                        rushBullSO.speedRotLock += Time.deltaTime / rushBullSO.maxSpeedRotLock;
                    }
                    else
                    {
                        if (!canStartRush)
                        {
                            SoundManager.Instance.PlaySound("event:/SFX_IA/ShredNoss_SFX(Dash)/Attack", 10f, gameObject);
                            ShowSoonAttack(false);
                            rushBullSO.speedRotLock = rushBullSO.maxSpeedRotLock;
                            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Rush");
                            canStartRush = true;
                        }
                    }
                    break;
            }
        }
        void SlowRotationRush()
        {
            if (rushBullSO.speedRotRush < rushBullSO.maxSpeedRotRush)
            {
                rushBullSO.speedRotRush += Time.deltaTime / rushBullSO.smoothRotRush;
            }
        }


        void ShowSoonAttack(bool active)
        {
            if(active)
            {
                for (int i = 0; i < material_Instances.Material.Length; i++)
                {
                    material_Instances.Material[0].color = material_Instances.ColorPreAtatck;
                }
                material_Instances.ChangeColorTexture(material_Instances.ColorPreAtatck);
            }
            else
            {
                for (int i = 0; i < material_Instances.Material.Length; i++)
                {
                    material_Instances.Material[0].color = material_Instances.ColorBase;
                }
                material_Instances.ChangeColorTexture(material_Instances.ColorBase);
            }
        }

        void StopRush()
        {
            if(delayInertieRushInWall <=0)
                stateController.SetActiveState(StateControllerBull.AIState.Idle);
        }
        private void OnDisable()
        {
            delayInertieRushInWall = maxDelayInertieRushInWall;
            if (rushBullSO != null)
            {
                rushBullSO.isFall = false;
                rushBullSO.isGround = true;
                rushBullSO.speedRotLock = 0;
                rushBullSO.speedRotRush = 0;
                rushBullSO.stopLockPlayer = false;
                rushBullSO.ennemiInCollider.Clear();
                rushBullSO.AIVelocity = Vector3.zero;
            }

            globalRef.launchRush = false;
            canStartRush = false;
            lockPlayer = false;
            globalRef.detectOtherAICollider.enabled = false;
            globalRef.hitBoxRush.gameObject.SetActive(false);
            globalRef.agent.enabled = true;
            stopLockPlayerRush = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (gameObject.activeInHierarchy && other.name.Contains("AICAC"))
            {
                if (!rushBullSO.ennemiInCollider.Contains(other.gameObject) || rushBullSO.ennemiInCollider == null)
                    rushBullSO.ennemiInCollider.Add(other.gameObject);

                if (rushBullSO.ennemiInCollider != null)
                {
                    for (int i = 0; i < rushBullSO.ennemiInCollider.Count; i++)
                    {
                        GlobalRefAICAC globalRefAICAC = rushBullSO.ennemiInCollider[i].GetComponent<GlobalRefAICAC>();

                        rushBullSO.hitAICAC = RaycastAIManager.instanceRaycast.RaycastAI(transform.position, transform.forward, globalRef.ennemiMask, Color.red, 10f);
                        float angle;
                        angle = Vector3.SignedAngle(transform.forward, rushBullSO.hitAICAC.normal, Vector3.up);

                        if (globalRef.characterController.velocity.magnitude > 0)
                        {
                            if (angle > 0)
                            {
                                globalRefAICAC.dodgeAICACSO.targetObjectToDodge = this.transform;
                                globalRefAICAC.dodgeAICACSO.leftDodge = true;
                                globalRefAICAC.ActiveStateDodge();
                            }
                            else
                            {
                                globalRefAICAC.dodgeAICACSO.targetObjectToDodge = this.transform;
                                globalRefAICAC.dodgeAICACSO.rightDodge = true;
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