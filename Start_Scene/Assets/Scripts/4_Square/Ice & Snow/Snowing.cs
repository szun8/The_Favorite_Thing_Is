using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Snowing : MonoBehaviourPun
{
    public Animator stairAnim;
    PhotonView PV;
    Animator snowAnim;

    public int isPlateup; // 0 이면 단상이 위에 있음 
    public bool isLight = false; //플레이어가 불키면 이게 true가 되고 쿵쿵이 스크립트에서 조건으로 쓴다 

    private bool isPlayer = false;
    RaycastHit player;

    private bool isSendOne = false; //패킷 반복 수 줄이기

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        snowAnim = GetComponent<Animator>();
        
    }

    void Update()
    {
        UpDownLay(); //플레이어 감지하는 레이를 발사 

        if (PhotonNetwork.InRoom)
        {
            CheckLight();
        }
    }


    void UpDownLay() //플레이어 감지 레이 발사 
    {
        if (isPlateup == 1)
        {
            Debug.DrawRay(transform.position, Vector3.down * 2.5f, Color.blue);
            isPlayer = Physics.Raycast(transform.position, Vector3.down, out player, 2.5f, LayerMask.GetMask("Player"));
        }

        else if (isPlateup == 0)
        {
            Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), Vector3.up * 1.7f, Color.blue);
            isPlayer = Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.up, out player, 1.7f, LayerMask.GetMask("Player"));
        }
    }

    void CheckLight() //플레이어가 있는경우 B을 눌렀을 때 true
    {
        if (isPlayer && player.collider != null)
        {
            if (player.collider.gameObject.GetComponentInParent<MultiPlayerMove>().b_pressed)
            {
                if (!isLight) //단상을 최초로 킨 경우
                {   
                    PV.RPC("SyncBlue", RpcTarget.AllBuffered, true);
                    PV.RPC("SyncSnow", RpcTarget.AllBuffered, true);
                    isSendOne = true;
                }
            } 
            else if(isSendOne) //켰다가 레이 위에서 꺼져있을때
            {
                PV.RPC("SyncBlue", RpcTarget.AllBuffered, false);
                PV.RPC("SyncSnow", RpcTarget.AllBuffered, false);
                isSendOne = false;
            }
            
        }
        else if (isSendOne) //단상 레이 아예 나가버린 경우 
        {
            PV.RPC("SyncBlue", RpcTarget.AllBuffered, false);
            PV.RPC("SyncSnow", RpcTarget.AllBuffered, false);
            isSendOne = false;
        }

    }


    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && isSendOne)
        {
            PV.RPC("SyncBlue", RpcTarget.AllBuffered, false);
            PV.RPC("SyncSnow", RpcTarget.AllBuffered, false);
            isSendOne = false;
        }
    }

    [PunRPC]
    void SyncBlue(bool value) => isLight = value;

    [PunRPC]
    void SyncSnow(bool value)
    {
        snowAnim.SetBool("isLight", value);
        stairAnim.SetBool("isLight", value);
    }

}
