using UnityEngine;

namespace State.AIBull
{
    public class BaseAttackStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;

        [SerializeField] bool canAttak;
        [SerializeField] bool launchAttack;
        public bool attackLaunched;
        public bool isAttacking;
        public bool attackDone;

        AnimatorStateInfo animStateInfo;
        AnimatorClipInfo[] currentClipInfo;
        public string currentAnimName;
        public float animTime;

        Vector3 catchPlayerPos;

        [SerializeField] float currentSpeed;

        bool hitPlayer;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.BaseAttack;
        }

        private void OnEnable()
        {
            RestartAttack();

            if (globalRef != null)
            {
                canAttak = false;
                currentSpeed = 0;
                globalRef.agent.speed = currentSpeed;
            }
        }

        void Update()
        {
            currentClipInfo = globalRef.myAnimator.GetCurrentAnimatorClipInfo(0);
            currentAnimName = currentClipInfo[0].clip.name;

            animStateInfo = globalRef.myAnimator.GetCurrentAnimatorStateInfo(0);
            animTime = animStateInfo.normalizedTime;

            if(globalRef.hitBoxAttack.isActiveAndEnabled)
            {
                if (globalRef.hitBoxAttack.Blacklist.Count > 0)
                {
                    Debug.Log(globalRef.hitBoxAttack.Blacklist.Count);
                    globalRef.hitBoxAttack.gameObject.SetActive(false);
                }
            }

            ManageAnimation();

            if (!isAttacking && !attackDone)
            {
                if (!launchAttack)
                    CheckCanAttack();

                if (launchAttack && !attackLaunched)
                {
                    LaunchPreAttack();
                }
            }
            else if(attackDone)
            {
                NextAttack();
            }

            SlowSpeed(globalRef.isInEylau || globalRef.IsZap);
        }
        void CheckCanGoToPlayer()
        {
            Vector3 dir = globalRef.playerTransform.position - globalRef.RayAttackMiddle.position;

            RaycastHit hit = RaycastAIManager.instanceRaycast.RaycastAI(globalRef.RayAttackMiddle.position,
                                                                        dir,
                                                                        globalRef.rushBullSO.maskCheckCanRush, Color.red, 100f);
            if(hit.transform == null)
            {
                stateController.SetActiveState(StateControllerBull.AIState.BaseMove);
            }
            else if(hit.transform.gameObject.layer == 9)
            {
                stateController.SetActiveState(StateControllerBull.AIState.BaseMove);
            }
            else if(hit.distance > globalRef.baseMoveBullSO.distActiveAttack && hit.transform.CompareTag("Player"))
            {
                stateController.SetActiveState(StateControllerBull.AIState.BaseMove);
            }
        }

        void SlowSpeed(bool active)
        {
            if (active)
            {
                globalRef.slowSpeed = currentSpeed / globalRef.slowRatio;
                globalRef.agent.speed = globalRef.slowSpeed;
            }
            else
            {
                globalRef.agent.speed = currentSpeed;
            }
        }

        void ManageAnimation()
        {
            if (animTime > 1.0f && currentAnimName == "PreAttack" && !isAttacking)
                Attack();
            else if (animTime > 1.0f && currentAnimName == "Attack" && !attackDone && isAttacking)
                EndAttack();

            if(currentAnimName == "Attack" && animTime <1)
            {
                if(animTime <= 0.2f)
                {
                    catchPlayerPos = globalRef.playerTransform.position;
                }
                else if(animTime <= 0.5f)
                {
                    if(!hitPlayer)
                    {
                        globalRef.hitBoxAttack.gameObject.SetActive(true);
                        hitPlayer = true;
                    }
                    if (Vector3.Distance(catchPlayerPos, globalRef.transform.position) > 1f)
                    {
                        currentSpeed = 15;
                        globalRef.agent.SetDestination(globalRef.transform.position + transform.forward*10);
                    }
                    else
                    {
                        currentSpeed = 0;
                    }
                }
                else
                {
                    globalRef.hitBoxAttack.gameObject.SetActive(false);
                    currentSpeed = 0;
                    hitPlayer = false;
                }
            }
        }

        void CheckCanAttack()
        {
            if (canAttak)
            {
                if (globalRef.distPlayer < globalRef.baseAttackBullSO.distLaunchAttack)
                {
                    launchAttack = true;
                    currentSpeed = 0;
                }
                else if(!attackDone)
                {
                    SmoothLookAtPlayer();
                    CheckCanGoToPlayer();
                    currentSpeed = 19;
                    globalRef.agent.SetDestination(globalRef.playerTransform.position);
                    if (AnimatorManager.instance.GetCurrentAnimatonName(globalRef.globalRefAnimator) != "Walk" && animTime > 1)
                    {
                        AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Walk");
                    }
                }
            }
            else
                SmoothLookAtPlayer();

            if (!canAttak)
            {
                if(CheckPlayerIsBack())
                {
                    canAttak = true;
                }
                else
                {
                    canAttak = false;
                }
            }
        }
        bool CheckPlayerIsBack()
        {
            Vector2 thisPosition = new Vector2(globalRef.transform.position.x, globalRef.transform.position.z) - new Vector2(globalRef.transform.forward.x, globalRef.transform.forward.z);
            Vector2 posPlayer = new Vector2(globalRef.playerTransform.position.x, globalRef.playerTransform.position.z);
            Vector2 thisForward = new Vector2(globalRef.transform.forward.x, globalRef.transform.forward.z);

            float dot = Vector2.Dot(thisForward, (posPlayer - thisPosition).normalized);

            if (dot > 0.95f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void LaunchPreAttack()
        {
            globalRef.agent.velocity = Vector3.zero;
            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "PreAttackCAC");

            if (globalRef.material_Instances.Material[0].mainTexture != globalRef.material_Instances.TextureBase)
                ShowSoonAttack(true);

            attackLaunched = true;
        }
        void Attack()
        {
            isAttacking = true;
            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Attack");
            if (globalRef.material_Instances.Material[0].mainTexture == globalRef.material_Instances.TextureBase)
                ShowSoonAttack(false);
        }
        void EndAttack()
        {
            globalRef.hitBoxAttack.gameObject.SetActive(false);
            isAttacking = false;
            attackLaunched = false;
            attackDone = true;
            currentSpeed = 0;
        }
        void NextAttack()
        {
            int i = Random.Range(0, 2);
            if (i == 0)
                stateController.SetActiveState(StateControllerBull.AIState.Rush);
            else
                stateController.SetActiveState(StateControllerBull.AIState.BaseMove);
        }

        void SmoothLookAtPlayer()
        {
            Vector3 direction;
            Vector3 relativePos;

            direction = globalRef.playerTransform.position;

            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            SlowRotation(globalRef.isInEylau || globalRef.IsZap);
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

        void RestartAttack()
        {
            launchAttack = false;
            canAttak = false;
            isAttacking = false;
            attackDone = false;
            attackLaunched = false;
            globalRef.baseAttackBullSO.curentDelayBeforeAttack = globalRef.baseAttackBullSO.maxDelayBeforeAttack;
        }

        private void OnDisable()
        {
            RestartAttack();
            globalRef.agent.speed = globalRef.baseAttackBullSO.speed;
            globalRef.hitBoxAttack.gameObject.SetActive(false);
            globalRef.baseAttackBullSO.speedRot = 0;
        }
    }
}