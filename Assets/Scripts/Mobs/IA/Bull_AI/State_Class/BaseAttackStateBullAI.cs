using UnityEngine;

namespace State.AIBull
{
    public class BaseAttackStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;

        [SerializeField] float attackDuration;

        Vector3 posPlayer;

        [SerializeField] bool canAttak;
        [SerializeField] bool launchAttack;

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

                posPlayer = globalRef.playerTransform.position;
                globalRef.agent.speed = 0;
            }
        }

        void Update()
        {
            if (!launchAttack)
            CheckCanAttack();

            if(launchAttack)
            {
                LaunchAttack();
            }
        }

        void CheckCanAttack()
        {
            if (canAttak)
            {
                if (/*Vector3.Distance(globalRef.transform.position, posPlayer) < globalRef.baseAttackBullSO.distLaunchAttack || */
                    Vector3.Distance(globalRef.transform.position, globalRef.playerTransform.position) < globalRef.baseAttackBullSO.distLaunchAttack)
                {
                    launchAttack = true;
                }
                else
                {
                    SmoothLookAtPlayer();
                    globalRef.agent.speed = 19;
                    globalRef.agent.SetDestination(globalRef.playerTransform.position);
                }
            }
            else
                SmoothLookAtPlayer();

            if (!canAttak)
                CheckObstaclePlayer();
        }

        void LaunchAttack()
        {
            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Idle");
            globalRef.agent.velocity = Vector3.zero;
            DelayBeforeAttack();
        }

        bool CheckObstaclePlayer()
        {
            Vector3 thisPosition = globalRef.transform.position - globalRef.transform.forward;
            Vector3 thisForward = globalRef.transform.forward;

            float dot = Vector3.Dot(thisForward, (globalRef.playerTransform.position - thisPosition).normalized);
            if (dot > 0.95f)
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
            stateController.SetActiveState(StateControllerBull.AIState.Rush);

            /*if (attackDuration >0)
            {
                attackDuration -= Time.deltaTime;
            }
            else
            {
                stateController.SetActiveState(StateControllerBull.AIState.Rush);
            }*/
        }

        void DelayBeforeAttack()
        {
            Debug.Log("Attack");

            if (globalRef.baseAttackBullSO.curentDelayBeforeAttack >= 0)
            {
                globalRef.baseAttackBullSO.curentDelayBeforeAttack -= Time.deltaTime;
                if (globalRef.material_Instances.Material[0].mainTexture != globalRef.material_Instances.TextureBase)
                    ShowSoonAttack(true);
            }
            else
            {
                globalRef.hitBoxAttack.gameObject.SetActive(true);
                if (globalRef.material_Instances.Material[0].mainTexture == globalRef.material_Instances.TextureBase)
                    ShowSoonAttack(false);

                Invoke("AttackDuration", 0.5f);
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

        void ShowSoonAttack(bool active)
        {
            if (active)
            {
                for (int i = 0; i < globalRef.material_Instances.Material.Length; i++)
                {
                    globalRef.material_Instances.Material[0].color = globalRef.material_Instances.ColorPreAtatck;
                }
                globalRef.material_Instances.ChangeColorTexture(globalRef.material_Instances.ColorPreAtatck);
            }
            else
            {
                for (int i = 0; i < globalRef.material_Instances.Material.Length; i++)
                {
                    globalRef.material_Instances.Material[0].color = globalRef.material_Instances.ColorBase;
                }
                globalRef.material_Instances.ChangeColorTexture(globalRef.material_Instances.ColorBase);
            }
        }

        private void OnDisable()
        {
            launchAttack = false;
            globalRef.agent.speed = globalRef.baseAttackBullSO.speed;
            globalRef.hitBoxAttack.gameObject.SetActive(false);
            attackDuration = 1;
            globalRef.baseAttackBullSO.curentDelayBeforeAttack = globalRef.baseAttackBullSO.maxDelayBeforeAttack;
        }
    }
}