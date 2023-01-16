using UnityEngine;
using UnityEngine.AI;

namespace State.WallAI
{
    public class BaseAttackWallAIState : _StateWallAI
    {
        protected StateControllerWallAI stateControllerWallAI;
        bool instanceSOIsCreate;

        BaseAttackWallAISO baseAttackWallAISO;
        [SerializeField] GlobalRefWallAI globalRef;

        public override void InitState(StateControllerWallAI stateController)
        {
            base.InitState(stateController);
            stateControllerWallAI = stateController;
            state = StateControllerWallAI.WallAIState.BaseAttack;
        }

        private void OnEnable()
        {
            try
            {
                globalRef.meshRenderer.enabled = true;
            }
            catch
            {
            }
        }

        private void Start()
        {
            baseAttackWallAISO = globalRef.baseAttackWallAISO;
        }

        private void Update()
        {
            if (state == StateControllerWallAI.WallAIState.BaseAttack)
            {
                LaunchAttack();
            }

            if (globalRef.enemyHealth._hp <= 0)
            {
                stateControllerWallAI.SetActiveState(StateControllerWallAI.WallAIState.Death, true);
            }
        }

        public void LaunchAttack()
        {
            globalRef.agent.speed = baseAttackWallAISO.stopSpeed;
            globalRef.animator.SetBool("LaunchAttack", true);
        }

        public float CalculateSpeedProjectile()
        {
            // v= d/t;
            // t = d/v;
            // d = v*t;

            Vector3 directionPlayer = Player.Instance.FinalMovement.normalized;

            switch (directionPlayer.magnitude)
            {
                case 0:
                    baseAttackWallAISO.playerPredicDir = globalRef.playerTransform.position;
                    baseAttackWallAISO.vPlayer = directionPlayer.magnitude;
                    baseAttackWallAISO.vProjectileGotToPredicPos = baseAttackWallAISO.defaultForceBullet;
                    break;
                default:
                    baseAttackWallAISO.playerPredicDir = globalRef.playerTransform.position + (directionPlayer * baseAttackWallAISO.distAnticipGround);
                    baseAttackWallAISO.vPlayer = directionPlayer.magnitude * 7f;

                    baseAttackWallAISO.timePlayerGoToPredicPos = Vector3.Distance(globalRef.playerTransform.position, baseAttackWallAISO.playerPredicDir) / baseAttackWallAISO.vPlayer;
                    baseAttackWallAISO.vProjectileGotToPredicPos = Vector3.Distance(globalRef.transform.position, baseAttackWallAISO.playerPredicDir) / baseAttackWallAISO.timePlayerGoToPredicPos;
                    break;
            }
            return baseAttackWallAISO.vProjectileGotToPredicPos;
        }

        public void ThrowProjectile()
        {
            globalRef.spawnBullet.LookAt(baseAttackWallAISO.playerPredicDir);
            Rigidbody cloneBullet = Instantiate(baseAttackWallAISO.bulletPrefab, globalRef.spawnBullet.position, globalRef.spawnBullet.rotation);
            cloneBullet.AddRelativeForce(Vector3.forward * CalculateSpeedProjectile(), ForceMode.VelocityChange);
        }

        public void ReturnBaseMoveState()
        {
            globalRef.animator.SetBool("LaunchAttack", false);
            stateControllerWallAI.SetActiveState(StateControllerWallAI.WallAIState.BaseMove, true);
        }

        ////////////////////////  ANIMATION EVENT \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public void StartAttack()
        {
            CalculateSpeedProjectile();
            ThrowProjectile();
        }
        public void EndAttack()
        {
            ReturnBaseMoveState();
        }
    }
}