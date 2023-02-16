using UnityEngine;

namespace State.AIBull
{
    public class BaseDeathStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;
        bool once;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.Death;
        }

        private void Update()
        {
            if(!once)
            {
                Death();
            }
        }

        void Death()
        {
            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Death");
            globalRef.agent.speed = 0;
            globalRef.rushManager.RemoveDeadAI(globalRef);
            // PLAY SOUND DEATH RUSHER
            once = true;
        }
    }
}