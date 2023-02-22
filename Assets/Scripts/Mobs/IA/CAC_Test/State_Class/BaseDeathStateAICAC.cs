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
            if(!once)
                Death();
        }

        void Death()
        {
            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Death");
            globalRef.transform.parent = null;
            globalRef.agent.speed = globalRef.deathAICACSO.stopSpeed;
            globalRef.agent.enabled = false;
            // PLAY SOUND DEATH TRASHMOB
            // TO DO lucas va te faire enculé
            once = true;
        }
    }
}