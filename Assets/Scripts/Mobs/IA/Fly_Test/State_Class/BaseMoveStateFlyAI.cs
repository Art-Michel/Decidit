using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace State.FlyAI
{
    public class BaseMoveStateFlyAI : _StateFlyAI
    {
        [SerializeField] GlobalRefFlyAI globalRef;
        BaseMoveFlySO baseMoveFlySO;

        RaycastHit hitObstacle;

        [SerializeField] GameObject flyAI;
        [SerializeField] Transform childflyAI;

        [SerializeField] bool dodgeObstacle;
        [SerializeField] bool right;
        [SerializeField] float offset;
        Quaternion rotation;

        [SerializeField] LayerMask maskSearchPos;

        [Header("Rotation")]
        float currentMaxSpeedRotationAIDodgeObstacle;
        float currentSmoothRotationDodgeObstacle;

        [SerializeField] float speedVelocity;
        [SerializeField] Vector3 velocity;

        [SerializeField] bool isInCover;

        public override void InitState(StateControllerFlyAI stateController)
        {
            base.InitState(stateController);

            state = StateControllerFlyAI.AIState.BaseMove;
        }

        private void OnEnable()
        {
            if (globalRef != null && globalRef.myAnimator != null)
                AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "FlyIdle");
        }

        private void Start()
        {
            baseMoveFlySO = globalRef.baseMoveFlySO;
            baseMoveFlySO.currentRateAttack = (int)Random.Range(baseMoveFlySO.maxRateAttack.x, baseMoveFlySO.maxRateAttack.y);
        }

        private void Update()
        {
            AdjustingYspeed();
            SmoothLookAtYAxisPatrol();
            AdjustSpeed();
        }

        private void FixedUpdate()
        {
            CheckObstacle();
            //SlowSpeed(globalRef.isInEylau);
        }

        void AdjustSpeed()
        {
            if(!dodgeObstacle)
            {
                if(baseMoveFlySO.currentSpeed < baseMoveFlySO.baseMoveSpeed)
                {
                    baseMoveFlySO.currentSpeed += Time.deltaTime * 1f;
                }
                else
                {
                    baseMoveFlySO.currentSpeed = baseMoveFlySO.baseMoveSpeed;
                }
            }
            else
            {
                if (baseMoveFlySO.currentSpeed != baseMoveFlySO.lowSpeed)
                {
                    baseMoveFlySO.currentSpeed = baseMoveFlySO.lowSpeed;
                }
            }
        }

        void AdjustingYspeed()
        {
            float distHorizontal = Vector2.Distance(new Vector2(childflyAI.position.x, childflyAI.position.z),
                    new Vector2(baseMoveFlySO.destinationFinal.x, baseMoveFlySO.destinationFinal.z));
            float t = distHorizontal / baseMoveFlySO.currentSpeed;

            float distVertical = Mathf.Abs(childflyAI.position.y - baseMoveFlySO.destinationFinal.y);

            float v = distVertical / t;
            baseMoveFlySO.currentSpeedYPatrol = v;
        }

        void CheckObstacle()
        {
            hitObstacle = RaycastAIManager.instanceRaycast.RaycastAI(transform.position, baseMoveFlySO.destinationFinal - 
                flyAI.transform.position, baseMoveFlySO.maskObstacle, 
                Color.red,Vector3.Distance(flyAI.transform.position, baseMoveFlySO.destinationFinal));

            if(hitObstacle.transform != null)
            {
                dodgeObstacle = true;
                SearchNewPos();
            }
            else
            {
                if (dodgeObstacle)
                {
                    dodgeObstacle = false;
                }
            }

            RaycastHit hit = RaycastAIManager.instanceRaycast.RaycastAI(childflyAI.transform.position, childflyAI.transform.forward, 
                baseMoveFlySO.maskObstacle, Color.red, baseMoveFlySO.lenghtRayDetectObstacle);

            if (hit.transform != null)
            {
                currentMaxSpeedRotationAIDodgeObstacle = baseMoveFlySO.maxSpeedRotationAIDodgeObstacle;
                currentSmoothRotationDodgeObstacle = baseMoveFlySO.smoothRotationDodgeObstacle;
            }
            else
            {
                currentMaxSpeedRotationAIDodgeObstacle = baseMoveFlySO.maxSpeedRotationAIPatrol;
                currentSmoothRotationDodgeObstacle = baseMoveFlySO.smoothRotationPatrol;
            }

            if (baseMoveFlySO.currentRateAttack <= 0)
            {
                RaycastHit hitCheckPlayer = RaycastAIManager.instanceRaycast.RaycastAI(childflyAI.position,
                   new Vector3(globalRef.playerTransform.GetChild(0).position.x, globalRef.playerTransform.GetChild(0).position.y, 
                   globalRef.playerTransform.GetChild(0).position.z) - childflyAI.position, baseMoveFlySO.maskCheckCanRush, Color.red, 100f);

                if (hitCheckPlayer.transform == globalRef.playerTransform && !CheckPlayerCover.instance.isCover)
                {
                    stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.LockPlayer);
                }
            }

            // check if is in collider cover
            Collider [] col = Physics.OverlapSphere(transform.position, 0.7f, 19);
            if(col.Length >0)
            {
                isInCover = true;
            }
            else
            {
                isInCover = false;
            }
        }

        public void SmoothLookAtYAxisPatrol()
        {
            Vector3 relativePos;

            if(isInCover)
                baseMoveFlySO.destinationFinal.y = childflyAI.transform.position.y;

            relativePos.x = baseMoveFlySO.destinationFinal.x - flyAI.transform.position.x;
            relativePos.y = baseMoveFlySO.destinationFinal.y - flyAI.transform.position.y;
            relativePos.z = baseMoveFlySO.destinationFinal.z - flyAI.transform.position.z;

            SlowRotation(globalRef.isInEylau, relativePos);
            childflyAI.localRotation = rotation;

            if (baseMoveFlySO.speedRotationAIPatrol < currentMaxSpeedRotationAIDodgeObstacle)
            {
                baseMoveFlySO.speedRotationAIPatrol += (Time.deltaTime / (currentSmoothRotationDodgeObstacle * globalRef.slowRatio));
            }

            ApplyFlyingMove();
            DelayBeforeAttack();
        }
        void SlowRotation(bool active, Vector3 relativePos)
        {
            if (active)
            {
                rotation = Quaternion.Slerp(childflyAI.localRotation, Quaternion.LookRotation(relativePos, Vector3.up), baseMoveFlySO.speedRotationAIPatrol);
            }
            else
            {
                rotation = Quaternion.Slerp(childflyAI.localRotation, Quaternion.LookRotation(relativePos, Vector3.up), baseMoveFlySO.speedRotationAIPatrol);
            }
        }

        ////////////// Set Destination \\\\\\\\\\\\\\\\\\\\\
        Vector3 SearchNewPos() // d�fini la position al�atoire choisi dans la fonction "RandomPointInBounds()" si la distance entre le point et l'IA est suffisament grande
        {
            if (baseMoveFlySO.distDestinationFinal < 10 || dodgeObstacle)
            {
                baseMoveFlySO.destinationFinal = RandomPointInBounds(globalRef.myCollider.bounds);

                baseMoveFlySO.newPosIsSet = false;
                baseMoveFlySO.speedRotationAIPatrol = 0;
                baseMoveFlySO.currentSpeedYPatrol = 0;
                return baseMoveFlySO.newPos = new Vector3(baseMoveFlySO.destinationFinal.x, baseMoveFlySO.destinationFinal.z);
            }
            else
            {
                Collider[] hitColliders = Physics.OverlapSphere(baseMoveFlySO.destinationFinal, 1f, maskSearchPos);

                if(hitColliders.Length == 0)
                {
                    baseMoveFlySO.newPosIsSet = true;
                    baseMoveFlySO.speedRotationAIPatrol = 0;
                    baseMoveFlySO.currentSpeedYPatrol = 0;

                    return baseMoveFlySO.newPos = new Vector3(baseMoveFlySO.destinationFinal.x, baseMoveFlySO.destinationFinal.z);
                }
                else
                {
                    baseMoveFlySO.destinationFinal = RandomPointInBounds(globalRef.myCollider.bounds);

                    baseMoveFlySO.newPosIsSet = false;
                    baseMoveFlySO.speedRotationAIPatrol = 0;
                    baseMoveFlySO.currentSpeedYPatrol = 0;
                    return baseMoveFlySO.newPos = new Vector3(baseMoveFlySO.destinationFinal.x, baseMoveFlySO.destinationFinal.z);
                }
            }
        }
        Vector3 RandomPointInBounds(Bounds bounds) // renvoie une position al�atoire dans un trigger collider
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        void ApplyFlyingMove()
        {
            baseMoveFlySO.distDestinationFinal = Vector3.Distance(flyAI.transform.position, baseMoveFlySO.destinationFinal);
            //globalRef.agent.speed = baseMoveFlySO.currentSpeed;

            if (baseMoveFlySO.newPosIsSet)
            {
                if (baseMoveFlySO.distDestinationFinal > 7)
                {
                    SlowSpeed(globalRef.isInEylau);

                    if (flyAI.transform.position.y < baseMoveFlySO.destinationFinal.y)
                    {
                        if (globalRef.isInEylau)
                            globalRef.agent.baseOffset += (baseMoveFlySO.currentSpeedYPatrol * Time.deltaTime) / globalRef.slowRatio;
                        else
                            globalRef.agent.baseOffset += (baseMoveFlySO.currentSpeedYPatrol * Time.deltaTime);
                    }
                    else
                    {
                        if (globalRef.isInEylau)
                            globalRef.agent.baseOffset -= (baseMoveFlySO.currentSpeedYPatrol * Time.deltaTime) / globalRef.slowRatio;
                        else
                            globalRef.agent.baseOffset -= (baseMoveFlySO.currentSpeedYPatrol * Time.deltaTime);
                    }
                }
                else
                    baseMoveFlySO.newPosIsSet = false;
            }
            else
                SearchNewPos();
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
                if (baseMoveFlySO.newPosIsSet)
                    globalRef.agent.velocity = (childflyAI.transform.forward * (baseMoveFlySO.currentSpeed / globalRef.slowRatio));

                velocity = globalRef.agent.velocity;
                speedVelocity = globalRef.agent.velocity.magnitude;
            }
            else
            {
                if (baseMoveFlySO.newPosIsSet)
                    globalRef.agent.velocity = childflyAI.transform.forward * baseMoveFlySO.currentSpeed;

                velocity = globalRef.agent.velocity;
                speedVelocity = globalRef.agent.velocity.magnitude;
            }
        }

        void DelayBeforeAttack()
        {
            if (baseMoveFlySO.currentRateAttack > 0)
            {
                baseMoveFlySO.currentRateAttack -= Time.deltaTime;
            }
        }

        public void ForceLaunchAttack()
        {
            baseMoveFlySO.currentRateAttack = 0;
        }

        void OnDisable()
        {
            if(baseMoveFlySO != null)
            {
                baseMoveFlySO.currentRateAttack = Random.Range(baseMoveFlySO.maxRateAttack.x, baseMoveFlySO.maxRateAttack.y);
                baseMoveFlySO.currentSpeedYPatrol = 0;
                baseMoveFlySO.speedRotationAIPatrol = 0;
                baseMoveFlySO.newPosIsSet = false;
            }
        }
    }
}