using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State.AICAC
{
    public class AnimEventAICAC : MonoBehaviour
    {
        [SerializeField] GlobalRefAICAC globalRef;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void PreAttack()
        {
            globalRef.material_Instances.Material.color = globalRef.material_Instances.ColorPreAtatck;
            globalRef.material_Instances.ChangeColorTexture(globalRef.material_Instances.ColorPreAtatck);
        }
        void LaunchAttack()
        {
            globalRef.material_Instances.Material.color = globalRef.material_Instances.Color;
            globalRef.material_Instances.ChangeColorTexture(globalRef.material_Instances.Color);
            globalRef.hitBox.SetActive(true);
        }
        void EndAttack()
        {
            globalRef.hitBox.SetActive(false);
            globalRef.myAnimator.SetBool("Attack", false);
            globalRef.baseAttackAICACSO.isAttacking = false;
        }
    }
}