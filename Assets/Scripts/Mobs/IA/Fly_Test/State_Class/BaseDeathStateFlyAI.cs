using UnityEngine;

namespace State.FlyAI
{
    public class BaseDeathStateFlyAI : _StateFlyAI
    {
        public override void InitState(StateControllerFlyAI stateController)
        {
            base.InitState(stateController);

            state = StateControllerFlyAI.AIState.Death;
        }

        private void Update()
        {
        }
    }
}