using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceColor : MonoBehaviour
{
    public RGBLamp BlueLamp;

    Animator animator;

    private bool isDone = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (BlueLamp.isColor && !isDone)
        {
            animator.SetBool("isIce", true);
            isDone = true;
        }
    }
}
