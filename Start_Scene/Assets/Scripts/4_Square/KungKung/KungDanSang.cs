using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KungDanSang : MonoBehaviourPunCallbacks
{
    public int isKungup; //1이면  쿵쿵이가 위에 있음 

    public bool isLight;

    public bool isRed = false; //isLight일시 이게 true가 되고 쿵쿵이 스크립트에서 조건으로 쓴다

    private PhotonView PV;
    private bool isPlayer = false;

    RaycastHit player;

    private bool isSendOne = false; //패킷 반복 수 줄이기 

    void Awake()=> PV = GetComponent<PhotonView>();


    void Update()
    {
        UpDownLay(); //플레이어 감지하는 레이를 발사 

        if(PhotonNetwork.InRoom) CheckLight();


    }

    void UpDownLay() //플레이어 감지 레이 발사 
    {
        if (isKungup == 1)
        {
            Debug.DrawRay(transform.position, Vector3.down * 2.5f, Color.blue);
            isPlayer = Physics.Raycast(transform.position, Vector3.down, out player, 2.5f, LayerMask.GetMask("Player"));
        }

        else if (isKungup == 0)
        {
            Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), Vector3.up * 1.7f, Color.blue);
            isPlayer = Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.up, out player, 1.7f, LayerMask.GetMask("Player"));
        }



    }

    void CheckLight() //플레이어가 있는경우 r을 눌렀을 때 true
    {
        if (isPlayer && player.collider != null)
        {
            if (player.collider.gameObject.GetComponentInParent<MultiPlayerMove>().r_pressed)
            {
                if (!isLight)
                {
                    PV.RPC("SyncRed", RpcTarget.AllBuffered, true);
                    isSendOne = true;
                }

            }
            /*else if (isSendOne)
            {
                PV.RPC("SyncRed", RpcTarget.AllBuffered, false);
            }*/

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && isSendOne) PV.RPC("SyncRed", RpcTarget.AllBuffered, false);
    }


    [PunRPC]
    void SyncRed(bool value)
    {
        isLight = value;

        if (value) isRed = true;
    }

}
