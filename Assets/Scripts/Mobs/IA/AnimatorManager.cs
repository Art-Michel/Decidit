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
        if(globalRefAnimator.currentAnimName != "")
            animator.SetBool(globalRefAnimator.currentAnimName, false);

        //Enable Next Animation
        if(nextAnimation != globalRefAnimator.currentAnimName)
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