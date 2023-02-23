using UnityEngine;

namespace State.AICAC
{
    public class BaseAttackStateAICAC : _StateAICAC
    {
        [SerializeField] GlobalRefAICAC globalRef;
        [SerializeField] Material_Instances material_Instances;
        BaseAttackParameterAICAC baseAttackAICACSO;

        [SerializeField] Transform tongue;

        [SerializeField] bool endAttack;

        [Header("LookAt Variable")]
        Vector3 direction;
        Vector3 relativePos;
        Quaternion rotation;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.BaseAttack;
        }

        private void OnEnable()
        {
            if (globalRef != null)
            {
                baseAttackAICACSO = globalRef.baseAttackAICACSO;
            }
        }

        private void Update()
        {
            BaseAttack();
            SmoothLookAt();
        }

        private void LateUpdate()
        {
            if (baseAttackAICACSO.currentAttackRate <=0.1f)
                tongue.LookAt(globalRef.playerTransform.position);
        }

        public void BaseAttack()
        {
            globalRef.agent.speed = 0;

            if (baseAttackAICACSO.currentAttackRate <= 0)
            {
                AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Attack");
                baseAttackAICACSO.isAttacking = true;
                baseAttackAICACSO.currentAttackRate = baseAttackAICACSO.maxAttackRate;
            }
            else if (!baseAttackAICACSO.isAttacking)
            {
                if (globalRef.distPlayer > baseAttackAICACSO.attackRange && baseAttackAICACSO.currentAttackRate == baseAttackAICACSO.maxAttackRate)
                {
                    stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseMove);
                    return;
                }
                else if(material_Instances.Material.color != material_Instances.ColorPreAtatck)
                {
                    material_Instances.ChangeColorTexture(material_Instances.ColorPreAtatck);
                }
                if(globalRef.distPlayer < baseAttackAICACSO.attackRange && baseAttackAICACSO.currentAttackRate == baseAttackAICACSO.maxAttackRate)
                {
                    //SoundManager.instance.PlaySoundMobOneShot(globalRef.audioSourceTrashMob, SoundManager.instance.soundAndVolumeListTrashMob[0]);
                    // TO DO lucas va te faire encul�
                    SoundManager.Instance.PlaySound("event:/SFX_IA/Voras_SFX(Trash)/Attack", 1f);
                    // PLAY SOUND PRE ATTACK TRASH MOB
                }

                baseAttackAICACSO.currentAttackRate -= Time.deltaTime;
            }
        }

        public void SmoothLookAt()
        {
            direction = globalRef.playerTransform.position;
            relativePos.x = direction.x - globalRef.transform.position.x;
            relativePos.y = 0;
            relativePos.z = direction.z - globalRef.transform.position.z;

            SlowRotation(globalRef.isInEylau);

            rotation = Quaternion.Slerp(globalRef.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), baseAttackAICACSO.speedRot);
            globalRef.transform.rotation = rotation;
        }

        void SlowRotation(bool active)
        {
            if (active)
            {
                if (baseAttackAICACSO.speedRot < (baseAttackAICACSO.maxSpeedRot / globalRef.slowRatio))
                {
                    baseAttackAICACSO.speedRot += Time.deltaTime / (baseAttackAICACSO.smoothRot * globalRef.slowRatio);
                }
                else
                {
                    baseAttackAICACSO.speedRot = (baseAttackAICACSO.maxSpeedRot / globalRef.slowRatio);
                }
            }
            else
            {
                if (baseAttackAICACSO.speedRot < baseAttackAICACSO.maxSpeedRot)
                {
                    baseAttackAICACSO.speedRot += Time.deltaTime / baseAttackAICACSO.smoothRot;
                }
                else
                {
                    baseAttackAICACSO.speedRot = baseAttackAICACSO.maxSpeedRot;
                }
            }
        }

        private void OnDisable()
        {
            if(material_Instances != null)
            {
                material_Instances.ChangeColorTexture(material_Instances.ColorBase);
            }
            baseAttackAICACSO.currentAttackRate = baseAttackAICACSO.maxAttackRate;
            baseAttackAICACSO.speedRot = 0;
        }
    }
}