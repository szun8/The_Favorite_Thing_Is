using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DanSang : MonoBehaviourPunCallbacks
{
    Animator animator;
    PhotonView PV;

    GameObject bridge;
    

    RaycastHit player;
    bool isPlayer = false;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponentInChildren<Animator>();      //자식(발판)의 애니메이터 가져오기 
        bridge = gameObject.transform.GetChild(0).gameObject; //자식 불러오기 (발판) 
    }

    // Update is called once per frame
    void Update()
    {
        isPlayer = Physics.Raycast(transform.position, transform.up, out player, 0.5f);

        if (PhotonNetwork.InRoom) //플레이어가 방안에 있는지 
        {
            if (isPlayer) PV.RPC("CheckPlayerLight", RpcTarget.AllBuffered);  //플레이어가 위에 있다면 빛내는지 검출하자 

            else PV.RPC("SyncAnim", RpcTarget.AllBuffered, false);   //플레이어 없으면 색 감추자  

        }

    }

    [PunRPC]
    void CheckPlayerLight() //플레이어 빨강이 눌렸는지에 따른 처리 
    {
        if(player.collider != null) //플레이어가 위에서 검출이되면 
        {
            if (player.collider.GetComponentInParent<MultiPlayerMove>().r_pressed) // r을 누른 상태면 
            {
                bridge.GetComponent<BoxCollider>().isTrigger = false;
                PV.RPC("SyncAnim", RpcTarget.AllBuffered, true);
            }
            else
            {
                PV.RPC("SyncAnim", RpcTarget.AllBuffered, false);

                AnimatorStateInfo curAnim = animator.GetCurrentAnimatorStateInfo(0);

                if(curAnim.IsName("R_Off") && curAnim.normalizedTime >= 0.99f)
                {
                    bridge.GetComponent<BoxCollider>().isTrigger = true;
                }
                
            }

        }
    }

    [PunRPC]
    void SyncAnim(bool value)  //애니메이션 변수 동기화 
    {
        animator.SetBool("isLight", value);
    }

}
