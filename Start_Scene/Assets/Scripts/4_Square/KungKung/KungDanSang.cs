using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KungDanSang : MonoBehaviourPunCallbacks
{
    public int isKungup; //1이면  쿵쿵이가 위에 있음 

    public bool isLight;

    public bool isRed = false;

    private PhotonView PV;
    private bool isPlayer = false;

    RaycastHit player;


    void Awake()=> PV = GetComponent<PhotonView>();


    void Update()
    {
        UpDownLay(); //플레이어 감지하는 레이를 발사 

        if(PhotonNetwork.InRoom) CheckLight();


    }

    void UpDownLay()
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

    void CheckLight()
    {
        if (isPlayer && player.collider != null)
        {
            if (player.collider.gameObject.GetComponentInParent<MultiPlayerMove>().r_pressed )
            {
                if(!isLight) PV.RPC("SyncRed", RpcTarget.AllBuffered, true);
            }
            else PV.RPC("SyncRed", RpcTarget.AllBuffered, false);

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player")) PV.RPC("SyncRed", RpcTarget.AllBuffered, false);
    }


    [PunRPC]
    void SyncRed(bool value)
    {
        isLight = value;

        if (value) isRed = true;
    }

}
