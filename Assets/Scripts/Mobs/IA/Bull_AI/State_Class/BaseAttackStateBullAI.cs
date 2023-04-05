using UnityEngine;

namespace State.AIBull
{
    public class BaseAttackStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;

        [SerializeField] float attackDuration;

        Vector3 posPlayer;

        [SerializeField] bool canAttak;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.BaseAttack;
        }

        private void OnEnable()
        {
            if (globalRef != null)
            {
                canAttak = false;

                if (globalRef.myAnimator != null)
                    AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Idle");

                posPlayer = globalRef.playerTransform.position;
                globalRef.agent.SetDestination(posPlayer);
            }
        }

        void Update()
        {
            if (canAttak)
            {
                if (Vector3.Distance(globalRef.transform.position, posPlayer) < globalRef.baseAttackBullSO.distLaunchAttack)
                {
                    AttackDuration();
                    DelayBeforeAttack();
                }
                else
                {
                    globalRef.agent.speed = globalRef.baseAttackBullSO.speed;
                }
            }
            else
                SmoothLookAtPlayer();

            if(!canAttak)
                CheckObstaclePlayer();
        }

        bool CheckObstaclePlayer()
        {
            Vector3 playerForward = globalRef.playerTransform.GetChild(0).forward;
            Vector3 thisForward = globalRef.transform.forward;

            float dot = Vector3.Dot(thisForward, (globalRef.playerTransform.position - globalRef.transform.position).normalized);
            if (dot > 0.9f)
            {
                canAttak = true;
                return true;
            }
            else
            {
                canAttak = false;
                return false;
            }
        }

        void AttackDuration()
        {
            if(attackDuration >0)
            {
                attackDuration -= Time.deltaTime;
            }
            else
            {
                stateController.SetActiveState(StateControllerBull.AIState.Rush);
            }
        }

        void DelayBeforeAttack()
        {
            globalRef.agent.speed = 0;

            if (globalRef.baseAttackBullSO.curentDelayBeforeAttack > 0)
            {
                globalRef.baseAttackBullSO.curentDelayBeforeAttack -= Time.deltaTime;
            }
            else
            {
                globalRef.hitBoxAttack.gameObject.SetActive(true);
            }
        }

        void SmoothLookAtPlayer()
        {
            Vector3 direction;
            Vector3 relativePos;

            direction = globalRef.playerTransform.position;

            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            SlowRotation(globalRef.isInEylau);
            Quaternion rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), globalRef.baseAttackBullSO.speedRot);
            globalRef.transform.rotation = rotation;
        }
        void SlowRotation(bool active)
        {
            if (active)
            {
                if (globalRef.baseAttackBullSO.speedRot < (globalRef.baseAttackBullSO.maxSpeedRot / globalRef.slowRatio))
                {
                    globalRef.baseAttackBullSO.speedRot += Time.deltaTime / (globalRef.baseAttackBullSO.smoothRot * globalRef.slowRatio);
                }
                else
                {
                    globalRef.baseAttackBullSO.speedRot = (globalRef.baseAttackBullSO.maxSpeedRot / globalRef.slowRatio);
                }
            }
            else
            {
                if (globalRef.baseAttackBullSO.speedRot < globalRef.baseAttackBullSO.maxSpeedRot)
                {
                    globalRef.baseAttackBullSO.speedRot += Time.deltaTime / globalRef.baseAttackBullSO.smoothRot;
                }
                else
                {
                    globalRef.baseAttackBullSO.speedRot = globalRef.baseAttackBullSO.maxSpeedRot;
                }
            }
        }

        private void OnDisable()
        {
            globalRef.agent.speed = globalRef.baseAttackBullSO.speed;
            globalRef.hitBoxAttack.gameObject.SetActive(false);
            attackDuration = 1;
            globalRef.baseAttackBullSO.curentDelayBeforeAttack = globalRef.baseAttackBullSO.maxDelayBeforeAttack;
        }
    }
}