using UnityEngine;

public class CharacterView : MonoBehaviour
{
    public Animator animator;

    public void PlayIdleAnimation()
    {
        if (animator != null)
            animator.Play("Idle");
    }

    public void PlayWalkAnimation()
    {
        if (animator != null)
            animator.Play("Walk");
    }

    public void PlayWaveAnimation()
    {
        if (animator != null)
            animator.Play("Wave");
    }
}
