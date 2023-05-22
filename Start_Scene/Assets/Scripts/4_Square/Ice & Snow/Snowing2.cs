using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Snowing2 : MonoBehaviourPun
{
    public Animator stairAnim; //계단 애니메이션 
    public Snowing2 anotherPlate;
    PhotonView PV;

    public bool isLight = false; //이 단상에서 플레이어가 빛내니?
    public int playerCnt =0;

    GameObject Player;
    GameObject P2;

    //ray 검출용 
    private bool isPlayer = false;
    RaycastHit player;

    private bool isSendOne = false; //패킷 반복 수 줄이기

    void Awake() => PV = GetComponent<PhotonView>();

    void Update()
    {
        Ray();

        if (PhotonNetwork.InRoom)
        { 
            CheckLight(); //플레이어가 단상 레이에 검출 될때 키고 안키고
        }
        
    }

    void Ray()
    {
        Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), Vector3.up * 1.7f, Color.blue);
        isPlayer = Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.up, out player, 1.7f, LayerMask.GetMask("Player"));
    }


    private void OnCollisionEnter(Collision collision)
    { 
        if (collision.gameObject.CompareTag("Player"))
        {
            playerCnt++;
            if (playerCnt == 1) Player = collision.gameObject;
            else if (playerCnt == 2) P2 = collision.gameObject;

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           playerCnt--;

           if (P2 == null) //1인용 단상 경우 
           {
               Player = null;
               if (isSendOne && isLight && !anotherPlate.isLight) // 내 불은 켜져있고, 상대 불은 꺼져 있는 경우만 exit시 
               {
                   PV.RPC("SyncBlue", RpcTarget.AllBuffered, false);
                   PV.RPC("SyncSnow", RpcTarget.AllBuffered, false);
               }

           }

           else if (P2 != null) //1 2p 중 1p가 나간 경우 2p를 1p로 해줘야함 
           {
               if (collision.gameObject == Player)
               {
                   Player = P2;
                   P2 = null;
               }

               else if (collision.gameObject == P2) P2 = null;
           }
           
        }
    }

    void CheckLight()
    {
        //ray 검출 되고 + 검출 된 놈이 먼저 들어온 놈이어야 함 
        if (isPlayer && player.collider != null && Player != null)
        {
            if(player.collider.transform.parent.gameObject == Player)
            {
                //내가 b를 키고 상대 불은 꺼져있는 경우
                if (player.collider.gameObject.GetComponentInParent<MultiPlayerMove>().b_pressed && !anotherPlate.isLight)
                {
                    PV.RPC("SyncBlue", RpcTarget.AllBuffered, true);
                    PV.RPC("SyncSnow", RpcTarget.AllBuffered, true);
                }

                //내가 레이에서 상대가 불 꺼져 있을 때에 내가 불 안키고 있으면 불 꺼짐
                else if (isSendOne && !anotherPlate.isLight) //켰다가 레이 위에서 꺼져있을때
                {
                    PV.RPC("SyncBlue", RpcTarget.AllBuffered, false);
                    PV.RPC("SyncSnow", RpcTarget.AllBuffered, false);
                }
            }
            
        }
        else if (Player !=null && isSendOne && !anotherPlate.isLight) //단상 레이 아예 나가버린 경우 
        {
            PV.RPC("SyncBlue", RpcTarget.AllBuffered, false);
            PV.RPC("SyncSnow", RpcTarget.AllBuffered, false);
        }
    }

    [PunRPC]
    void SyncBlue(bool value)
    {
        isLight = value;
        isSendOne = value;
    }

    [PunRPC]
    void SyncSnow(bool value)
    {
        stairAnim.SetBool("isLight", value);
    }
}
