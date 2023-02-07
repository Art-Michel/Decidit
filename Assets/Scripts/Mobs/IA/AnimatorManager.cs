using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public static AnimatorManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void SetAnimation(Animator animator, GlobalRefAnimator globalRefAnimator, string nextAnimation)
    {
        //Disable Current Animation
        if(globalRefAnimator.currentAnimName != null)
            animator.SetBool(globalRefAnimator.currentAnimName, false);

        //Enable Next Animation
        animator.SetBool(nextAnimation, true);

        //Enable Current Animation
        globalRefAnimator.currentAnimName = nextAnimation;
    }
    public void DisableAnimation(Animator animator, GlobalRefAnimator globalRefAnimator, string nameAnimation)
    {
        //Disable Current Animation
        animator.SetBool(nameAnimation, false);
        globalRefAnimator.currentAnimName = null;
    }
}