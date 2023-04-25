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


    private GameObject Player;

    private bool isOnPlayer = false;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponentInChildren<Animator>();      //자식(발판)의 애니메이터 가져오기 

    }


    void Update()
    {
        if (PhotonNetwork.InRoom ) //플레이어가 방안에 있는지 + 단상에 있는지 
        {
            if(Player != null)
            {
                if (Player.GetComponent<MultiPlayerMove>().l_pressed && Player.GetComponent<MultiPlayerMove>().isGround)
                {
                    PV.RPC("SyncAnim", RpcTarget.AllBuffered, true);  //플레이어가 위에 있다면 빛내는지 검출하자
                }

                else if(!Player.GetComponent<MultiPlayerMove>().l_pressed)
                    PV.RPC("SyncAnim", RpcTarget.AllBuffered, false);

                isOnPlayer = true;
            }

            else
            {
                if (isOnPlayer)
                {
                    PV.RPC("SyncAnim", RpcTarget.AllBuffered, false);
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

        if (value) bridge.GetComponent<MeshCollider>().isTrigger = false;

        else
        {
            AnimatorStateInfo curAnim = animator.GetCurrentAnimatorStateInfo(0); //현재 진행중인 애니메이션 상태 가져옴 
            if (curAnim.IsName("L_Off") && curAnim.normalizedTime >= 0.9f)//애니메이션 이름이 R_Off이고, 90%이상 완료된 경우 
            {
                bridge.GetComponent<MeshCollider>().isTrigger = true;
                isOnPlayer = false;
            }

        }

    }



}
