using UnityEngine;

namespace State.AIBull
{
    public class BaseDeathStateBullAI : _StateBull
    {
        [SerializeField] GlobalRefBullAI globalRef;
        bool once;

        public override void InitState(StateControllerBull stateController)
        {
            base.InitState(stateController);

            state = StateControllerBull.AIState.Death;
        }

        private void Update()
        {
            if(!once)
            {
                Death();
            }
        }

        void Death()
        {
            //AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Death");
            AnimatorManager.instance.StopAnimation(globalRef.myAnimator);
            globalRef.agent.speed = 0;
            globalRef.transform.parent = null;
            globalRef.rushManager.RemoveDeadAI(globalRef);
            // PLAY SOUND DEATH RUSHER
            // TODO lucas va te faire encul�

            globalRef.hitBoxRush.gameObject.SetActive(false);
            SoundManager.Instance.PlaySound("event:/SFX_IA/DeathIA", 1f, gameObject);
            once = true;
        }
    }
}