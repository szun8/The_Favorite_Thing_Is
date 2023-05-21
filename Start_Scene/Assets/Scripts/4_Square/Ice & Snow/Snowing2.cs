using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Snowing2 : MonoBehaviourPun
{
    public Animator stairAnim; //계단 애니메이션 

    PhotonView PV;

    public bool isLight = false; //이 단상에서 플레이어가 빛내니?

    //ray 검출용 
    private bool isPlayer = false;
    RaycastHit player;

    private bool isSendOne = false; //패킷 반복 수 줄이기

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
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


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           if(isSendOne && stairAnim.GetBool("isLight") && isLight)
           {
                PV.RPC("SyncBlue", RpcTarget.AllBuffered, false);
                PV.RPC("SyncSnow", RpcTarget.AllBuffered, false);
           }
        }
    }

    void CheckLight()
    {
        if (isPlayer && player.collider != null)
        {
            if (player.collider.gameObject.GetComponent<MultiPlayerMove>().b_pressed && !stairAnim.GetBool("isLight"))
            {
                PV.RPC("SyncBlue", RpcTarget.AllBuffered, true);
                PV.RPC("SyncSnow", RpcTarget.AllBuffered, true);
                    //isSendOne = true;
                    /*if (!isLight) //단상을 최초로 킨 경우
                    {
                    
                    }*/
            }

            else if (isSendOne && stairAnim.GetBool("isLight")) //켰다가 레이 위에서 꺼져있을때
            {
                PV.RPC("SyncBlue", RpcTarget.AllBuffered, false);
                PV.RPC("SyncSnow", RpcTarget.AllBuffered, false);
                    //isSendOne = false;
            }
            
        }
        else if (isSendOne && stairAnim.GetBool("isLight")) //단상 레이 아예 나가버린 경우 
        {
            PV.RPC("SyncBlue", RpcTarget.AllBuffered, false);
            PV.RPC("SyncSnow", RpcTarget.AllBuffered, false);
            //isSendOne = false;
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
