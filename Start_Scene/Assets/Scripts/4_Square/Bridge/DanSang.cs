using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DanSang : MonoBehaviourPunCallbacks
{
    public int up;
    Animator animator;
    PhotonView PV;

    public GameObject bridge; //해당 단상과 연결이 되는 발판 


    private GameObject Player; // 단상에 충돌한 플레이어 

    private bool isNoLight = false; //플레이어가 단상에 있지만 빛 안키면 true되게 하자 

    private bool isL, noL = false; // 계속해서 패킷 보내기 방지 

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponentInChildren<Animator>();      //자식(발판)의 애니메이터 가져오기 

    }


    void Update()
    {
        if (PhotonNetwork.InRoom) //플레이어가 방안에 있는지 + 단상에 있는지 
        {
            if(Player != null) //플레이어가 단상에 충돌 ENTER하면 여기로 
            {
                //플레이어가 l눌러야하고 + 충돌뿐 아니라 밟고 있어야 함 
                if (Player.GetComponent<MultiPlayerMove>().l_pressed && Player.GetComponent<MultiPlayerMove>().isGround
                    && !isL)
                {
                    noL = false;
                    PV.RPC("SyncAnim", RpcTarget.AllBuffered, true);
                    isL = true;
                }

                else if (!Player.GetComponent<MultiPlayerMove>().l_pressed && !noL)
                {
                    isL = false;
                    PV.RPC("SyncAnim", RpcTarget.AllBuffered, false);
                    noL = true;
                } 
                    
            }

            else //플레이어가 단상에 아예 없을 때, 
            {
                if (isNoLight) //플레이어가 처음으로 불켰다 꺼야만 이게 true되서 활성화됨 
                {
                    PV.RPC("SyncAnim", RpcTarget.AllBuffered, false);
                    noL = isL = false;
                    isNoLight = false;
                }
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) Player = collision.gameObject;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) Player = null;
    }


    [PunRPC]
    void SyncAnim(bool value)  //애니메이션 변수 동기화 
    {
        animator.SetBool("isLight", value);

        //불 키면 서서히 밝아지면서 collider 생김 
        if (value) bridge.GetComponent<MeshCollider>().isTrigger = false;

        else //불 끄면 서서히 어두워지고 90퍼 되야 trigger 
        {
            AnimatorStateInfo curAnim = animator.GetCurrentAnimatorStateInfo(0); //현재 진행중인 애니메이션 상태 가져옴 
            if (curAnim.IsName("L_Off") && curAnim.normalizedTime >= 0.9f)//애니메이션 이름이 R_Off이고, 90%이상 완료된 경우 
            {
                bridge.GetComponent<MeshCollider>().isTrigger = true;
                isNoLight = true;
            }

        }

    }



}
