using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fire : MonoBehaviourPun
{
    public GameObject RedLamp;

    private RGBLamp rgbLamp;

    Animator animator;
    PhotonView PV;

    private bool isDone = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
        rgbLamp = RedLamp.GetComponent<RGBLamp>();
    }

    void Update()
    {
        if (!isDone && rgbLamp.isColor) PV.RPC("SyncAnim", RpcTarget.AllBuffered); 
    }

    [PunRPC]
    void SyncAnim()
    {
        animator.SetBool("isFire", true);
        isDone = true;
    }

}
