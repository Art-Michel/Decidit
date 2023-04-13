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

    }
}