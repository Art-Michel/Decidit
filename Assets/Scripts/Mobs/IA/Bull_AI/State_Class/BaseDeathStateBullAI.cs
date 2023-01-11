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

        private void OnEnable()
        {
            if(globalRef.isDead)
            {
                globalRef.bullCount.RemoveAI(globalRef.transform);
                globalRef.agent.speed = 0;
                Debug.Log("Death");
            }
        }
    }
}