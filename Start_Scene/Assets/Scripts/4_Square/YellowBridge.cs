using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class YellowBridge : MonoBehaviourPun
{
    PhotonView PV;
    Animator animator;
    //MeshCollider meshCollider;

    public DanSang red;
    public DanSang green;

    public bool isDone; //rpc 함수 한번만 하게 하려고 

    
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        //meshCollider = GetComponent<MeshCollider>();
    }

    
    void Update()
    {
        if(!isDone && red.plateLight && green.plateLight)
        {
            PV.RPC("SyncAnim", RpcTarget.AllBuffered);
            //meshCollider.isTrigger = false;
            isDone = true;
        }
    }

    [PunRPC]
    void SyncAnim() => animator.SetBool("isLight", true);
 
}
