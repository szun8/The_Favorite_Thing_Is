/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro; //textmeshpro 쓸 때

public class WaitingWall : MonoBehaviourPunCallbacks
{
    BoxCollider boxCollider;        //벽 콜라이더 
    NetworkManager networkManager;
    CaveTalkManager caveTalkManager;
    PhotonView PV;


    private int playerLightCount = 0;    //네트워크 매니저의 변수를 플레이어무브에서 추가해주고 여기로 가져온다 
    public int collisionCount = 0;  //플레이어 무브에서 이 변수를 접근해줄 거라 public 
    
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        caveTalkManager = GameObject.Find("TalkManager/Canvas/Talk/Wait").GetComponent<CaveTalkManager>();
        PV = GetComponent<PhotonView>();
    }

    
    void Update()
    {
        this.playerLightCount = networkManager.playerLightCount; //플레이어 생성보다 이게 더 빨라서 update에서 해주자 ,,

        //두명이 부딪히면서, 빛을 둘다 켰으면 벽은 trigger 
        if (playerLightCount == 2 && collisionCount ==2)
        {
            boxCollider.isTrigger = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //두명이 빛을 내지만 둘이서 박지 않는다면 자막이 나오기 위해 isOpen ->true , noLight->true
        if (collision.gameObject.CompareTag("Player")) PV.RPC("SendisOpenTrue", RpcTarget.AllBuffered);
       
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) PV.RPC("SendisOpenFalse", RpcTarget.AllBuffered);

    }

    [PunRPC]
    void SendisOpenTrue()
    {
        if (collisionCount < 2 && playerLightCount <= 2) caveTalkManager.isOpen=true;
    }

    [PunRPC]
    void SendisOpenFalse()
    {
        caveTalkManager.isOpen = false;
    }
}*/
