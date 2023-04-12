using UnityEngine;

namespace State.AIBull
{
    public class AnimEventRusher : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;
        [SerializeField] BaseMoveStateBullAI baseMoveState;
        [SerializeField] BaseAttackStateBullAI baseAttackState;

        // JUMP ANIM
        public void EndJump()
        {
            Invoke("Walk", 0.4f);
        }
        void Walk()
        {
            baseMoveState.isOnNavLink = false;
            globalRef.agent.autoTraverseOffMeshLink = true;
            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Walk");
        }

        // Attack ANIM
        public void LaunchAttack()
        {
            Debug.Log("LaunchAttack");
            baseAttackState.isAttacking = true;

            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Attack");

            globalRef.baseAttackBullSO.curentDelayBeforeAttack -= Time.deltaTime;
            if (globalRef.material_Instances.Material[0].mainTexture != globalRef.material_Instances.TextureBase)
                ShowSoonAttack(true);
        }
        public void Attack()
        {
            globalRef.hitBoxAttack.gameObject.SetActive(true);

            globalRef.hitBoxAttack.gameObject.SetActive(true);
            if (globalRef.material_Instances.Material[0].mainTexture == globalRef.material_Instances.TextureBase)
                ShowSoonAttack(false);
        }
        public void EndAttack()
        {
            globalRef.hitBoxAttack.gameObject.SetActive(false);
            baseAttackState.isAttacking = false;
            baseAttackState.attackDone = true;
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

    }
}