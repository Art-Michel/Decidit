using UnityEngine;

namespace State.AICAC
{
    public class BaseDeathState : _StateAICAC
    {
        [SerializeField] GlobalRefAICAC globalRef;
        bool once;
        public override void InitState(StateControllerAICAC stateController)
        {
            base.InitState(stateController);

            state = StateControllerAICAC.AIState.BaseDeath;
        }

        private void Update()
        {
            if (state == StateControllerAICAC.AIState.BaseDeath)
            {
                Death();
            }
        }

        void Death()
        {
            globalRef.transform.parent = null;
            globalRef.agent.speed = globalRef.deathAICACSO.stopSpeed;
            if (!once)
            {
                //stateManagerAICAC.aICACVarianteState.SetListActiveAI();
                once = true;
            }
        }
    }
}