using UnityEngine;

namespace State.FlyAI
{
    public class BaseAttackStateFlyAI : _StateFlyAI
    {
        [SerializeField] GlobalRefFlyAI globalRef;
        BaseAttackFlySO baseAttackFlySO;
        LockPlayerFlySO lockPlayerFlySO;
        [SerializeField] LockPlayerStateFlyAI lockPlayerStateFlyAI;

        [SerializeField] Transform childflyAI;

        [SerializeField] bool stopLock;

        public override void InitState(StateControllerFlyAI stateController)
        {
            base.InitState(stateController);

            state = StateControllerFlyAI.AIState.BaseAttack;
        }


        private void Start()
        {
            lockPlayerFlySO = globalRef.lockPlayerFlySO;
            baseAttackFlySO = globalRef.baseAttackFlySO;
        }

        private void OnEnable()
        {
            if (baseAttackFlySO != null)
            {
                baseAttackFlySO.speedRotationAIAttack = 0;
            }

            /*if (SoundManager.instance != null)
                SoundManager.instance.PlaySoundMobOneShot(globalRef.audioSourceFly, SoundManager.instance.soundAndVolumeFlyMob[1]);*/

            //PLAY SOUND PRE ATTACK FLY
        }


        private void Update()
        {
            AdjustingYspeed();
            Attack();
        }

        private void FixedUpdate()
        {
            RaycastHit hit = RaycastAIManager.instanceRaycast.RaycastAI(childflyAI.position, transform.forward,
                                                        globalRef.baseAttackFlySO.wallMask, Color.blue, globalRef.baseAttackFlySO.distDetectWall);

            if(hit.transform != null)
            {
                stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
            }

            SlowSpeed(globalRef.isInEylau);
        }

        void AdjustingYspeed()
        {
            float distHorizontal = Vector2.Distance(new Vector2(childflyAI.position.x, childflyAI.position.z),
                    new Vector2(globalRef.playerTransform.position.x, globalRef.playerTransform.position.z));
            float t = distHorizontal / baseAttackFlySO.baseAttackSpeed;

            float distVertical = Mathf.Abs(childflyAI.position.y - (globalRef.playerTransform.position.y + 0.5f));

            float v = distVertical / t;
            baseAttackFlySO.currentSpeedYAttack = v;
        }

        void Attack()
        {
            baseAttackFlySO.distDestinationFinal = Vector3.Distance(lockPlayerFlySO.destinationFinal, globalRef.transform.position);
            globalRef.colliderBaseAttack.gameObject.SetActive(true);

            ApplyFlyingMove();

            if (baseAttackFlySO.distDestinationFinal > lockPlayerFlySO.distStopLockPlayer && !stopLock)
            {
                lockPlayerStateFlyAI.LockPlayer();
                lockPlayerStateFlyAI.SmoothLookAtYAxisAttack();
            }
            else if(!stopLock)
            {
                stopLock = true;
            }

            if (baseAttackFlySO.distDestinationFinal <= 1.5f)  // (globalRef.agent.remainingDistance <= 1f)// (baseAttackFlySO.distDestinationFinal <= 1.5f)
            {
                stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
            }
            else
            {
                if(globalRef.hitbox.Blacklist.Count >0)
                {
                    globalRef.colliderBaseAttack.gameObject.SetActive(false);
                    stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
                }
            }
        }

        void ApplyFlyingMove()
        {
            //globalRef.agent.speed = baseAttackFlySO.baseAttackSpeed;

            //SlowSpeed(globalRef.isInEylau);
            if (globalRef.transform.position.y < lockPlayerFlySO.destinationFinal.y)
            {
                if (globalRef.isInEylau)
                    globalRef.agent.baseOffset += (baseAttackFlySO.currentSpeedYAttack * Time.deltaTime) / 2;
                else
                    globalRef.agent.baseOffset += baseAttackFlySO.currentSpeedYAttack * Time.deltaTime;

            }
            else
            {
                if (globalRef.isInEylau)
                    globalRef.agent.baseOffset -= (baseAttackFlySO.currentSpeedYAttack * Time.deltaTime) / 2;
                else
                    globalRef.agent.baseOffset -= baseAttackFlySO.currentSpeedYAttack * Time.deltaTime;
            }
        }
        void SlowSpeed(bool active)
        {
            if (active)
            {
                globalRef.agent.velocity = (childflyAI.transform.forward * baseAttackFlySO.baseAttackSpeed) / globalRef.slowRatio;
            }
            else
            {
                globalRef.agent.velocity = childflyAI.transform.forward * baseAttackFlySO.baseAttackSpeed;
            }
        }

        private void OnDisable()
        {
            stopLock = false;

            if(baseAttackFlySO != null)
            {
                baseAttackFlySO.speedRotationAIAttack = 0;
                baseAttackFlySO.currentSpeedYAttack = 0;
            }
        }
    }
}