using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cloud : MonoBehaviour
{
    public RGBLamp BlueLamp;

    Animator animator;

    public bool isDone = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (BlueLamp.isColor && !isDone)
        {
            animator.SetBool("isCloud", true);
            isDone = true;
        }
    }
}
