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
            if (state == StateControllerFlyAI.AIState.BaseAttack)
            {
                Attack();
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

            if (baseAttackFlySO.distDestinationFinal > lockPlayerFlySO.distStopLockPlayer)
            {
                lockPlayerStateFlyAI.LockPlayer();
                lockPlayerStateFlyAI.SmoothLookAtYAxisAttack();
            }

            if (baseAttackFlySO.distDestinationFinal <= 1.5f)
            {
                stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);
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
            baseAttackFlySO.speedRotationAIAttack = 0;
            baseAttackFlySO.currentSpeedYAttack = 0;
            baseAttackFlySO.lerpSpeedYValueAttack = 0;
        }
    }
}