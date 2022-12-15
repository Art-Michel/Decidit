using UnityEngine;

namespace State.FlyAI
{
    public class LockPlayerStateFlyAI : _StateFlyAI
    {
        [SerializeField] GlobalRefFlyAI globalRef;
        [SerializeField] Material_Instances material_Instances;
        LockPlayerFlySO lockPlayerFlySO;
        BaseAttackFlySO baseAttackFlySO;

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

        private void Update()
        {
            if (state == StateControllerFlyAI.AIState.LockPlayer)
            {
                Debug.Log("Idle");
                LockPlayer();
                SmoothLookAtYAxisAttack();
            }
        }

        public void LockPlayer()
        {
            globalRef.colliderBaseAttack.gameObject.SetActive(false);

            lockPlayerFlySO.destinationFinal = new Vector3(globalRef.playerTransform.position.x, globalRef.playerTransform.position.y - 1f, globalRef.playerTransform.position.z);

            if (baseAttackFlySO.speedRotationAIAttack >= 1f)
            {
                Vector3 destinationFinal2D = new Vector2(lockPlayerFlySO.destinationFinal.x, lockPlayerFlySO.destinationFinal.z);
                Vector3 transformPos2D = new Vector2(globalRef.transform.position.x, globalRef.transform.position.z);

                baseAttackFlySO.timeGoToDestinationAttack = Vector3.Distance(destinationFinal2D, transformPos2D) / baseAttackFlySO.baseAttackSpeed;
                baseAttackFlySO.maxSpeedYTranslationAttack = Mathf.Abs(lockPlayerFlySO.destinationFinal.y - globalRef.transform.position.y) / baseAttackFlySO.timeGoToDestinationAttack;

                stateControllerFlyAI.SetActiveState(StateControllerFlyAI.AIState.BaseAttack);
            }
            else
            {
                if (ThisStateIsActive())
                {
                    Debug.Log("Set Red color");
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

            Quaternion rotation = Quaternion.Slerp(childflyAI.localRotation, Quaternion.LookRotation(relativePos, Vector3.up), baseAttackFlySO.speedRotationAIAttack);
            childflyAI.localRotation = rotation;

            if (baseAttackFlySO.speedRotationAIAttack < baseAttackFlySO.maxSpeedRotationAIAttack)
            {
                baseAttackFlySO.speedRotationAIAttack += (Time.deltaTime / baseAttackFlySO.smoothRotationAttack);
                //lerpSpeedYValueAttack += (Time.deltaTime / ySpeedSmootherAttack);
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
            Debug.Log("Set Black color");
            material_Instances.Material.color = material_Instances.Color;
            material_Instances.ChangeColorTexture(material_Instances.Color);
        }
    }
}