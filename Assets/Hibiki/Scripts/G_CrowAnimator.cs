using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_CrowAnimator : MonoBehaviour
{
    private Animator animator;

    private readonly string riseAnim = "Rise", takeOffAnim = "OnTakeOff", landingAnim = "OnLanding";
    // Start is called before the first frame update
    void Start()
    {
        NullCheck();
    }

    public void OnLanding()
    {
        NullCheck();
        animator.SetTrigger(landingAnim);
    }

    public void OnTakeOff()
    {
        NullCheck();
        animator.SetTrigger(takeOffAnim);
    }

    public void SetRise(float value)
    {
        NullCheck();
        animator.SetFloat(riseAnim, Mathf.Clamp01(value));
    }

    private void NullCheck()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
    }
}
