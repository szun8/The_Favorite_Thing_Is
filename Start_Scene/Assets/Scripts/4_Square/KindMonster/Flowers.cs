using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Flowers : MonoBehaviourPun
{
    PhotonView PV;
    Animator animator;
    public KindPlate kindPlate;     //단상이 초록이면 꽃피게할 isgreen 가져오려고
    public KindMonster kindMonster;

    //rpc 한번만 보내주기 위해 
    private bool isSendOne, lastSend = false;

    public bool isBloom = false;    //거북이 유인 변수 
    
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!kindMonster.isArrive)
        {
            if (kindPlate.isgreen && !isSendOne)
            {
                PV.RPC("SyncAnim", RpcTarget.AllBuffered, true);
                isSendOne = true; //isSendOne을 true해줘서 더이상 G누르고 있어도 패킷 안보냄 
            }

            else if (!kindPlate.isgreen && isSendOne)
            {
                PV.RPC("SyncAnim", RpcTarget.AllBuffered, false);
                isSendOne = false;
            }
        }
        else if (kindMonster.isArrive && !lastSend) //꽃에 도달하면 꽃은 피어있는 상태로 
        {
            PV.RPC("SyncAnim", RpcTarget.AllBuffered, true);
            lastSend = true;
        }
    }

    [PunRPC]
    void SyncAnim(bool value)
    {
        animator.SetBool("isBloom", value);
        isBloom = value;
    }
}
