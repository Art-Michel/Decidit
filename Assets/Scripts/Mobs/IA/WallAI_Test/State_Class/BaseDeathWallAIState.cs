using UnityEngine;

namespace State.WallAI
{
    public class BaseDeathWallAIState : _StateWallAI
    {
        protected StateControllerWallAI stateControllerWallAI;

        public DeathWallAISO deathWallAISO;
        bool instanceSOIsCreate;

        GlobalRefWallAI globalRef;

        public override void InitState(StateControllerWallAI stateController)
        {
            if (!instanceSOIsCreate)
            {
                deathWallAISO = Instantiate(deathWallAISO);
                instanceSOIsCreate = true;
            }

            base.InitState(stateController);

            state = StateControllerWallAI.WallAIState.Death;
        }

        private void Update()
        {
            if (state == StateControllerWallAI.WallAIState.Death)
            {
                Debug.Log("Death");
            }
        }

        public void Death()
        {
            globalRef.myAnimator.SetBool("IsDead", true);
        }
    }
}