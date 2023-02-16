using UnityEngine;

namespace State.AIBull
{
    public class AnimEventRusher : MonoBehaviour
    {
        [SerializeField] GlobalRefBullAI globalRef;
        [SerializeField] BaseMoveStateBullAI baseMoveState;

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
    }
}