using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Snowing2 : MonoBehaviourPun
{
    public Animator stairAnim; //계단 애니메이션 

    PhotonView PV;

    public bool isLight = false; //이 단상에서 플레이어가 빛내니?
    public int playerCnt = 0; // 발판을 밟고 있는 플레이어 수

    private GameObject Player; // 단상에 충돌한 플레이어 
    private GameObject P2;


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

                if (isSendOne && stairAnim.GetBool("isLight")) // 플레이어가 빛을 안끈 상태로 단상 탈출시 안꺼지는 문제 해결
                {
                    PV.RPC("SyncBlue", RpcTarget.AllBuffered, false);
                    PV.RPC("SyncSnow", RpcTarget.AllBuffered, false);
                    isSendOne = false;
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
        if (isPlayer && player.collider.gameObject == Player)
        {
            Debug.Log("1p입징 ");
            if (Player.GetComponent<MultiPlayerMove>().b_pressed && !stairAnim.GetBool("isLight"))
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
