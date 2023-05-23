using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject RedLamp;

    private RGBLamp rgbLamp;

    Animator animator;

    public bool isDone = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rgbLamp = RedLamp.GetComponent<RGBLamp>();
    }

    void Update()
    {
        if (!isDone && rgbLamp.isColor)
        {
            animator.SetBool("isFire", true);
            isDone = true;
        }//PV.RPC("SyncAnim", RpcTarget.AllBuffered); 
    }

}
