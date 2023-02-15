using UnityEngine;

namespace State.AICAC
{
    public class AnimEventAICAC : MonoBehaviour
    {
        [SerializeField] GlobalRefAICAC globalRef;
        [SerializeField] BaseMoveStateAICAC baseMoveState;
        /*void PreAttack()
        {
            globalRef.material_Instances.Material.color = globalRef.material_Instances.ColorPreAtatck;
            globalRef.material_Instances.ChangeColorTexture(globalRef.material_Instances.ColorPreAtatck);
        }*/
        void LaunchAttack()
        {
            globalRef.material_Instances.Material.color = globalRef.material_Instances.ColorBase;
            globalRef.material_Instances.ChangeColorTexture(globalRef.material_Instances.ColorBase);
            globalRef.hitBox.SetActive(true);
        }
        void LaunchSoundAttack()
        {
            // PLAY SOUND ATTACK TRASHMOB
            //SoundManager.instance.PlaySoundMobOneShot(globalRef.audioSourceTrashMob, SoundManager.instance.soundAndVolumeListTrashMob[1]);
        }
        void EndAttack()
        {
            globalRef.hitBox.SetActive(false);
            globalRef.baseAttackAICACSO.isAttacking = false;
            AnimatorManager.instance.DisableAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Attack");
            //globalRef.myAnimator.SetBool("Attack", false);
        }


        // JUMP ANIM
        private void StartJump()
        {
            globalRef.agent.autoTraverseOffMeshLink = true;
        }
        public void EndJump()
        {
            Invoke("Walk", 0.4f);
        }
        void Walk()
        {
            baseMoveState.isOnNavLink = false;
            globalRef.agent.autoTraverseOffMeshLink = true;
            AnimatorManager.instance.SetAnimation(globalRef.myAnimator, globalRef.globalRefAnimator, "Walk");
        }
    }
}