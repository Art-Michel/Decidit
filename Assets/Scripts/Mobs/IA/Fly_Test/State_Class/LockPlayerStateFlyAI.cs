using UnityEngine;

namespace State.FlyAI
{
    public class LockPlayerStateFlyAI : _StateFlyAI
    {
        [SerializeField] GlobalRefFlyAI globalRef;
        [SerializeField] Material_Instances material_Instances;
        LockPlayerFlySO lockPlayerFlySO;
        BaseAttackFlySO baseAttackFlySO;
        Quaternion rotation;

        [SerializeField] Transform childflyAI;

        public override void InitState(StateControllerFlyAI stateController)
        {
            base.InitState(stateController);

            state = StateControllerFlyAI.AIState.LockPlayer;
        }

        private void Start()
        {
            lockPlayerFlySO = globalRef.lockPlayerFlySO;
            baseAttackFlySO = globalRef.baseAttackFlySO;
        }

        private void OnEnable()
        {
            if (globalRef != null && globalRef.myAnimator != null)
                AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "PreAttack");

            try
            {
                globalRef.colliderBaseAttack.gameObject.SetActive(false);
                //PLAY SOUND PRE ATTACK FLY IA
                // TODO lucas va te faire enculé
            }
            catch
            {
                Debug.LogWarning("Missing Reference");
            }
        }

        private void Update()
        {
            if(CheckPlayerCover.isCover)
                stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseMove);

            LockPlayer();
            SmoothLookAtYAxisAttack();
        }

        public void LockPlayer()
        {
            lockPlayerFlySO.destinationFinal = new Vector3(globalRef.playerTransform.position.x, globalRef.playerTransform.position.y - lockPlayerFlySO.offsetYpos, globalRef.playerTransform.position.z);

            if (baseAttackFlySO.speedRotationAIAttack >= 1f)
            {
                stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseAttack);
            }
            else
            {
                if (ThisStateIsActive())
                {
                    material_Instances.Material.color = material_Instances.ColorPreAtatck;
                    material_Instances.ChangeColorTexture(material_Instances.ColorPreAtatck);
                }
            }
        }
        public void SmoothLookAtYAxisAttack()
        {
            Vector3 relativePos;

            relativePos.x = lockPlayerFlySO.destinationFinal.x - globalRef.transform.position.x;
            relativePos.y = lockPlayerFlySO.destinationFinal.y - globalRef.transform.position.y;
            relativePos.z = lockPlayerFlySO.destinationFinal.z - globalRef.transform.position.z;

            SlowRotation(globalRef.isInEylau, relativePos);

            Quaternion rotation = Quaternion.Slerp(childflyAI.localRotation, Quaternion.LookRotation(relativePos, Vector3.up), baseAttackFlySO.speedRotationAIAttack);
            childflyAI.localRotation = rotation;
        }
        void SlowRotation(bool active, Vector3 relativePos)
        {
            if(active)
            {
                if (this.isActiveAndEnabled == true)
                {
                    if (baseAttackFlySO.speedRotationAIAttack < baseAttackFlySO.maxSpeedRotationAILock)
                        baseAttackFlySO.speedRotationAIAttack += (Time.deltaTime / (baseAttackFlySO.smoothRotationLock * globalRef.slowRatio));
                    else
                        baseAttackFlySO.speedRotationAIAttack = baseAttackFlySO.maxSpeedRotationAILock;
                }
                else
                {
                    if (baseAttackFlySO.speedRotationAIAttack < baseAttackFlySO.maxSpeedRotationAIAttack)
                        baseAttackFlySO.speedRotationAIAttack += (Time.deltaTime / (baseAttackFlySO.smoothRotationAttack * globalRef.slowRatio));
                    else
                        baseAttackFlySO.speedRotationAIAttack = baseAttackFlySO.maxSpeedRotationAIAttack;
                }
            }
            else
            {
                if (this.isActiveAndEnabled == true)
                {
                    if (baseAttackFlySO.speedRotationAIAttack < baseAttackFlySO.maxSpeedRotationAILock)
                        baseAttackFlySO.speedRotationAIAttack += (Time.deltaTime / baseAttackFlySO.smoothRotationLock);
                    else
                        baseAttackFlySO.speedRotationAIAttack = baseAttackFlySO.maxSpeedRotationAILock;
                }
                else
                {
                    if (baseAttackFlySO.speedRotationAIAttack < baseAttackFlySO.maxSpeedRotationAIAttack)
                        baseAttackFlySO.speedRotationAIAttack += (Time.deltaTime / baseAttackFlySO.smoothRotationAttack);
                    else
                        baseAttackFlySO.speedRotationAIAttack = baseAttackFlySO.maxSpeedRotationAIAttack;
                }
            }
        }

        bool ThisStateIsActive()
        {
            if (this.gameObject.activeInHierarchy)
                return true;
            else
                return false;
        }

        private void OnDisable()
        {
            baseAttackFlySO.speedRotationAIAttack = 0;
            material_Instances.Material.color = material_Instances.ColorBase;
            material_Instances.ChangeColorTexture(material_Instances.ColorBase);
        }
    }
}