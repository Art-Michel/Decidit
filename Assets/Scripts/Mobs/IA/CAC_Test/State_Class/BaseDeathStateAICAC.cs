using UnityEngine;

namespace State.AICAC
{
    public class BaseDeathStateAICAC : _StateAICAC
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
            Debug.Log("Ta grand mere la pute");
            Death();
        }

        void Death()
        {
            globalRef.transform.parent = null;
            globalRef.agent.speed = globalRef.deathAICACSO.stopSpeed;
            globalRef.agent.enabled = false;

            Debug.Log(globalRef.agent.speed);

            /*if (!once)
            {
                globalRef.transform.parent = null;
                globalRef.agent.speed = globalRef.deathAICACSO.stopSpeed;
                globalRef.agent.enabled = false;
                once = true;
            }*/
        }
    }
}