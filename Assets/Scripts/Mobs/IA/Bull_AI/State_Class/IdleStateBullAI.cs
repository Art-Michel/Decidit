using UnityEngine;

namespace State.AIBull
{
    public class IdleStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.Idle;
        }

        private void Update()
        {
            BaseIdle();
        }

        void BaseIdle() // Switch state BaseIdle To BaseMovement and restart RushMovementVariable
        {
            globalRef.agent.speed = globalRef.baseIdleBullSO.stopSpeed;

            if (globalRef.baseIdleBullSO.currentTransition > 0)
                globalRef.baseIdleBullSO.currentTransition -= Time.deltaTime;
            else
                stateController.SetActiveState(StateControllerBull.AIState.BaseMove);
        }

        private void OnDisable()
        {
            globalRef.baseIdleBullSO.currentTransition = globalRef.baseIdleBullSO.transitionDurationMax;

        }
    }
}