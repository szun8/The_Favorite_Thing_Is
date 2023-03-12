using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro; //textmeshpro 쓸 때

public class CaveTalkManager : MonoBehaviourPunCallbacks
{
    Animator animator;
    TMP_Text tmp;
    PhotonView PV;

    // 2명이 들어와서 불키고 부딪히면 벽이 열림 
    public bool isOpen = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
        tmp = GetComponent<TMP_Text>();
        tmp.text = "빛의 세기가 부족해,,";
    }

    
    void Update()
    {
        //현재 포톤 네트워크에 연결되어있고 && 플레이어가 방 안에 있는지 
        if(PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
            PV.RPC("SetAnimBool", RpcTarget.AllBuffered);

    }

    [PunRPC]
    void SetAnimBool()
    {
        //isOpen ->true 면 자막나오는 bool 을 true로 바꿔~ 
        if (isOpen) animator.SetBool("noLight", true);
        else if (!isOpen) animator.SetBool("noLight", false);
    }
}
