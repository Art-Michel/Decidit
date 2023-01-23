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

        private void Update()
        {
            Attack();
        }

        private void FixedUpdate()
        {
            RaycastHit hit = RaycastAIManager.RaycastAI(childflyAI.position, transform.forward,
                                                        globalRef.baseAttackFlySO.wallMask, Color.blue, globalRef.baseAttackFlySO.distDetectWall);

            if(hit.transform != null)
            {
                stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
            }
        }

        private void OnEnable()
        {
            if(baseAttackFlySO != null)
            {
                baseAttackFlySO.speedRotationAIAttack = 0;
                baseAttackFlySO.currentSpeedYAttack = 0;
                baseAttackFlySO.lerpSpeedYValueAttack = 0;
            }
        }

        public void Attack()
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
            globalRef.agent.speed = baseAttackFlySO.baseAttackSpeed;

            globalRef.agent.SetDestination(new Vector3(globalRef.transform.position.x, 0, globalRef.transform.position.z) + childflyAI.TransformDirection(Vector3.forward));
            baseAttackFlySO.currentSpeedYAttack = Mathf.Lerp(baseAttackFlySO.currentSpeedYAttack, baseAttackFlySO.maxSpeedYTranslationAttack, baseAttackFlySO.lerpSpeedYValueAttack);

            baseAttackFlySO.lerpSpeedYValueAttack += (Time.deltaTime / baseAttackFlySO.ySpeedSmootherAttack);

            if (Mathf.Abs(globalRef.transform.position.y - lockPlayerFlySO.destinationFinal.y) > 1)
            {
                if (globalRef.transform.position.y < lockPlayerFlySO.destinationFinal.y)
                {
                    globalRef.agent.baseOffset += baseAttackFlySO.currentSpeedYAttack * Time.deltaTime;
                }
                else
                {
                    globalRef.agent.baseOffset -= baseAttackFlySO.currentSpeedYAttack * Time.deltaTime;
                }
            }
        }

        private void OnDisable()
        {
            stopLock = false;
            baseAttackFlySO.speedRotationAIAttack = 0;
            baseAttackFlySO.currentSpeedYAttack = 0;
            baseAttackFlySO.lerpSpeedYValueAttack = 0;
        }
    }
}