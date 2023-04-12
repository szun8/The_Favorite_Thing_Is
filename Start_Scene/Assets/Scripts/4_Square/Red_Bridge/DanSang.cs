using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DanSang : MonoBehaviourPunCallbacks
{
    Animator animator;
    PhotonView PV;

    GameObject bridge; //해당 단상과 연결이 되는 발판 
    

    RaycastHit player;
    bool isPlayer = false;

    //L,R,G,B 단상 중 무슨 단상인지
    bool light_L = false;
    bool light_R = false;
    bool light_G = false;
    bool light_B = false;


    void Awake()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponentInChildren<Animator>();      //자식(발판)의 애니메이터 가져오기 
        bridge = gameObject.transform.GetChild(0).gameObject; //자식 불러오기 (발판)
        CheckPlateColor();
        
    }

    
    void Update()
    {
        isPlayer = Physics.Raycast(transform.position, transform.up, out player, 0.5f);

        if (PhotonNetwork.InRoom) //플레이어가 방안에 있는지 
        {
            if (isPlayer) PV.RPC("CheckPlayerLight", RpcTarget.AllBuffered);  //플레이어가 위에 있다면 빛내는지 검출하자 

            else PV.RPC("SyncAnim", RpcTarget.AllBuffered, false);   //플레이어 없으면 색 감추자  

        }

    }

    void CheckPlateColor()
    {
        if (gameObject.CompareTag("L_Plate")) light_L = true;
        else if (gameObject.CompareTag("R_Plate")) light_R = true;
        else if (gameObject.CompareTag("G_Plate")) light_G = true;
        else if (gameObject.CompareTag("B_Plate")) light_B = true;
    }

    [PunRPC]
    void CheckPlayerLight() //플레이어 빨강이 눌렸는지에 따른 처리 
    {
        if(player.collider != null) //플레이어가 위에서 검출이되면 
        {
            if (light_R)
            {
                if (player.collider.GetComponentInParent<MultiPlayerMove>().r_pressed)
                {
                    bridge.GetComponent<BoxCollider>().isTrigger = false;
                    PV.RPC("SyncAnim", RpcTarget.AllBuffered, true);
                }
                else
                {
                    PV.RPC("SyncAnim", RpcTarget.AllBuffered, false);

                    AnimatorStateInfo curAnim = animator.GetCurrentAnimatorStateInfo(0); //현재 진행중인 애니메이션 상태 가져옴 

                    if (curAnim.IsName("R_Off") && curAnim.normalizedTime >= 0.99f) //애니메이션 이름이 R_Off이고, 99%이상 완료된 경우 
                    {
                        bridge.GetComponent<BoxCollider>().isTrigger = true;
                    }

                }
            }
            else if (light_G)
            {
                if (player.collider.GetComponentInParent<MultiPlayerMove>().g_pressed)
                {
                    //발판 녀석의 메시콜라이더 컴포넌트의 트리거를 False
                    // rpc SyncAnim해주기
                }
            }

            else if (light_B)
            {

            }
            else
            {

            }
           

        }
    }

    [PunRPC]
    void SyncAnim(bool value)  //애니메이션 변수 동기화 
    {
        animator.SetBool("isLight", value);
    }

}
