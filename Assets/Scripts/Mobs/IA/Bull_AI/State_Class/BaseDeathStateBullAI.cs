using UnityEngine;

namespace State.AIBull
{
    public class BaseDeathStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.Death;
        }

        private void Update()
        {
            if (state == StateControllerBull.AIState.Death)
            {
                Debug.Log("Death");
            }
        }
    }
}