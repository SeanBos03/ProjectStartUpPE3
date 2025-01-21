using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("animState", 1);
    }

    void Update()
    {
        if (Input.GetKey("w"))
        {
            animator.SetInteger("animState", 2);
        }
        else
        {
            animator.SetInteger("animState", 1);
        }
    }
}
