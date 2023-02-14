using UnityEngine;

namespace State.FlyAI
{
    public class BaseDeathStateFlyAI : _StateFlyAI
    {
        bool once;

        public override void InitState(StateControllerFlyAI stateController)
        {
            base.InitState(stateController);

            state = StateControllerFlyAI.AIState.Death;
        }

        private void Update()
        {
            if (!once)
                Death();
        }

        void Death()
        {
            // PLAY SOUND DEATH FLY AI
        }
    }
}