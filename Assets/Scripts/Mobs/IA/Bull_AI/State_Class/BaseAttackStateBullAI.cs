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
        public bool attackLaunched;
        public bool isAttacking;
        public bool attackDone;

        AnimatorStateInfo animStateInfo;
        AnimatorClipInfo[] currentClipInfo;
        public string currentAnimName;
        public float animTime;

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

            if (animTime > 1.0f && currentAnimName == "PreAttackIdleCAC" && !isAttacking)
                Attack();
            else if (animTime > 1.0f && currentAnimName == "Attack" && !attackDone)
                EndAttack();

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
        }

        void CheckCanAttack()
        {
            if (canAttak)
            {
                if (/*Vector3.Distance(globalRef.transform.position, posPlayer) < globalRef.baseAttackBullSO.distLaunchAttack || */
                    Vector3.Distance(globalRef.transform.position, globalRef.playerTransform.position) < globalRef.baseAttackBullSO.distLaunchAttack)
                {
                    launchAttack = true;
                    globalRef.agent.speed = 0;
                }
                else
                {
                    Debug.Log("Follow Player");
                    SmoothLookAtPlayer();
                    globalRef.agent.speed = 19;
                    globalRef.agent.SetDestination(globalRef.playerTransform.position);
                }
            }
            else
                SmoothLookAtPlayer();

            if (!canAttak)
                CheckPlayerIsBack();
        }
        bool CheckPlayerIsBack()
        {
            Vector2 thisPosition = new Vector2(globalRef.transform.position.x, globalRef.transform.position.z) - new Vector2(globalRef.transform.forward.x, globalRef.transform.forward.z);
            Vector2 posPlayer = new Vector2(globalRef.playerTransform.position.x, globalRef.playerTransform.position.z);
            Vector2 thisForward = new Vector2(globalRef.transform.forward.x, globalRef.transform.forward.z);

            float dot = Vector2.Dot(thisForward, (posPlayer - thisPosition).normalized);

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
            Debug.Log("Attack");

            isAttacking = true;
            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Attack");
            globalRef.hitBoxAttack.gameObject.SetActive(true);
            if (globalRef.material_Instances.Material[0].mainTexture == globalRef.material_Instances.TextureBase)
                ShowSoonAttack(false);
        }
        void EndAttack()
        {
            Debug.Log("End Attack");

            globalRef.hitBoxAttack.gameObject.SetActive(false);
            isAttacking = false;
            attackLaunched = false;
            attackDone = true;
        }
        void NextAttack()
        {
            int i = Random.Range(0, 2);
            if (i == 0)
                stateController.SetActiveState(StateControllerBull.AIState.Rush);
            else
                stateController.SetActiveState(StateControllerBull.AIState.BaseAttack);
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
            attackDone = false;
            isAttacking = false;
            globalRef.agent.speed = globalRef.baseAttackBullSO.speed;
            globalRef.hitBoxAttack.gameObject.SetActive(false);
            attackDuration = 1;
            globalRef.baseAttackBullSO.curentDelayBeforeAttack = globalRef.baseAttackBullSO.maxDelayBeforeAttack;
            globalRef.baseAttackBullSO.speedRot = 0;
        }
    }
}