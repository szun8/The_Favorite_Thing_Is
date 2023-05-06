using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//이 스크립트는 부모인 가지한테 있음 
public class Ivy : MonoBehaviourPun
{
    public GameObject GLamp; //빛나는 잎들 구분하기 위해 
    private RGBLamp rgbLamp; //해당 램프의 스크립트 

    Animator animator;  //나뭇가지 자식 빛나는 잎들한테 animator가 있음 
    PhotonView PV;

    private bool isDone = false;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        rgbLamp = GLamp.GetComponent<RGBLamp>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if(!isDone && rgbLamp.isColor) PV.RPC("SyncAnim", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void SyncAnim()
    {
        animator.SetBool("isIvy", true);
        isDone = true;
 
    }
}
