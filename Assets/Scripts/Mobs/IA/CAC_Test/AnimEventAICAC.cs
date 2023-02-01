using System.Collections;
using System.Collections.Generic;
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
        void EndAttack()
        {
            globalRef.hitBox.SetActive(false);
            globalRef.baseAttackAICACSO.isAttacking = false;
            globalRef.myAnimator.SetBool("Attack", false);
        }
    }
}