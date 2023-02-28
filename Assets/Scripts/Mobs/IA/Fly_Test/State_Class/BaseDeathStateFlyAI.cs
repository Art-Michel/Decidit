using UnityEngine;

namespace State.FlyAI
{
    public class BaseDeathStateFlyAI : _StateFlyAI
    {
        [SerializeField] GlobalRefFlyAI globalRef;
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
            if (globalRef != null && globalRef.myAnimator != null)
                AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Death");

            globalRef.agent.enabled = false;
            globalRef.colliderBaseAttack.SetActive(false);

            // PLAY SOUND DEATH FLY AI
            SoundManager.Instance.PlaySound("event:/SFX_IA/DeathIA", 1f, gameObject);
            // TO DO lucas va te faire encul�
            once = true;
        }
    }
}