using UnityEngine;

namespace State.AICAC
{
    public class IdleStateAICAC : _StateAICAC
    {
        [SerializeField] GlobalRefAICAC globalRef;

        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.BaseIdle;
        }

        private void Update()
        {
            StateIdle();
        }

        public void StateIdle()
        {
            if (globalRef.baseIdleAICACSO.currentDelayIdleState > 0)
            {
                globalRef.agent.speed = 0;
                globalRef.baseIdleAICACSO.currentDelayIdleState -= Time.deltaTime;
            }
            else
            {
                stateControllerAICAC.SetActiveState(StateControllerAICAC.AIState.BaseMove);
            }
        }

        private void OnDisable()
        {
            globalRef.baseIdleAICACSO.currentDelayIdleState = globalRef.baseIdleAICACSO.maxDelayIdleState;

            if (globalRef != null && globalRef.myAnimator != null)
                AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Idle");
        }
    }
}