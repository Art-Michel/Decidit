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

        [SerializeField] float minBaseOffset;
 
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
            globalRef.colliderBaseAttack.SetActive(true);

            try
            {
                SoundManager.Instance.PlaySound("event:/SFX_IA/Vorice_SFX(Vol)/Attack", 1f, gameObject);
            }
            catch
            {
                //
            }

            if (baseAttackFlySO != null)
            {
                baseAttackFlySO.speedRotationAIAttack = 0;
            }

            if (globalRef != null && globalRef.myAnimator != null)
                AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Rush");

            //PLAY SOUND PRE ATTACK FLY
            // TODO lucas va te faire encul�
        }


        private void Update()
        {
            AdjustingYspeed();
            Attack();
            StopAttack();
        }

        private void FixedUpdate()
        {
            RaycastHit hit = RaycastAIManager.instanceRaycast.RaycastAI(childflyAI.position, transform.forward,
                                                        globalRef.baseAttackFlySO.wallMask, Color.blue, globalRef.baseAttackFlySO.distDetectWall);

            if (hit.transform != null)
            {
                if(hit.transform.gameObject.layer == 9)
                {
                    stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
                }
            }

            SlowSpeed(globalRef.isInEylau || globalRef.IsZap);
        }

        bool CheckPlayerIsBack()
        {
            Vector3 playerForward = globalRef.playerTransform.GetChild(0).forward;
            Vector3 thisForward = childflyAI.forward;

            float dot = Vector3.Dot(thisForward, (globalRef.playerTransform.position - childflyAI.position).normalized);
            if (dot < 0.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
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
            else if (!stopLock)
            {
                stopLock = true;
            }
        }
        void ApplyFlyingMove()
        {
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

            if (globalRef.agent.baseOffset < minBaseOffset)
                globalRef.agent.baseOffset = minBaseOffset;
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

        void StopAttack()
        {
            if (baseAttackFlySO.distDestinationFinal <= 1.5f)
                stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
            else
            {
                if (globalRef.hitbox.Blacklist.Count > 0)
                {
                    globalRef.colliderBaseAttack.gameObject.SetActive(false);
                    stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
                }
            }

            if(CheckPlayerIsBack())
            {
                globalRef.colliderBaseAttack.gameObject.SetActive(false);
                stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
            }
        }

        private void OnDisable()
        {
            stopLock = false;

            if (baseAttackFlySO != null)
            {
                baseAttackFlySO.speedRotationAIAttack = 0;
                baseAttackFlySO.currentSpeedYAttack = 0;
                globalRef.colliderBaseAttack.SetActive(false);
            }
        }
    }
}