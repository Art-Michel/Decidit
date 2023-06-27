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
            Debug.Log(globalRef.enemyHealth.Room.CurrentEnemiesInRoom);

            if (globalRef.enemyHealth.Room.CurrentEnemiesInRoom == 0)
            {
                globalRef.vfxShowDoor.SetActive(true);
                Debug.Log("Active VFX ShowDoor");
            }

            globalRef.managerAnticipMoveTrash.RemoveRef(globalRef);
            globalRef.managerSurroundTrash.RemoveRef(globalRef);
            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Death");
            globalRef.transform.parent = null;
            globalRef.agent.speed = globalRef.deathAICACSO.stopSpeed;
            globalRef.agent.enabled = false;
            globalRef.hitBox.gameObject.SetActive(false);
            // PLAY SOUND DEATH TRASHMOB
            // TODO lucas va te faire encul�
            SoundManager.Instance.PlaySound("event:/SFX_IA/DeathIA", 1f, gameObject);
            once = true;
        }
    }
}