using UnityEngine;

namespace State.AICAC
{
    public class AnimEventAICAC : MonoBehaviour
    {
        [SerializeField] GlobalRefAICAC globalRef;
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
    }
}