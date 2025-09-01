using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class G_NornAnimator : MonoBehaviour
{
    [SerializeField] Animations animations;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Trun()
    {
        animator.SetTrigger(animations.turn);
    }

    public void Shocked()
    {
        animator.SetTrigger(animations.shocked);
    }

    [Serializable]
    private class Animations
    {
        public string turn, shocked;
    }
}
