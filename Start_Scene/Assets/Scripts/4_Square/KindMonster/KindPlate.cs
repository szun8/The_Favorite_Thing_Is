using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KindPlate : MonoBehaviourPunCallbacks
{
    public int isKind; //1이면 거북이는 위에 있음

    public bool isLight = false;

    public bool isgreen = false;    

    PhotonView PV;
    bool isPlayer = false;

    private bool oneSend = false; //패킷 제한 용도 

    RaycastHit player;

    void Awake() => PV = GetComponent<PhotonView>();


    void Update()
    {
        UpDownLay(); //플레이어 감지하는 레이를 발사

        if (PhotonNetwork.InRoom) CheckLight();
        
    }

    void UpDownLay()
    {
        if (isKind == 1)
        {
            Debug.DrawRay(transform.position, Vector3.down * 2.5f, Color.blue);
            isPlayer = Physics.Raycast(transform.position, Vector3.down, out player, 2.5f, LayerMask.GetMask("Player"));
        }

        else if (isKind == 0)
        {
            Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), Vector3.up * 1.7f, Color.blue);
            isPlayer = Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.up, out player, 1.7f, LayerMask.GetMask("Player"));
        }
    }

    void CheckLight() //플레이어가 있는경우 g를 눌렀을 때 true
    {
        if (isPlayer && player.collider != null) //단상이 플레이어 감지 + 감지된 플레이어 collider 있  
        {
            if (player.collider.gameObject.GetComponentInParent<MultiPlayerMove>().g_pressed)
            {
                if (!isLight)
                {
                    PV.RPC("SyncGreen", RpcTarget.AllBuffered, true);
                    oneSend = true;
                }
            }
            else if(oneSend)
            {
                PV.RPC("SyncGreen", RpcTarget.AllBuffered, false);
                oneSend = false;
            }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) PV.RPC("SyncGreen", RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    void SyncGreen(bool value)
    {
        isgreen = isLight = value;
        
    }
}
