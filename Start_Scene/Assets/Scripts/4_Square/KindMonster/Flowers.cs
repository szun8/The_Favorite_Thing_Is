using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Flowers : MonoBehaviourPun
{
    PhotonView PV;
    Animator animator;
    public KindPlate kindPlate;

    //rpc 한번만 보내주기 위해 
    private bool isgood, isNo = false;
    
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (kindPlate.isgreen && !isgood)
        {
            isNo = true;
            PV.RPC("SyncAnim", RpcTarget.AllBuffered, true);
            isgood = true; //isgood을 true해줘서 더이상 G누르고 있어도 패킷 안보냄 
        }

        else if(!kindPlate.isgreen && isNo)
        {
            isgood = false;
            PV.RPC("SyncAnim", RpcTarget.AllBuffered, false);
            isNo = false;  //isNo를 false해줘서 더이상 단상에 G안누르고 서있기만해도 보내는 패킷 제거 
        }
    }

    [PunRPC]
    void SyncAnim(bool value)
    {
        animator.SetBool("isBloom", value);
    }
}
