using UnityEngine;

namespace State.WallAI
{
    public class BaseDeathWallAIState : _StateWallAI
    {
        protected StateControllerWallAI stateControllerWallAI;

        public DeathWallAISO deathWallAISO;
        bool instanceSOIsCreate;

        [SerializeField] GlobalRefWallAI globalRef;

        bool once;

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
            if (!once)
                Death();
        }

        public void Death()
        {
            //PLAY SOUND DEATH WALL IA
            SoundManager.Instance.PlaySound("event:/SFX_IA/DeathIA", 1f, gameObject);
            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "IsDead");
            once = true;
        }
    }
}