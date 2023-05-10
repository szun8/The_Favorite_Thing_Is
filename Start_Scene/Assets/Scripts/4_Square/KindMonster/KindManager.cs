using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KindManager : MonoBehaviourPun
{
    public Animator animator;   //거북이 녀석의 animator
    public KindPlate kindPlate;
    public KindMonster kindMonster;

    PhotonView PV;

    private bool oneWalkSend, lastSend = false; //패킷 제한 용도 anim walk rpc true신호 하나 보냈는지
    public int p_count = 0;

    public bool isWalk = false;  //kindMonster cs에서 움직이게 용도

    

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!kindMonster.isArrive)
        {
            //플레이어가 초록빛 내면 + 아직 Rpc로 신호 안보냈으면 
            if (kindPlate.isgreen && !oneWalkSend)
            {
                PV.RPC("SyncWalk", RpcTarget.AllBuffered, true);
                oneWalkSend = true;
            }

            //플레이어가 초록빛 안내면 + rpc로 한번 walk 줬었으면 
            else if (!kindPlate.isgreen && oneWalkSend)
            {
                PV.RPC("SyncWalk", RpcTarget.AllBuffered, false);
                oneWalkSend = false;
            }
        }

        else if (kindMonster.isArrive && !lastSend)
        {
            PV.RPC("SyncWalk", RpcTarget.AllBuffered, false);
            PV.RPC("SyncWake", RpcTarget.AllBuffered, true);
            lastSend = true;
        }
    }

    //trigger에 진입시 거북이 깨어나기 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player_mesh"))
        {
            p_count++;
            if (!lastSend)
            {
                PV.RPC("SyncWake", RpcTarget.AllBuffered, true);

            }
        }
        
 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player_mesh"))
        {
            p_count--;
            if (!lastSend && p_count == 0)
            {
                PV.RPC("SyncWake", RpcTarget.AllBuffered, false);
            }
        }
  
    }

    [PunRPC]
    void SyncWake(bool value) => animator.SetBool("isWake", value);

    [PunRPC]
    void SyncWalk(bool value)
    {
        animator.SetBool("isWalk", value);
        isWalk = value;
    }







}
